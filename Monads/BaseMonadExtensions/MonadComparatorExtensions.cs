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
    public static class MonadComparatorExtensions
    {
        public static bool Contains<A, D>(this Monad<A> monad, D dest, Func<A, D, bool> comparer)
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

        public static A GetItemBy<A, D>(this Monad<A> monad, D dest, Func<A, D, bool> comparer)
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

        public static int IndexOfBy<A, D>(this Monad<A> monad, D dest, Func<A, D, bool> comparer)
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

        public static bool Any<A, D>(this Monad<A> monad, Monad<D> destMonad, Func<A, D, bool> comparer)
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

        public static bool All<A, D>(this Monad<A> monad, Monad<D> destMonad, Func<A, D, bool> comparer)
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

        public static Monad<A> Except<A, D>(this Monad<A> monad, Monad<D> destMonad, Func<A, D, bool> comparer)
        {
            Type[] arg = new Type[] { };
            Monad<A> result = (Monad<A>)monad.GetType().GetConstructor(arg).Invoke(null);
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

        public static Monad<A> Match<A, D>(this Monad<A> monad, Monad<D> destMonad, Func<A, D, bool> comparer)
        {
            Type[] arg = new Type[] { };
            Monad<A> result = (Monad<A>)monad.GetType().GetConstructor(arg).Invoke(null);
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

    }
}
