﻿/*
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
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FunctionalProgramming
{
    public static partial class Extensions
    {
        public static Identity<T> ToIdentity<T>(this T value)
        {
            return new Identity<T>(value);
        }

        public static Identity<T> ToIdentity<T>(this IMonad<T> value)
        {
            return new Identity<T>(value.Return());
        }
    }

    public class Identity<A> : IMonad<A>
    {
        private A idValue;
        
        public Identity(A aValue)
        {
            this.idValue = aValue;
        }

        public Identity()
        {
            this.idValue = default(A);
        }

        public A Value
        {
            get { return idValue; }
            set { this.idValue = value; }
        }

        public static implicit operator A(Identity<A> instance)
        {
            return instance.Return();
        }

        public static implicit operator Identity<A>(A value)
        {
            return new Identity<A>(value);
        }

        public override string ToString()
        {
            return "Id<" + Return().GetType().Name + ">(" + Return().ToString() + ")";
        }

        #region IMonad_Interface_Implementation

        public override IMonad<B> Fmap<B>(Func<A, B> function)
        {
            return new Identity<B>(function(idValue));
        }

        public override IMonad<A> Pure(A parameter)
        {
            idValue = parameter;
            return this;
        }

        public override A Return()
        {
            return idValue;
        }

        public override IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad)
        {
            Identity<B> resultIdentity = new Identity<B>();
            foreach (var function in functionMonad)
                if (function != null)
                    resultIdentity = function(idValue);
            return resultIdentity;
        }

        public override IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad)
        {
            IMonad<B> resultMonad = null;
            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    if (resultMonad == null)
                        resultMonad = function(idValue);
                    else
                        resultMonad = resultMonad.Concatenate(function(idValue));
                }
            }
            if (resultMonad == null)
                resultMonad = new Identity<B>();
            return resultMonad;
        }

        public override IMonad<B> Bind<B>(Func<A, IMonad<B>> func)
        {
            return func(idValue);
        }

        public override Func<A, IMonad<C>> Kleisli<B, C>(Func<A, IMonad<B>> fAtB, Func<B, IMonad<C>> fBtC)
        {
            return (a) =>
            {
                return fAtB(idValue).Bind(fBtC);
            };
        }

        public override IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther)
        {
            Identity<C> resultMonad = new Identity<C>();

            foreach (var function in functionMonad)
                if (function != null)
                    foreach (var value in mOther)
                        resultMonad = function(idValue, value);

            return resultMonad;
        }

        public override IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            IMonad<C> resultMonad = null;

            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    foreach (var value in mOther)
                    {
                        if (resultMonad == null)
                            resultMonad = function(idValue, value);
                        else
                            resultMonad = resultMonad.Concatenate(function(idValue, value));
                    }
                }
            }
            if (resultMonad == null)
                resultMonad = new Identity<C>();

            return resultMonad;
        }

        public override IMonad<A> Visit(Action<A> action)
        {
            action(idValue);
            return this;
        }

        public override IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther)
        {
            foreach (var element in mOther)
                action(idValue, element);
            return this;
        }

        public override IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther)
        {
            Identity<C> resultMonad = new Identity<C>();

            foreach (var value in mOther)
                resultMonad = function(idValue, value);

            if (resultMonad == null)
                resultMonad = new Identity<C>();

            return resultMonad;
        }

        public override IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther)
        {
            IMonad<C> resultMonad = null;

            foreach (var value in mOther)
                if (resultMonad == null)
                    resultMonad = function(idValue, value);
                else
                    resultMonad = resultMonad.Concatenate(function(idValue, value));

            if (resultMonad == null)
                resultMonad = new Identity<C>();

            return resultMonad;
        }

        public override IMonad<A> Append(A value)
        {
            idValue = value;
            return this;
        }

        public override IMonad<A> Concatenate(IMonad<A> otherMonad)
        {
            this.idValue = otherMonad.Return();
            return this;
        }

        #endregion

        #region Linq_Enumerable_Interface_implemenation

        public override IMonad<A> Where(Func<A, bool> predicate)
        {
            IMonad<A> resultMonad = new Identity<A>();
            if (predicate(idValue))
                resultMonad = new Identity<A>(idValue);
            return resultMonad;
        }

        public override IMonad<A> Where(Func<A, int, bool> predicate)
        {
            IMonad<A> resultMonad = new Identity<A>();
            if (predicate(idValue, 0))
                resultMonad = new Identity<A>(idValue);
            return resultMonad;
        }

        public override IMonad<B> Select<B>(Func<A, B> function)
        {
            return Fmap<B>(function);
        }

        public override IMonad<B> Select<B>(Func<A, int, B> function)
        {
            return new Identity<B>(function(idValue, 0));
        }

        public override IMonad<B> SelectMany<B>(Func<A, IMonad<B>> function)
        {
            return function(idValue);
        }

        public override IMonad<B> SelectMany<B>(Func<A, int, IMonad<B>> function)
        {
            return function(idValue, 0);
        }

        public override IMonad<B> SelectMany<R, B>(Func<A, IMonad<R>> selector, Func<A, R, B> function)
        {
            IMonad<R> tmp = selector(idValue);
            B result = function(idValue, tmp.Return());
            return new Identity<B>(result);
        }

        public override IMonad<B> SelectMany<R, B>(Func<A, int, IMonad<R>> selector, Func<A, R, B> function)
        {
            IMonad<R> tmp = selector(idValue, 0);
            B result = function(idValue, tmp.Return());
            return new Identity<B>(result);
        }

        #endregion

        #region IEnumerator_Implementation

        public override IEnumerator<A> GetEnumerator()
        {
            return new SingleEnumerator<A>(idValue);
        }

        #endregion        
   
    }
}
