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

namespace Monads.Extension.IdentityAtomicExtensions
{
    public static class IdentityAtomicExtensions
    {
        public static bool CompareExchange<T>(this Identity<T> id, T value, Func<T, bool> comparand)
        {
            bool result = false;
            id.Lock.EnterWriteLock();
            try
            {
                if ((result = comparand(id.Value)))
                    id.Value = value;
            }
            finally
            {
                id.Lock.ExitWriteLock();
            }
            return result;
        }

        public static T Read<T>(this Identity<T> id)
        {
            T result = default(T);
            id.Lock.EnterReadLock();
            try
            {
                result = id.Value;
            }
            finally
            {
                id.Lock.ExitWriteLock();
            }
            return result;
        }

        public static void Write<T>(this Identity<T> id, T value)
        {
            id.Lock.EnterWriteLock();
            try
            {    
                id.Value = value;
            }
            finally
            {
                id.Lock.ExitWriteLock();
            }
        }
    }
}
