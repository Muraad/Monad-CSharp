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

namespace Monads
{
    public static partial class ToMonadExtensions
    {
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return new Just<T>(value);
        }

        public static Maybe<T> ToMaybe<T>(this Monad<T> value)
        {
            return value.Return();
        }

    }

    public class Just<T> : Maybe<T>
    {
        public Just(T jValue)
            : base(jValue)
        {
        }
    }

    public class Nothing<T> : Maybe<T>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public abstract class Maybe<A> : Monad<A>
    {
        public static implicit operator A(Maybe<A> instance)
        {
            return instance.Return();
        }

        public static implicit operator Maybe<A>(A value)
        {
            if (value == null || value.Equals(default(A)))
                return new Nothing<A>();
            else
                return new Just<A>(value);
        }

        #region Maybe_Class_Implementation

        private A aValue;

        public override string ToString()
        {
            string result = "";
            if (isNothing)
                result = "N<" + Return().GetType().Name + ">(" + Return().ToString() + ")";
            else
                result = "J<" + Return().GetType().Name + ">(" + Return().ToString() + ")";
            return result;
        }

        protected Maybe()
        {
            aValue = default(A);        // Nothing/Default constructor creates default(A) as value.
        }

        protected Maybe(A value)
        {
            aValue = value;
        }

        public A Value()
        {
            return aValue;
        }

        public bool isNothing
        {
            get
            {
                return this is Nothing<A>;
            }
        }

        #endregion

        #region IMonad_Implementation


        public override Monad<A> Fmap(Func<A, A> function)
        {
            if (this is Just<A> && function != null)
                aValue = function(aValue);
            return this;
        }

        public override Monad<A> Fmap(Func<A, int, A> function)
        {
            if (this is Just<A> && function != null)
                aValue = function(aValue, 0);        // implicit operator makes it automatically a Just or a Nothing depending on the function result.
            return this;
        }

        public override Monad<B> Fmap<B>(Func<A, B> function)
        {
            Maybe<B> resMaybe = new Nothing<B>();
            if (this is Just<A> && function != null)
                resMaybe = function(aValue);        // implicit operator makes it automatically a Just or a Nothing depending on the function result.
            if (resMaybe == null)
                resMaybe = new Nothing<B>();
            return resMaybe;
        }

        public override Monad<B> Fmap<B>(Func<A, int, B> function)
        {
            Maybe<B> resMaybe = new Nothing<B>();
            if (this is Just<A> && function != null)
                resMaybe = function(aValue, 0);        // implicit operator makes it automatically a Just or a Nothing depending on the function result.
            if (resMaybe == null)
                resMaybe = new Nothing<B>();
            return resMaybe;
        }

        public override Monad<A> Pure(A parameter)
        {
            aValue = parameter;        // Use implicit operator to make Just or Nothing 
            return this;
        }

        public override A Return()
        {
            return aValue;
        }

        public override Monad<B> App<B>(Monad<Func<A, B>> functionMonad)
        {
            Maybe<B> result = new Nothing<B>();
            if (this is Just<A> && functionMonad != null)
            {
                foreach (var function in functionMonad)
                {
                    if (function != null)
                        result = new Just<B>(functionMonad.Return()(aValue));
                }
                if (result == null)
                    result = new Nothing<B>();
            }
            return result;
        }

        public override Monad<B> App<B>(Monad<Func<A, Monad<B>>> functionMonad)
        {
            Monad<B> result = new Nothing<B>();
            if (this is Just<A> && functionMonad != null)
            {
                result = null;
                //result = functionMonad.Return()(aValue).Return();
                foreach (var function in functionMonad)
                {
                    if (function != null)
                    {
                        if (result == null)  // if first time or first time (and second...) was Nothing
                            result = function(aValue);
                        else
                        {
                            var fResult = function(aValue);
                            if (!(fResult is Nothing<B>))        // skip if result is nothing
                                result = result.Concatenate(fResult);
                        }
                    }
                }
                if (result == null) // If some function returned null
                    result = new Nothing<B>();
            }

            return result;
        }

        public override Monad<B> Bind<B>(Func<A, Monad<B>> func)
        {
            if (isNothing)
                return new Nothing<B>();
            else
                return func(aValue);
        }

        public override Monad<B> Bind<B>(Func<A, int, Monad<B>> func)
        {
            if (isNothing)
                return new Nothing<B>();
            else
                return func(aValue, 0);
        }

        public override Func<A, Monad<C>> Kleisli<B, C>(Func<A, Monad<B>> fAtB, Func<B, Monad<C>> fBtC)
        {
            return (a) =>
            {
                if (isNothing)
                    return new Nothing<C>();
                else
                    return fAtB(aValue).Bind(fBtC);
            };
        }

        public override Monad<C> Com<B, C>(Monad<Func<A, B, C>> functionMonad, Monad<B> mOther)
        {
            Maybe<C> result = new Nothing<C>();

            if (!isNothing && !(mOther is Nothing<B>))         // other no nothing monad.
            {
                foreach (var function in functionMonad)
                {
                    if (function != null)
                        foreach (var otherValue in mOther)
                            result = function(aValue, otherValue);
                }
                if (result == null)
                    result = new Nothing<C>();
            }

            return result;
        }

        public override Monad<C> Com<B, C>(Monad<Func<A, B, Monad<C>>> functionMonad, Monad<B> mOther)
        {
            Monad<C> result = new Nothing<C>();

            if (!isNothing && !(mOther is Nothing<B>))         // other is no maybe and this is not nothing.
            {
                result = null;
                foreach (var function in functionMonad)
                {
                    foreach (var otherValue in mOther)
                    {
                        if (result == null)       // Make result monad the monad type of the function result
                            result = function(aValue, otherValue);
                        else
                        {
                            var fResult = function(aValue, otherValue);
                            if (!(fResult is Nothing<B>))
                                result = result.Concatenate(fResult);
                        }
                    }
                }
                if (result == null)
                    result = new Nothing<C>();
            }

            return result;
        }

        public override Monad<C> Com<B, C>(Func<A, B, C> function, Monad<B> mOther)
        {
            Monad<C> resultMonad = new Nothing<C>();  // New Nothing<B> maybe
            if (!isNothing && !(mOther is Nothing<B>))
            {
                foreach (var otherValue in mOther)
                    resultMonad = new Just<C>(function(aValue, otherValue));
            }
            return resultMonad;
        }

        public override Monad<C> Com<B, C>(Func<A, B, Monad<C>> function, Monad<B> mOther)
        {
            Monad<C> result = new Nothing<C>();  // New Nothing<B> maybe
            if (!isNothing && !(mOther is Nothing<B>))
            {
                result = null;
                foreach (var otherValue in mOther)
                {
                    if (result == null)
                        result = function(aValue, otherValue);
                    else
                    {
                        var fResult = function(aValue, otherValue);
                        if (!(fResult is Nothing<B>))
                            result = result.Concatenate(fResult);
                    }
                }
                if (result == null)
                    result = new Nothing<C>();
            }
            return result;
        }

        public override Monad<A> Visit(Action<A> action)
        {
            if (this is Just<A> && action != null)
                action(aValue);
            return this;
        }

        public override Monad<A> Visit<B>(Action<A, B> action, Monad<B> mOther)
        {
            if (this is Just<A> && action != null && mOther != null)
                foreach (var element in mOther)
                    action(aValue, element);
            return this;
        }

        public override Monad<A> Append(A value)
        {
            if (!isNothing)
                aValue = value;
            return this;
        }

        /// <summary>
        /// If this is not nothing, then the result monad is a new Maybe<A> with the value inside the other monad.
        /// </summary>
        /// <param name="otherMonad">The other monad.</param>
        /// <returns>The new monad.</returns>
        public override Monad<A> Concatenate(Monad<A> otherMonad)
        {
            if (!isNothing)
                aValue = otherMonad.Return();
            return this;
        }

        #endregion

        #region IEnumerator_Implementation

        public override IEnumerator<A> GetEnumerator()
        {
            return new SingleEnumerator<A>(Return());
        }

        #endregion
    }
}
