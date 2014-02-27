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

namespace Monads
{
    public abstract partial class Monad<A> : IObserver<A>
    {
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
                                        Action<Monad<A>, A> nextAction = null,
                                        Action<Monad<A>> completeAction = null,
                                        Action<Monad<A>, Exception> errorAction = null)
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
        public Action<Monad<A>> CompleteAction
        {
            get;
            set;
        }

        /// <summary>
        /// Is called inside OnError().
        /// </summary>
        public Action<Monad<A>, Exception> ErrorAction
        {
            get;
            set;
        }

        /// <summary>
        /// Is called inside OnNext().
        /// </summary>
        public Action<Monad<A>, A> NextAction
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
            if (unsubscriber != null)
                unsubscriber.Dispose();
        }

        #endregion
    }
}
