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
    public class CacheEntry<K, V> : Tuple<K, DateTime, V>
    {
        public CacheEntry(K key, V value)
            : base(key, DateTime.Now, value)
        {
        }

        public K Key
        {
            get
            {
                return Item1;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return Item2;
            }
        }

        public V Value
        {
            get
            {
                return Item3;
            }
        }

        public override string ToString()
        {
            return Key.ToString() + " : " + Value.ToString() + " (" + DateTime.Diff(DateTime.Now) + ") ";
        }
    }
}
