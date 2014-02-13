﻿
/*
 *  Copyright (C) 2013  Muraad Nofal

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
using System.Reflection;
using System.Linq.Expressions;

namespace FunctionalProgramming
{

    public static partial class Extensions
    {
        public static bool Contains<A, D>(this IMonad<A> monad, D dest, Func<A, D, bool> comparer)
        {
            bool result = false;
            foreach (A element in monad)
            {
                if (comparer(element, dest))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static A GetItemBy<A, D>(this IMonad<A> monad, D dest, Func<A, D, bool> comparer)
        {
            A result = default(A);
            foreach (A element in monad)
            {
                if (comparer(element, dest))
                {
                    result = element;
                    break;
                }
            }
            return result;
        }

        public static int IndexOfBy<A, D>(this IMonad<A> monad, D dest, Func<A, D, bool> comparer)
        {
            int result = -1;
            int index = 0;
            foreach (A element in monad)
            {
                if (comparer(element, dest))
                {
                    result = index;
                    break;
                }
                index++;
            }
            return result;
        }

        public static bool Any<A, D>(this IMonad<A> monad, IMonad<D> destMonad, Func<A, D, bool> comparer)
        {
            bool result = false;
            foreach (A element in monad)
            {
                foreach (D value in destMonad)
                {
                    if (comparer(element, value))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public static bool All<A, D>(this IMonad<A> monad, IMonad<D> destMonad, Func<A, D, bool> comparer)
        {
            bool result = false;
            foreach (A element in monad)
            {
                foreach (D value in destMonad)
                {
                    if (!comparer(element, value))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        public static IMonad<A> Except<A, D>(this IMonad<A> monad, IMonad<D> destMonad, Func<A, D, bool> comparer)
        {
            Type[] arg = new Type[] { };
            IMonad<A> result = (IMonad<A>) monad.GetType().GetConstructor(arg).Invoke(null);
            foreach (A element in monad)
            {
                foreach (D value in destMonad)
                {
                    if (!comparer(element, value))
                    {
                        result.Append(element);
                        break;
                    }
                }
            }
            return result;
        }

        public static IMonad<A> Match<A, D>(this IMonad<A> monad, IMonad<D> destMonad, Func<A, D, bool> comparer)
        {
            Type[] arg = new Type[] { };
            IMonad<A> result = (IMonad<A>)monad.GetType().GetConstructor(arg).Invoke(null);
            foreach (A element in monad)
            {
                foreach (D value in destMonad)
                {
                    if (comparer(element, value))
                    {
                        result.Append(element);
                        break;
                    }
                }
            }
            return result;
        }


        public static Func<IMonad<A>> LiftM<A>(this IMonad<A> monad, Func<A> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[]{});
            IMonad<A> m = (IMonad<A>)constructor.Invoke(null);
            return () =>
            {
                return m.Pure(func());
            };
        }

        public static Func<IMonad<A>, IMonad<B>> LiftM<A, B>(this IMonad<B> monad, Func<A, B> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            IMonad<B> m = (IMonad<B>)constructor.Invoke(null);
            return (a) =>
            {
                return m.Pure(func(a.Return()));
            };
        }

        public static Func<IMonad<A>, IMonad<B>, IMonad<C>> LiftM<A, B, C>(this IMonad<C> monad, Func<A, B, C> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            IMonad<C> m = (IMonad<C>)constructor.Invoke(null);
            return (a, b) =>
            {
                return m.Pure(func(a.Return(), b.Return()));
            };
        }

        public static Func<IMonad<A>, IMonad<B>, IMonad<C>, IMonad<D>> LiftM<A, B, C, D>(this IMonad<D> monad, Func<A, B, C, D> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            IMonad<D> m = (IMonad<D>)constructor.Invoke(null);
            return (a, b, c) =>
            {
                return m.Pure(func(a.Return(), b.Return(), c.Return()));
            };
        }

        public static Func<IMonad<A>, IMonad<B>, IMonad<C>, IMonad<D>, IMonad<E>> LiftM<A, B, C, D, E>(this IMonad<E> monad, Func<A, B, C, D, E> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            IMonad<E> m = (IMonad<E>)constructor.Invoke(null);
            return (a, b, c, d) =>
            {
                return m.Pure(func(a.Return(), b.Return(), c.Return(), d.Return()));
            };
        }



        public static Func<IMonad<A>, IMonad<A>> LiftM<A>(this IMonad<A> monad, Func<A, A> func)
        {
            return LiftM<A, A>(monad, func);
        }

        public static Func<IMonad<A>, IMonad<A>, IMonad<A>> LiftM<A>(this IMonad<A> monad, Func<A, A, A> func)
        {
            return LiftM<A, A, A>(monad, func);
        }

        public static Func<IMonad<A>, IMonad<A>, IMonad<A>, IMonad<A>> LiftM<A>(this IMonad<A> monad, Func<A, A, A, A> func)
        {
            return LiftM<A, A, A, A>(monad, func);
        }

        public static Func<IMonad<A>, IMonad<A>, IMonad<A>, IMonad<A>, IMonad<A>> LiftM<A>(this IMonad<A> monad, Func<A, A, A, A, A> func)
        {
            return LiftM<A, A, A, A, A>(monad, func);
        }

        #region Do_Write_Or_Read_Action_On_Model_Safe

        public static IMonad<A> ActionW<A>(this IMonad<A> monad, Action expr, bool updateNext = true)
        {
            monad.Lock.EnterWriteLock();

            try
            {
                expr();
                if (updateNext)
                    monad.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return monad;
        }

        public static IMonad<A> ActionW<A>(this IMonad<A> monad, Action<IMonad<A>> expr, bool updateNext = true)
        {
            monad.Lock.EnterWriteLock();

            try
            {
                expr(monad);
                if (updateNext)
                    monad.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return monad;
        }

        public static IMonad<A> ActionR<A>(this IMonad<A> monad, Action<A> action)
        {
            monad.Lock.EnterReadLock();
            try
            {
                monad.Visit(action);
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitReadLock();
            }
            return monad;
        }

        #endregion

        #region Call_Method_From_Model_By_Name_Write_And_Read_Safe

        public static IMonad<A> MethodW<A>(this IMonad<A> monad, Func<IMonad<A>> expr, bool updateNext = true)
        {
            monad.Concatenate(monad.MethodW2<A, A>(expr, false));
            if (updateNext)
                monad.UpdateValue();
            return monad;
        }

        public static IMonad<B> MethodW2<A, B>(this IMonad<A> monad, Func<IMonad<B>> expr, bool updateNext = true)
        {
            IMonad<B> result = null;
            monad.Lock.EnterWriteLock();
            try
            {
                result = expr();
                if (updateNext)
                    result.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static IMonad<A> MethodW<A>(this IMonad<A> monad, Func<A, IMonad<A>> expr, bool updateNext = true)
        {

            monad.Concatenate(monad.MethodW2<A, A>(expr, false));
            if (updateNext)
                monad.UpdateValue();
            return monad;
        }
        public static IMonad<B> MethodW2<A, B>(this IMonad<A> monad, Func<A, IMonad<B>> expr, bool updateNext = true)
        {
            IMonad<B> result = null;
            monad.Lock.EnterWriteLock();
            try
            {
                result = expr(monad.Return());
                if (updateNext)
                    result.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static IMonad<A> MethodW<A>(this IMonad<A> monad, Func<IMonad<A>, IMonad<A>> expr, bool updateNext = true)
        {
            monad.Concatenate(monad.MethodW2<A, A>(expr, false));
            if (updateNext)
                monad.UpdateValue();
            return monad;
        }

        public static IMonad<B> MethodW2<A, B>(this IMonad<A> monad, Func<IMonad<A>, IMonad<B>> expr, bool updateNext = true)
        {
            IMonad<B> result = null;
            monad.Lock.EnterWriteLock();
            try
            {
                result = expr(monad);
                if (updateNext)
                    result.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return result;
        }

        #endregion
    }

    public abstract class IMonad<A> : IEnumerable<A>, IObservable<A>, IObserver<A>
    {

        

        #region IMonad_Core_Interface_Function_Definitions

        private System.Threading.ReaderWriterLockSlim rwLock = new System.Threading.ReaderWriterLockSlim();
        public System.Threading.ReaderWriterLockSlim Lock
        {
            get { return rwLock; }
        }

        public IMonad<A> Set(IMonad<A> other)
        {
            return Pure(other.Return());
        }

        /// <summary>
        /// Simpler Fmap that takes a function thats return type is the same as the argument type.
        /// Then it uses Concatenate to add the value(s) inside the result monad to this monad.
        /// </summary>
        /// <param name="function">The function to apply.</param>
        /// <returns>The calling monad instance (this).</returns>
        public abstract IMonad<A> Fmap(Func<A, A> function);

        public abstract IMonad<A> Fmap(Func<A, int, A> function);

        /// <summary>
        /// Applys a function to the value(s) inside this monad. 
        /// The results are packed into a new monad of the same type than the calling monad.
        /// But with a generic type of the function result type.
        /// </summary>
        /// <typeparam name="B">The type of the function result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The new monad.</returns>
        public abstract IMonad<B> Fmap<B>(Func<A, B> function);

        /// <summary>
        /// Applys a function to the value(s) inside this monad. 
        /// The results are packed into a new monad of the same type than the calling monad.
        /// But with a generic type of the function result type.
        /// </summary>
        /// <typeparam name="B">The type of the function result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The new monad.</returns>
        public abstract IMonad<B> Fmap<B>(Func<A, int, B> function);


        /// <summary>
        /// Puts the given value inside of this monad. It does not return a new monad.
        /// Its like a setter. Returning a new monad makes no sense in C#, because most times
        /// the value inside the monad will be a reference type.
        /// Monads like a list should clear their values before adding the new given value.
        /// </summary>
        /// <param name="parameter">The new value inside of this monad.</param>
        /// <returns>this.</returns>
        public abstract IMonad<A> Pure(A parameter);

        /// <summary>
        /// The same as fmap only that the function itself is returning the new monad.
        /// </summary>
        /// <typeparam name="B">The type of the value inside the returned monad.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The new monad.</returns>
        public abstract IMonad<B> Bind<B>(Func<A, IMonad<B>> func);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public abstract IMonad<B> Bind<B>(Func<A, int, IMonad<B>> func);

        /// <summary>
        /// The Kleisli-Operator for monads.
        /// It transforms a function from A to monad B and a function from B to monad C
        /// into a function from A to monad C directly.
        /// Basically it does function composition.
        /// </summary>
        /// <typeparam name="B">Type.</typeparam>
        /// <typeparam name="C">Type.</typeparam>
        /// <param name="fAtB">Function from A to monad B.</param>
        /// <param name="fBtC">Function from B to monad C</param>
        /// <returns>Function from A to monad C.</returns>
        public abstract Func<A, IMonad<C>> Kleisli<B, C>(Func<A, IMonad<B>> fAtB, Func<B, IMonad<C>> fBtC);

        /// <summary>
        /// Get the value inside this monad.
        /// </summary>
        /// <returns>The value.</returns>
        public abstract A Return();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="functionMonad"></param>
        /// <returns></returns>
        public abstract IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad);

        // Haskell applicative function (operator) from Applicative. 
        // In Haskell its from a ApplicativeFunctor, but it is the same that a monad. The only established the ApplicativeFunctor later.
        public abstract IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad);

        // Combination
        public abstract IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther);
        public abstract IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther);
        public abstract IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther);
        public abstract IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther);

        // Usefull helper function. Do a action on every element in this monad, and return this at the end.
        public abstract IMonad<A> Visit(Action<A> function);
        public abstract IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther);

        public abstract IMonad<A> Append(A value);

        public abstract IMonad<A> Concatenate(IMonad<A> otherMonad);

        #endregion
      
        
        #region Linq_Enumerable_Connection

        public virtual IMonad<A> Where(Func<A, bool> predicate)
        {
            IMonad<A> result = (IMonad<A>)this.GetType().GetConstructor(new Type[] { }).Invoke(null);
            foreach(A element in this)
                if(predicate(element))
                    result.Append(element);
            return result;
        }

        public virtual IMonad<A> Where(Func<A, int, bool> predicate)
        {
            IMonad<A> result = (IMonad<A>)this.GetType().GetConstructor(new Type[] { }).Invoke(null);
            int index = 0;
            foreach (A element in this)
            {
                if (predicate(element, index))
                    result.Append(element);
                index++;
            }
            return result;
        }

        public virtual IMonad<B> Select<B>(Func<A, B> f)
        {
            return Fmap(f);
        }

        public virtual IMonad<B> Select<B>(Func<A, Int32, B> f)
        {
            return Fmap(f);
        }

        public virtual IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f)
        {
            return Bind(f);
        }

        public virtual IMonad<B> SelectMany<B>(Func<A, Int32, IMonad<B>> f)
        {
            return Bind(f);
        }

        public virtual IMonad<B> SelectMany<TMonad, B>(Func<A, IMonad<TMonad>> selector, Func<A, TMonad, B> function)
        {
            return Com(function, Bind(selector));
        }

        public virtual IMonad<B> SelectMany<TMonad, B>(Func<A, Int32, IMonad<TMonad>> selector, Func<A, TMonad, B> function)
        {
            return Com(function, Bind(selector));
        }
        
        #endregion

        public abstract IEnumerator<A> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SingleEnumerator<A>(Return());
        }
        
        #region IObservable implementation

        private List<IObserver<A>> observers = new List<IObserver<A>>();

        public IDisposable Subscribe(IObserver<A> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<A>> _observers;
            private IObserver<A> _observer;

            public Unsubscriber(List<IObserver<A>> observers, IObserver<A> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void UpdateValue(Exception exc = null)
        {
            foreach (var observer in observers)
            {
                if (exc != null)
                    observer.OnError(new NexValueUnknownException());
                else
                    observer.OnNext(this.Return());
            }
        }

        public void UpdateValue(A next, Exception exc = null)
        {
            this.Append(next);
            foreach (var observer in observers)
            {
                if (exc != null)
                    observer.OnError(new NexValueUnknownException());
                else
                    observer.OnNext(next);
            }
        }

        public async Task UpdateValueAsync(Exception exc = null)
        {
            Task[] tasks = new Task[observers.Count];
            int index = 0;
            foreach (var observer in observers)
            {
                tasks[index] = Task.Run(() =>
                {
                    if (exc != null)
                        observer.OnError(new NexValueUnknownException());
                    else
                        observer.OnNext(this.Return());
                });
                index++;
            }
            await Task.WhenAll(tasks);
        }

        public async Task UpdateValueAsync(A next, Exception exc = null)
        {
            Task[] tasks = new Task[observers.Count];
            int index = 0;
            foreach (var observer in observers)
            {
                tasks[index] = Task.Run(() => {
                                                if (exc != null)
                                                    observer.OnError(new NexValueUnknownException());
                                                else
                                                    observer.OnNext(next);
                                              });
                index++;
            }
            await Task.WhenAll(tasks);
        }

        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }

        #endregion

        #region IObserver implementation

        private IDisposable unsubscriber = null;

        /// <summary>
        /// The IObservalbe subscribe will return an IDisposable.
        /// Set this after subscription.
        /// Use like: observer.Disposable = observable.Subscribe(observer);
        /// </summary>
        public IDisposable Disposable 
        {
            get
            {
                return unsubscriber;
            }
            set
            {
                unsubscriber = value;
            }
        }

        /// <summary>
        /// Subscribe at a given observable.
        /// Provider Subscribe() is called. Disposable is set from return value.
        /// </summary>
        /// <param name="provider">The IObservable provider we want to subscribe for changes.</param>
        /// <param name="nextAction">The action that is called when there is a new value from the observable.</param>
        /// <param name="completeAction">The action called on complete.</param>
        /// <param name="errorAction">The action that is called if there was an exception at the observer.</param>
        public virtual void SubscribeAt(IObservable<A> provider, 
                                        Action<IMonad<A>, A> nextAction = null, 
                                        Action<IMonad<A>> completeAction = null,
                                        Action<IMonad<A>, Exception> errorAction = null)
        {
            if (nextAction != null)
                NextAction = nextAction;

            if (completeAction != null)
                CompleteAction = completeAction;

            if (errorAction != null)
                ErrorAction = errorAction;

            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }


        /// <summary>
        /// Is called inside OnComplete().
        /// </summary>
        public Action<IMonad<A>> CompleteAction
        {
            get;
            set;
        }

        /// <summary>
        /// Is called inside OnError().
        /// </summary>
        public Action<IMonad<A>, Exception> ErrorAction
        {
            get;
            set;
        }

        /// <summary>
        /// Is called inside OnNext().
        /// </summary>
        public Action<IMonad<A>, A> NextAction
        {
            get;
            set;
        }

        public void OnCompleted()
        {
            if (CompleteAction != null)
                CompleteAction(this);
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            if (ErrorAction != null)
                ErrorAction(this, error);
        }

        public void OnNext(A value)
        {
            if (NextAction != null)
                NextAction(this, value);
        }

        public virtual void Unsubscribe()
        {
            if(unsubscriber != null)
                unsubscriber.Dispose();
        }

        #endregion
    }

    public class NexValueUnknownException : Exception
    {
        internal NexValueUnknownException()
        { }
    }
}
