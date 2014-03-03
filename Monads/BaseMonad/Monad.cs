
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

namespace Monads
{
    public abstract partial class Monad<A> : IEnumerable<A>
    {
        #region IMonad_Core_Interface_Function_Definitions

        private System.Threading.ReaderWriterLockSlim rwLock = new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);
        public System.Threading.ReaderWriterLockSlim Lock
        {
            get { return rwLock; }
        }

        public Monad<A> Set(Monad<A> other)
        {
            return Pure(other.Return());
        }

        /// <summary>
        /// Simpler Fmap that takes a function thats return type is the same as the argument type.
        /// Then it uses Concatenate to add the value(s) inside the result monad to this monad.
        /// </summary>
        /// <param name="function">The function to apply.</param>
        /// <returns>The calling monad instance (this).</returns>
        public abstract Monad<A> Fmap(Func<A, A> function);

        public abstract Monad<A> Fmap(Func<A, int, A> function);

        /// <summary>
        /// Applys a function to the value(s) inside this monad. 
        /// The results are packed into a new monad of the same type than the calling monad.
        /// But with a generic type of the function result type.
        /// </summary>
        /// <typeparam name="B">The type of the function result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The new monad.</returns>
        public abstract Monad<B> Fmap<B>(Func<A, B> function);

        /// <summary>
        /// Applys a function to the value(s) inside this monad. 
        /// The results are packed into a new monad of the same type than the calling monad.
        /// But with a generic type of the function result type.
        /// </summary>
        /// <typeparam name="B">The type of the function result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>The new monad.</returns>
        public abstract Monad<B> Fmap<B>(Func<A, int, B> function);


        /// <summary>
        /// Puts the given value inside of this monad. It does not return a new monad.
        /// Its like a setter. Returning a new monad makes no sense in C#, because most times
        /// the value inside the monad will be a reference type.
        /// Monads like a list should clear their values before adding the new given value.
        /// </summary>
        /// <param name="parameter">The new value inside of this monad.</param>
        /// <returns>this.</returns>
        public abstract Monad<A> Pure(A parameter);

        /// <summary>
        /// The same as fmap only that the function itself is returning the new monad.
        /// </summary>
        /// <typeparam name="B">The type of the value inside the returned monad.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The new monad.</returns>
        public abstract Monad<B> Bind<B>(Func<A, Monad<B>> func);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public abstract Monad<B> Bind<B>(Func<A, int, Monad<B>> func);

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
        public abstract Func<A, Monad<C>> Kleisli<B, C>(Func<A, Monad<B>> fAtB, Func<B, Monad<C>> fBtC);

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
        public abstract Monad<B> App<B>(Monad<Func<A, B>> functionMonad);

        // Haskell applicative function (operator) from Applicative. 
        // In Haskell its from a ApplicativeFunctor, but it is the same that a monad. The only established the ApplicativeFunctor later.
        public abstract Monad<B> App<B>(Monad<Func<A, Monad<B>>> functionMonad);

        // Combination
        public abstract Monad<C> Com<B, C>(Func<A, B, C> function, Monad<B> mOther);
        public abstract Monad<C> Com<B, C>(Func<A, B, Monad<C>> function, Monad<B> mOther);
        public abstract Monad<C> Com<B, C>(Monad<Func<A, B, C>> functionMonad, Monad<B> mOther);
        public abstract Monad<C> Com<B, C>(Monad<Func<A, B, Monad<C>>> functionMonad, Monad<B> mOther);

        // Usefull helper function. Do a action on every element in this monad, and return this at the end.
        public abstract Monad<A> Visit(Action<A> function);
        public abstract Monad<A> Visit<B>(Action<A, B> action, Monad<B> mOther);

        public abstract Monad<A> Append(A value);

        public abstract Monad<A> Concatenate(Monad<A> otherMonad);

        #endregion 

    }

}
