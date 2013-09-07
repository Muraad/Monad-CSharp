
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

namespace FunctionalProgramming
{
    public interface IMonad<A> : IEnumerable<A>
    {

        #region IMonad_Core_Interface_Function_Definitions

        // Haskell fmap from Monad, maps a function over the value inside this monad.
        IMonad<B> Fmap<B>(Func<A, B> function);

        // Haskell pure from Monad. Puts a given value in the minimal context of this monad.
        IMonad<A> Pure(A parameter);

        // Haskell return from Monad. Returns the value inside this monad.
        A Return();

        IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad);

        // Haskell applicative function (operator) from Applicative. 
        // In Haskell its for a ApplicativeFunctor, but it is the same that a monad. The only established the ApplicativeFunctor later.
        IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad);

        // Combination
        IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther);
        IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther);
        IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther);
        IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther);

        // Usefull helper function. Do a action on every element in this monad, and return this at the end.
        IMonad<A> Visit(Action<A> function);
        IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther);

        IMonad<A> Concat(IMonad<A> otherMonad);

        #endregion

      
        #region Linq_Enumerable_Connection

        IMonad<A> Where(Func<A, bool> predicate);   // filter.
        IMonad<A> Where(Func<A, int, bool> predicate);

        IMonad<B> Select<B>(Func<A, B> f);       // fmap
        IMonad<B> Select<B>(Func<A, Int32, B> f);   // fmap with index.

        IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f);
        IMonad<B> SelectMany<B>(Func<A, Int32, IMonad<B>> f);
        IMonad<B> SelectMany<TMonad, B>(Func<A, IMonad<TMonad>> selector, 
                                        Func<A, TMonad, B> function);
        IMonad<B> SelectMany<TMonad, B>(Func<A, Int32, IMonad<TMonad>> selector, 
                                        Func<A, TMonad, B> function);

        #endregion
    }

    public abstract class AMonadDecorator<A> : IMonad<A>
    {
        #region IMonad_Core_Interface_Function_Definitions

        /*
        #region Operator_overloading

        // applicate with multiplicate operator.
        public static IMonad<A> operator *(AMonadDecorator<A> firstM, IMonad<Func<A, IMonad<A>>> functionMonad)
        {
            return firstM.App(functionMonad);
        }

        public static IMonad<A> operator *(AMonadDecorator<A> firstM, IMonad<Func<A, A>> functionMonad)
        {
            return firstM.App(functionMonad);
        }

        // Combinate with multiplicate operator.
        public static IMonad<A> operator *(AMonadDecorator<A> firstM, Tuple<IMonad<Func<A, A, A>>, IMonad<A>> tupel)
        {
            return firstM.Com<A, A>(tupel.Item1, tupel.Item2);
        }

        public static IMonad<A> operator *(AMonadDecorator<A> firstM, Tuple<IMonad<Func<A, A, IMonad<A>>>, IMonad<A>> tupel)
        {
            return firstM.Com<A, A>(tupel.Item1, tupel.Item2);
        }

        public static IMonad<A> operator /(AMonadDecorator<A> firstM, Func<A, A> functionMonad)
        {
            return firstM.Fmap(functionMonad);
        }

        public static IMonad<A> operator +(AMonadDecorator<A> firstM, ListMonad<A> otherMonad)
        {
            return firstM.Concat(otherMonad);
        }

        #endregion
        */
        // Haskell fmap from Monad, maps a function over the value inside this monad.
        public abstract IMonad<B> Fmap<B>(Func<A, B> function);

        // Haskell pure from Monad. Puts a given value in the minimal context of this monad.
        public abstract IMonad<A> Pure(A parameter);

        // Haskell return from Monad. Returns the value inside this monad.
        public A Return()
        {
            return default(A);
        }

        public abstract IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad);

        // Haskell applicative function (operator) from Applicative. 
        // In Haskell its for a ApplicativeFunctor, but it is the same that a monad. The only established the ApplicativeFunctor later.
        public abstract IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad);

        // Combination
        public abstract IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther);
        public abstract IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther);

        // Usefull helper function. Do a action on every element in this monad, and return this at the end.
        public abstract IMonad<A> Visit(Action<A> function);
        public abstract IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther);

        // Zip two values inside of this monad and another monad to a third result value with a given function. And pack this result value inside a new monad and return it.
        // In Haskell ZipWith is only for lists.
        public abstract IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther);

        // same as ZipWith only that the function returns a IMonad itselfs.
        public abstract IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther);

        public abstract IMonad<A> Concat(IMonad<A> otherMonad);

        #endregion

        #region Linq_Enumerable_Connection

        public abstract IMonad<B> Fmap<B>(Func<A, IMonad<B>> f);     // 

        public abstract IMonad<A> Where(Func<A, bool> predicate);   // filter.
        public abstract IMonad<A> Where(Func<A, int, bool> predicate);

        public abstract IMonad<B> Select<B>(Func<A, B> f);       // fmap
        public abstract IMonad<B> Select<B>(Func<A, Int32, B> f);   // fmap with index.

        public abstract IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f);
        public abstract IMonad<B> SelectMany<B>(Func<A, Int32, IMonad<B>> f);
        public abstract IMonad<B> SelectMany<TMonad, B>(Func<A, IMonad<TMonad>> selector, Func<A, TMonad, B> function);
        public abstract IMonad<B> SelectMany<TMonad, B>(Func<A, Int32, IMonad<TMonad>> selector, Func<A, TMonad, B> function);

        #endregion

        public IEnumerator<A> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
