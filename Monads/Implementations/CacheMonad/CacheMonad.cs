/*
 *  Copyright (C) 2014  Muraad Nofal

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Monads.Extension.AtomicExtensions;

namespace Monads
{
    public class CacheMonad<K, V> : Monad<CacheEntry<K, V>> 
    {
        Dictionary<K, CacheEntry<K, V>> cacheDict = new Dictionary<K, CacheEntry<K, V>>();

        long pufferTimeMs = 0;

        long lastCleanupTicks = 0;
        long ticksToCleanup = 10000;        // default every second. if get is called.
        int cacheCountForAsyncCleanUp = 1000;

        // used as atomic boolean.
        Identity<bool> isCleaningUp = false;

        /// <summary>
        /// A function that is used to calculate a key from the given value.
        /// Used for function Add(V value).
        /// </summary>
        public Func<V, K> KeyCalculator = null;

        /// <summary>
        /// Action that is called for every CacheEntry that is removed from the cache monad on TryCleanUp().
        /// </summary>
        public Action<CacheEntry<K, V>> CleanUpAction = null;

        public CacheMonad()
        {
        }

        public CacheMonad(long maxPufferTimeMs, long maxTicksToCleanup, int minCountForAsyncCleanUp = 1000)
        {
            pufferTimeMs = maxPufferTimeMs;
            ticksToCleanup = maxTicksToCleanup;
            cacheCountForAsyncCleanUp = minCountForAsyncCleanUp;
        }

        #region CacheMonad implementation

        public int Count
        {
            get { return cacheDict.Count; }
        }

        /*public V this[K key]
        {
            get
            {
                CacheEntry<K, V> result = null;
                TryGet(key, out result);
                return result == null ? default(V) : result.Value;
            }
            set
            {
                Add(value);
            }
        }*/

        /// <summary>
        /// Operator overloading.
        /// </summary>
        /// <param name="key">The key.
        /// Usage:
        /// var cacheEntry = cacheMonad[key];
        /// or 
        /// cacheMonad[key] = cacheEntry;</param>
        /// <returns> </returns>
        public CacheEntry<K, V> this[K key]
        {
            get
            {
                CacheEntry<K, V> result = null;
                TryGet(key, out result);
                return result;
            }
            set
            {
                Append(value);
            }
        }

        /// <summary>
        /// Trys to write a cache entry for a given key to the entry parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>True if CacheEntry for given key was found. False otherwise.</returns>
        public bool TryGet(K key, out CacheEntry<K, V> entry)
        {
            bool result = false;
            Lock.EnterReadLock();
            try
            {
                result = cacheDict.TryGetValue(key, out entry);
            }
            finally
            {
                Lock.ExitReadLock();
            }

            TryCleanUp();

            return result;
        }

        /// <summary>
        /// Returns a cache entry for a given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The cache entry.</returns>
        public CacheEntry<K, V> Get(K key)
        {
            CacheEntry<K, V> result = null;
            Lock.EnterReadLock();
            try
            {
                result = cacheDict[key];
            }
            finally
            {
                Lock.ExitReadLock();
            }

            // Trigger cleanup if needed.
            TryCleanUp();

            return result;
        }

        /// <summary>
        /// Adds a new value to the cache monad.
        /// KeyCalculator function is used to create a key from the given value.
        /// If KeyCalculator is null a NullReferenceException is thrown.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>this.</returns>
        public CacheMonad<K, V> Add(V value)
        {
            if (KeyCalculator == null)
                throw new NullReferenceException("Add: KeyCalculator is null");

            Append(new CacheEntry<K,V>(KeyCalculator(value), value));
            return this;
        }

        /// <summary>
        /// Adds a new value with given key to the cache monad.
        /// </summary>
        /// <param name="key">The value key.</param>
        /// <param name="value">The value.</param>
        /// <returns>this.</returns>
        public CacheMonad<K, V> Add(K key, V value)
        {
            Append(new CacheEntry<K, V>(key, value));
            return this;
        }

        /// <summary>
        /// Adds a new CacheEntry to the cache monad. 
        /// If the cache already contains a value for the given key the original value is overwritten. 
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>this.</returns>
        public override Monad<CacheEntry<K, V>> Append(CacheEntry<K, V> value)
        {
            Lock.EnterWriteLock();
            try
            {
                cacheDict[value.Key] = value;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return this;
        }

        /// <summary>
        /// Tries to remove old cache entrys.
        /// First checks if ticks since last clean up are greater than ticksToCleanUp.
        /// Then if needed removes all CacheEntry´s that have reached the maxPufferTimeMs.
        /// </summary>
        /// <returns></returns>
        public int TryCleanUp()
        {
            int result = 0;

            // check if some other thread is already trying to clean up the cache dictionary.
            if(isCleaningUp.CompareExchange(true, (b) => b == false))
            {
                // Trigger cleanup if needed.
                if (DateTime.Now.Ticks - lastCleanupTicks >  ticksToCleanup)
                {
                    lastCleanupTicks = DateTime.Now.Ticks;
                    if (cacheDict.Count >= cacheCountForAsyncCleanUp)
                    {
                        var task = Task.Run(() => cleanUp());
                        task.Wait();
                        result = task.Result;
                    }
                    else
                        result = cleanUp();
                }
                isCleaningUp = false;
            }
            return result;
        }

        private int cleanUp()
        {
            int removed = 0;
            Lock.EnterWriteLock();
            try
            {
                var toBeRemoved = cacheDict.Where((entry) => Math.Abs(entry.Value.DateTime.Diff(DateTime.Now)) > pufferTimeMs).ToList();
                foreach (var value in toBeRemoved)
                {
                    if (CleanUpAction != null)
                        CleanUpAction(value.Value);

                    cacheDict.Remove(value.Key);
                    removed++;
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return removed;
        }

        #endregion 

        #region Monad implementation

        public override Monad<CacheEntry<K, V>> Fmap(Func<CacheEntry<K, V>, CacheEntry<K, V>> function)
        {
            Lock.EnterWriteLock();
            try
            {
                Dictionary<K, CacheEntry<K, V>> tmp = new Dictionary<K, CacheEntry<K, V>>();

                foreach (var element in cacheDict.Values)
                    tmp.Add(element.Key, function(element));

                foreach (var element in tmp)
                    cacheDict[element.Key] = element.Value;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return this;
        }

        public override Monad<CacheEntry<K, V>> Fmap(Func<CacheEntry<K, V>, int, CacheEntry<K, V>> function)
        {
            int index = 0;
            Lock.EnterWriteLock();
            try
            {
                Dictionary<K, CacheEntry<K, V>> tmp = new Dictionary<K, CacheEntry<K, V>>();

                foreach (var element in cacheDict.Values)
                    tmp.Add(element.Key, function(element, index++));

                foreach (var element in tmp)
                    cacheDict[element.Key] = element.Value;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return this;
        }

        public override Monad<B> Fmap<B>(Func<CacheEntry<K, V>, B> function)
        {
            ListMonad<B> result = new ListMonad<B>();

            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict)
                    result.Add(function(element.Value));
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<B> Fmap<B>(Func<CacheEntry<K, V>, int, B> function)
        {
            ListMonad<B> result = new ListMonad<B>();
            int index = 0;
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict)
                    result.Add(function(element.Value, index++));
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<CacheEntry<K, V>> Pure(CacheEntry<K, V> parameter)
        {
            Lock.EnterWriteLock();
            try
            {

                cacheDict.Clear();
                cacheDict[parameter.Key] = parameter;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return this;
        }

        public override Monad<B> Bind<B>(Func<CacheEntry<K, V>, Monad<B>> func)
        {
            ListMonad<B> result = new ListMonad<B>();
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                    foreach (var fRes in func(element))
                        result.Add(fRes);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<B> Bind<B>(Func<CacheEntry<K, V>, int, Monad<B>> func)
        {
            ListMonad<B> result = new ListMonad<B>();
            int index = 0;
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                    foreach (var fRes in func(element, index++))
                        result.Add(fRes);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Func<CacheEntry<K, V>, Monad<C>> Kleisli<B, C>(Func<CacheEntry<K, V>, Monad<B>> fAtB, Func<B, Monad<C>> fBtC)
        {
            return (a) =>
            {
                Monad<B> result = null;
                Lock.EnterWriteLock();
                try
                {

                    foreach (var element in cacheDict.Values)
                        if (result == null)
                            result = fAtB(element);
                        else
                            result.Concatenate(fAtB(element));
                }
                finally
                {
                    Lock.ExitWriteLock();
                }
                return result.Bind(fBtC);
            };
        }

        public override CacheEntry<K, V> Return()
        {
            CacheEntry<K, V> result = null;
            Lock.EnterReadLock();
            try
            {
                int count = cacheDict.Values.Count;
                result = count > 0 ? cacheDict.Values.ElementAt(count) : null;
            }
            finally
            {
                Lock.ExitReadLock();
            }
            return result;
        }

        public override Monad<B> App<B>(Monad<Func<CacheEntry<K, V>, B>> functionMonad)
        {
            ListMonad<B> result = new ListMonad<B>();
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                    foreach(var func in functionMonad)
                        result.Add(func(element));
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<B> App<B>(Monad<Func<CacheEntry<K, V>, Monad<B>>> functionMonad)
        {
            Monad<B> result = null;
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                {
                    foreach (var func in functionMonad)
                    {
                        if (result == null)
                            result = func(element);
                        else
                            result = result.Concatenate(func(element));
                    }
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<C> Com<B, C>(Func<CacheEntry<K, V>, B, C> function, Monad<B> mOther)
        {
            ListMonad<C> result = new ListMonad<C>();
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                    foreach (var elementB in mOther)
                        result.Add(function(element, elementB));
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<C> Com<B, C>(Func<CacheEntry<K, V>, B, Monad<C>> function, Monad<B> mOther)
        {
            Monad<C> result = null;
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                {
                    foreach (var elementB in mOther)
                    {
                        if (result == null)
                            result = function(element, elementB);
                        else
                            result = result.Concatenate(function(element, elementB));
                    }
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<C> Com<B, C>(Monad<Func<CacheEntry<K, V>, B, C>> functionMonad, Monad<B> mOther)
        {
            ListMonad<C> result = new ListMonad<C>();
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                    foreach(var func in functionMonad)
                        foreach (var elementB in mOther)
                            result.Add(func(element, elementB));
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<C> Com<B, C>(Monad<Func<CacheEntry<K, V>, B, Monad<C>>> functionMonad, Monad<B> mOther)
        {
            Monad<C> result = null;
            Lock.EnterWriteLock();
            try
            {

                foreach (var element in cacheDict.Values)
                {
                    foreach (var func in functionMonad)
                    {
                        foreach (var elementB in mOther)
                        {
                            if (result == null)
                                result = func(element, elementB);
                            else
                                result = result.Concatenate(func(element, elementB));
                        }
                    }
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return result;
        }

        public override Monad<CacheEntry<K, V>> Visit(Action<CacheEntry<K, V>> function)
        {
            Lock.EnterWriteLock();
            try
            {
                foreach (var entry in cacheDict.Values)
                    function(entry);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return this;
        }

        public override Monad<CacheEntry<K, V>> Visit<B>(Action<CacheEntry<K, V>, B> action, Monad<B> mOther)
        {
            Lock.EnterWriteLock();
            try
            {
                foreach (var entry in cacheDict.Values)
                    foreach(var elementB in mOther)
                        action(entry, elementB);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
            return this;
        }

        public override Monad<CacheEntry<K, V>> Concatenate(Monad<CacheEntry<K, V>> otherMonad)
        {
            Lock.EnterWriteLock();
            try
            {
                foreach (var entry in otherMonad)
                    cacheDict[entry.Key] = entry;
            }
            finally
            {
                Lock.ExitWriteLock();
            }        
            return this;
        }

        public override IEnumerator<CacheEntry<K, V>> GetEnumerator()
        {
            IEnumerator<CacheEntry<K, V>> result = null;
            Lock.EnterReadLock();
            try
            {
                result = cacheDict.Values.GetEnumerator();
            }
            finally
            {
                Lock.ExitReadLock();
            } 
            return result;
        }

        #endregion 

    }
}
