
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

namespace Monads
{
    /// <summary>
    /// MaybeEnumb always has only one element!
    /// The element is default(A) if maybe is Nothing<A>, if its Just<A> then the element is the just value. 
    /// Be carefull default(A) can be null!!!!
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class SingleEnumerator<A> : IEnumerator<A>
    {
        private A aValue = default(A);
        private bool next = true;

        public SingleEnumerator(A value)
        {
            aValue = value;
        }

        public A Current
        {
            get
            {
                next = false;
                return aValue;
            }
        }

        public void Dispose()
        {
            return;
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                next = false;
                return aValue;
            }
        }

        public bool MoveNext()
        {
            return next;
        }

        public void Reset()
        {
            next = true;
        }
    }
}
