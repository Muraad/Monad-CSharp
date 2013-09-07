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
    public class ListFunctions
    {

        public static void visit<A>(Action<A> action, ICollection<A> collection)
        {
            foreach (A element in collection)
                action(element);
        }

        public static ICollection<B> map<A, B>(Func<A, B> function, ICollection<A> collection)
        {
            ICollection<B> resultEnumerable = new List<B>();
            foreach (A element in collection)
                resultEnumerable.Add(function(element));
            return resultEnumerable;
        }

        public static A filter<A>(Func<A, bool> predicate, A collection)
            where A : ICollection<A>
        {
            var resultList = collection.Where(predicate);
            return (A)resultList;
        }

        public static C zipWith<A, B, C>(Func<A, B, C> function, A collA, B collB)
            where A : ICollection<A>
            where B : ICollection<B>
            where C : ICollection<C>
        {
            if (collA.Count() > 0 && collB.Count() > 0)
                return (C)function(collA.First(), collB.First()).Concat(zipWith<A, B, C>(function, collA, collB));
            else
                return default(C);
        }
    }
}
