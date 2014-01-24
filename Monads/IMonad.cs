
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
                        result.Add(element);
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
                        result.Add(element);
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
    }

    public interface IMonad<A> : IEnumerable<A>
    {

        #region IMonad_Core_Interface_Function_Definitions

        // Haskell fmap from Monad, maps a function over the value inside this monad.
        IMonad<B> Fmap<B>(Func<A, B> function);

        // Haskell pure from Monad. Puts a given value in the minimal context of this monad.
        IMonad<A> Pure(A parameter);

        // Bind function 
        IMonad<B> Bind<B>(Func<A, IMonad<B>> func);

        // Kleisli-Operator
        Func<A, IMonad<C>> Kleisli<B, C>(Func<A, IMonad<B>> fAtB, Func<B, IMonad<C>> fBtC);

        // Haskell return from Monad. Returns the value inside this monad.
        A Return();

        IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad);

        // Haskell applicative function (operator) from Applicative. 
        // In Haskell its from a ApplicativeFunctor, but it is the same that a monad. The only established the ApplicativeFunctor later.
        IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad);

        // Combination
        IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther);
        IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther);
        IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther);
        IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther);

        // Usefull helper function. Do a action on every element in this monad, and return this at the end.
        IMonad<A> Visit(Action<A> function);
        IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther);

        IMonad<A> Add(A value);

        IMonad<A> Concat(IMonad<A> otherMonad);

        #endregion

      
        #region Linq_Enumerable_Connection

        IMonad<A> Where(Func<A, bool> predicate);   // filter.
        IMonad<A> Where(Func<A, int, bool> predicate);

        IMonad<B> Select<B>(Func<A, B> f);       // fmap
        IMonad<B> Select<B>(Func<A, Int32, B> f);   // fmap with index.

        IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f);          // bind
        IMonad<B> SelectMany<B>(Func<A, Int32, IMonad<B>> f);
        IMonad<B> SelectMany<TMonad, B>(Func<A, IMonad<TMonad>> selector, 
                                        Func<A, TMonad, B> function);
        IMonad<B> SelectMany<TMonad, B>(Func<A, Int32, IMonad<TMonad>> selector, 
                                        Func<A, TMonad, B> function);

        #endregion
    }
}
