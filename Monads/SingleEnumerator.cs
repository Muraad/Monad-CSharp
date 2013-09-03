using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
