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

namespace Monads
{
    public abstract partial class Monad<A>
    {
        #region Linq_Enumerable_Connection

        public virtual Monad<A> Where(Func<A, bool> predicate)
        {
            Monad<A> result = (Monad<A>)this.GetType().GetConstructor(new Type[] { }).Invoke(null);
            foreach (A element in this)
                if (predicate(element))
                    result.Append(element);
            return result;
        }

        public virtual Monad<A> Where(Func<A, int, bool> predicate)
        {
            Monad<A> result = (Monad<A>)this.GetType().GetConstructor(new Type[] { }).Invoke(null);
            int index = 0;
            foreach (A element in this)
            {
                if (predicate(element, index))
                    result.Append(element);
                index++;
            }
            return result;
        }

        public virtual Monad<B> Select<B>(Func<A, B> f)
        {
            return Fmap(f);
        }

        public virtual Monad<B> Select<B>(Func<A, Int32, B> f)
        {
            return Fmap(f);
        }

        public virtual Monad<B> SelectMany<B>(Func<A, Monad<B>> f)
        {
            return Bind(f);
        }

        public virtual Monad<B> SelectMany<B>(Func<A, Int32, Monad<B>> f)
        {
            return Bind(f);
        }

        public virtual Monad<B> SelectMany<TMonad, B>(Func<A, Monad<TMonad>> selector, Func<A, TMonad, B> function)
        {
            return Com(function, Bind(selector));
        }

        public virtual Monad<B> SelectMany<TMonad, B>(Func<A, Int32, Monad<TMonad>> selector, Func<A, TMonad, B> function)
        {
            return Com(function, Bind(selector));
        }

        #endregion

        public abstract IEnumerator<A> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SingleEnumerator<A>(Return());
        }
    }
}
