using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monads
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
