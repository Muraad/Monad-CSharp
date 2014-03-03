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

namespace Monads.Extension.AtomicExtensions
{
    public static class MonadAtomicExtensions
    {
        public static Monad<T> Exchange<T>(this Monad<T> thisMonad, Monad<T> monad)
        {
            monad.Lock.EnterReadLock();
            try
            {
                thisMonad.Write(monad.Return());
            }
            finally
            {
                monad.Lock.ExitReadLock();
            }
            return thisMonad;

        }

        public static bool CompareExchange<T>(this Monad<T> thisMonad, T value, Func<T, bool> comparand)
        {
            bool result = false;
            thisMonad.Lock.EnterWriteLock();
            try
            {
                if ((result = comparand(thisMonad.Return())))
                    thisMonad.Append(value);
            }
            finally
            {
                thisMonad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static bool CompareExchange<T>(this Monad<T> thisMonad, T value, Func<bool> comparand)
        {
            bool result = false;
            thisMonad.Lock.EnterWriteLock();
            try
            {
                if ((result = comparand()))
                    thisMonad.Append(value);
            }
            finally
            {
                thisMonad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static bool CompareExchange<T>(this Monad<T> thisMonad, Monad<T> monad, Func<T, bool> comparand)
        {
            bool result = false;
            thisMonad.Lock.EnterWriteLock();
            monad.Lock.EnterReadLock();
            try
            {
                if ((result = comparand(thisMonad.Return())))
                    foreach (var element in monad)
                        thisMonad.Append(element);
            }
            finally
            {
                monad.Lock.ExitReadLock();
                thisMonad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static bool CompareExchange<T>(this Monad<T> thisMonad, Monad<T> monad, Func<bool> comparand)
        {
            bool result = false;
            thisMonad.Lock.EnterWriteLock();
            monad.Lock.EnterReadLock();
            try
            {
                if ((result = comparand()))
                {
                    foreach(var element in monad)
                        thisMonad.Append(monad.Return());
                }
            }
            finally
            {
                monad.Lock.ExitReadLock();
                thisMonad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static T Read<T>(this Monad<T> id)
        {
            T result = default(T);
            id.Lock.EnterReadLock();
            try
            {
                result = id.Return() ;
            }
            finally
            {
                id.Lock.ExitWriteLock();
            }
            return result;
        }

        public static void Write<T>(this Monad<T> id, T value)
        {
            id.Lock.EnterWriteLock();
            try
            {
                id.Append(value);
            }
            finally
            {
                id.Lock.ExitWriteLock();
            }
        }
    }
}
