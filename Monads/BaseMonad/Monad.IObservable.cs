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
using System.Threading.Tasks;

namespace Monads
{

    public class NexValueUnknownException : Exception
    {
        internal NexValueUnknownException()
        { }
    }

    public abstract partial class Monad<A> : IObservable<A>
    {
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
                tasks[index] = Task.Run(() =>
                {
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
    }
}
