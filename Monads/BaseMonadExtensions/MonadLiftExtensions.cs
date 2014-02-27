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
using System.Reflection;

namespace Monads
{
    public static class MonadLiftExtensions
    {
        public static Func<Monad<A>> LiftM<A>(this Monad<A> monad, Func<A> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            Monad<A> m = (Monad<A>)constructor.Invoke(null);
            return () =>
            {
                return m.Pure(func());
            };
        }

        public static Func<Monad<A>, Monad<B>> LiftM<A, B>(this Monad<B> monad, Func<A, B> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            Monad<B> m = (Monad<B>)constructor.Invoke(null);
            return (a) =>
            {
                return m.Pure(func(a.Return()));
            };
        }

        public static Func<Monad<A>, Monad<B>, Monad<C>> LiftM<A, B, C>(this Monad<C> monad, Func<A, B, C> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            Monad<C> m = (Monad<C>)constructor.Invoke(null);
            return (a, b) =>
            {
                return m.Pure(func(a.Return(), b.Return()));
            };
        }

        public static Func<Monad<A>, Monad<B>, Monad<C>, Monad<D>> LiftM<A, B, C, D>(this Monad<D> monad, Func<A, B, C, D> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            Monad<D> m = (Monad<D>)constructor.Invoke(null);
            return (a, b, c) =>
            {
                return m.Pure(func(a.Return(), b.Return(), c.Return()));
            };
        }

        public static Func<Monad<A>, Monad<B>, Monad<C>, Monad<D>, Monad<E>> LiftM<A, B, C, D, E>(this Monad<E> monad, Func<A, B, C, D, E> func)
        {
            ConstructorInfo constructor = monad.GetType().GetConstructor(new Type[] { });
            Monad<E> m = (Monad<E>)constructor.Invoke(null);
            return (a, b, c, d) =>
            {
                return m.Pure(func(a.Return(), b.Return(), c.Return(), d.Return()));
            };
        }


        public static Func<Monad<A>, Monad<A>> LiftM<A>(this Monad<A> monad, Func<A, A> func)
        {
            return LiftM<A, A>(monad, func);
        }

        public static Func<Monad<A>, Monad<A>, Monad<A>> LiftM<A>(this Monad<A> monad, Func<A, A, A> func)
        {
            return LiftM<A, A, A>(monad, func);
        }

        public static Func<Monad<A>, Monad<A>, Monad<A>, Monad<A>> LiftM<A>(this Monad<A> monad, Func<A, A, A, A> func)
        {
            return LiftM<A, A, A, A>(monad, func);
        }

        public static Func<Monad<A>, Monad<A>, Monad<A>, Monad<A>, Monad<A>> LiftM<A>(this Monad<A> monad, Func<A, A, A, A, A> func)
        {
            return LiftM<A, A, A, A, A>(monad, func);
        }
    }
}
