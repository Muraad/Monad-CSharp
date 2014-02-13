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
    public static partial class Extensions
    {
        public static ListMonad<A> ToListMonad<A>(this A value)
        {
            return new ListMonad<A>() { value };
        }

        public static ListMonad<A> ToListMonad<A>(this IList<A> value)
        {
            ListMonad<A> result = new ListMonad<A>();
            foreach (A element in value)
                result.Append(element);
            return result;
        }

        public static ListMonad<T> ToListMonad<T>(this IMonad<T> value)
        {
            ListMonad<T> result = new ListMonad<T>();
            foreach (T element in value)
                result.Add(element);
            return result;
        }
    }

    public class ListMonad<A> : IMonad<A>, IList<A> 
    {
        private List<A> list = new List<A>();

        public override string ToString()
        {
            StringBuilder res = new StringBuilder("LM<" + this.GetType().GenericTypeArguments[0].Name + ">[");
            int index = 0;

            foreach (var element in list)
            {
                if (index != 0)
                    res.Append(" | " + element);
                else
                    res.Append(element);
                index++;
            }
            res.Append("]");
            return res.ToString();
        }

        #region Operator_overloading

        // applicate with multiplicate operator.
        public static IMonad<A> operator *(ListMonad<A> firstM, IMonad<Func<A, IMonad<A>>> functionMonad)
        {
            return firstM.App(functionMonad);
        }

        public static IMonad<A> operator *(ListMonad<A> firstM, IMonad<Func<A, A>> functionMonad)
        {
            return firstM.App(functionMonad);
        }

        // Combinate with multiplicate operator.
        public static ListMonad<A> operator *(ListMonad<A> firstM, Tuple<IMonad<Func<A, A, A>>, IMonad<A>> tupel)
        {
            return (ListMonad<A>)firstM.Com<A, A>(tupel.Item1, tupel.Item2);
        }

        public static ListMonad<A> operator *(ListMonad<A> firstM, Tuple<IMonad<Func<A, A, IMonad<A>>>, IMonad<A>> tupel)
        {
            
            return (ListMonad<A>)firstM.Com<A, A>(tupel.Item1, tupel.Item2);
        }

        public static ListMonad<A> operator /(ListMonad<A> firstM, Func<A, A> functionMonad)
        {
            return (ListMonad < A > )firstM.Fmap<A>(functionMonad);
        }

        public static IMonad<A> operator +(ListMonad<A> firstM, ListMonad<A> otherMonad)
        {
            return firstM.Concatenate(otherMonad);
        }

        /// <summary>
        /// ZipWith using the + operator where arguments are packed in a tuple.
        /// </summary>
        /// <param name="firstM">The monad to zip with.</param>
        /// <param name="tuple">The function used for zipping.</param>
        /// <returns>The result zipped monad.</returns>
        public static IMonad<A> operator +(ListMonad<A> firstM, Tuple<Func<A, A, A>, IMonad<A>> tuple)
        {
            return firstM.Com<A, A>(tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// ZipWithResultMonad via + operator and both arguments packed in a tuple.
        /// </summary>
        /// <param name="firstM">The other monad to zip with.</param>
        /// <param name="tuple">The tupe with the funtion that is used to zip the two values together</param>
        /// <returns>The ziped result monad.</returns>
        public static IMonad<A> operator +(ListMonad<A> firstM, Tuple<Func<A, A, IMonad<A>>, IMonad<A>> tuple)
        {
            return firstM.Com<A, A>(tuple.Item1, tuple.Item2);
        }

        #endregion

        #region IMonad_Interface_Implementation

        public override IMonad<A> Fmap(Func<A, A> function)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = function(list[i]);
            return this;
        }

        public override IMonad<A> Fmap(Func<A, int, A> function)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = function(list[i], i);
            return this;
        }

        /// <summary>
        /// Map the given function to each element in this ListMonad,
        /// and put the result in a new ListMonad of the result type.
        /// </summary>
        /// <typeparam name="B">Type of the value inside the result ListMonad.</typeparam>
        /// <param name="function">The function to map over the values.</param>
        /// <returns>The result ListMonad<B></returns>
        public override IMonad<B> Fmap<B>(Func<A, B> function)
        {
            ListMonad<B> resultEnumerable = new ListMonad<B>();
            foreach (A element in list)
                resultEnumerable.Append(function(element));
            return resultEnumerable;
        }

        public override IMonad<B> Fmap<B>(Func<A, int, B> function)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            int index = 0;
            foreach (A element in list)
            {
                resultListMonad.Append(function(element, index));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
                index++;
            }
            return resultListMonad;
        }

        /// <summary>
        /// The minimal context of a ListMonad is a list with only one value inside.
        /// </summary>
        /// <param name="parameter">The value to put inside the new ListMonad.</param>
        /// <returns>A new ListMonad.</returns>
        public override IMonad<A> Pure(A parameter)
        {
            list.Clear();
            list.Add(parameter);
            return this;
        }

        public override IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (Func<A, B> function in functionMonad)
            {
                // function can be null, for example when the functionMonad is a Maybe with Nothing<Func<A, IMonad<B>> then default(Func<A, IMonad<B>>) returns null
                // we could check for IMonad as Maybe and then check for isNothing, but then ListMonad have to "know" Maybe, i dont like that.
                if (function != null)
                {
                    foreach (A element in list)
                        resultListMonad.Append(function(element));
                }
            }
            return resultListMonad;
        }

        /// <summary>
        /// Map each function inside the given Monad over each element in this ListMonad,
        /// and put all the values inside the result monads into a new ListMonad of the type B.
        /// </summary>
        /// <typeparam name="B">The type inside the result ListMonad.</typeparam>
        /// <param name="functionMonad">The monad that has functions inside.</param>
        /// <returns>The new ListMonad of type B.</returns>
        public override IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad)
        {
            ListMonad<B> result = new ListMonad<B>();

            foreach (Func<A, IMonad<B>> function in functionMonad)
            {
                // function can be null, for example when the functionMonad is a Maybe with Nothing<Func<A, IMonad<B>> then default(Func<A, IMonad<B>>) returns null
                // we could check for IMonad as Maybe and then check for isNothing, but then ListMonad have to "know" Maybe, i dont like that.
                if (function != null) 
                {
                    foreach (A element in list)     // calculate function result for each element in this ListFunctor<T>
                    {
                            result.Concatenate(function(element));
                    }
                }
            }
            return result;
        }

        public override IMonad<B> Bind<B>(Func<A, IMonad<B>> func)
        {
            IMonad<B> result = null;

            foreach (A element in list)
                if (result == null)
                    result = func(element);
                else
                    result.Concatenate(func(element));

            return result;
        }

        public override IMonad<B> Bind<B>(Func<A, int, IMonad<B>> func)
        {
            IMonad<B> result = null;
            for (int i = 0; i < list.Count; i++)
                if (result == null)
                    result = func(list[i], i);
                else
                    result.Concatenate(func(list[i], i));

            return result;
        }

        public override Func<A, IMonad<C>> Kleisli<B, C>(Func<A, IMonad<B>> fAtB, Func<B, IMonad<C>> fBtC)
        {
            return (a) =>
            {
                IMonad<B> result = null;
                foreach (A element in list)
                    if (result == null)
                        result = fAtB(element);
                    else
                        result.Concatenate(fAtB(element));

                return result.Bind(fBtC);
            };
        }

        public override IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther)
        {
            ListMonad<C> resultListMonad = new ListMonad<C>();
            foreach (Func<A, B, C> f in functionMonad)
                foreach (A a in list)
                    foreach (B b in mOther)
                        resultListMonad.Append(f(a, b));

            return resultListMonad;
        }

        public override IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            ListMonad<C> result = new ListMonad<C>();

            foreach (Func<A, B, IMonad<C>> f in functionMonad)
                foreach (A a in list)
                    foreach (B b in mOther)
                        result.Concatenate(f(a, b));

            return result;
        }

        public override IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther)
        {
            ListMonad<C> resultListMonad = new ListMonad<C>();

            foreach (A elementThis in list)
                foreach (B elementOther in mOther)
                    resultListMonad.Append(function(elementThis, elementOther));

            return resultListMonad;
        }
        
        public override IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther)
        {
            ListMonad<C> result = new ListMonad<C>();

            foreach (A a in list)
                foreach (B b in mOther)
                    result.Concatenate(function(a, b));

            return result;
        }

        public override IMonad<A> Visit(Action<A> function)
        {
            foreach (A element in list)
                function(element);
            return this;
        }

        public override IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther)
        {
            foreach (var aElement in list)
                foreach (var otherElement in mOther)
                    action(aElement, otherElement);
            return this;
        }

        public override A Return()
        {
            return list[list.Count-1];
        }

        public override IMonad<A> Concatenate(IMonad<A> otherMonad)
        {
            if (this.Equals(otherMonad))
                return this;
            else
                foreach (A element in otherMonad)
                    list.Add(element);

            return this;
        }

        public override IMonad<A> Append(A value)
        {
            list.Add(value);
            return this;
        }

        #endregion

        #region IList (decorator) implementation

        public int IndexOf(A item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, A item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public A this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        /*public new void Add(A item)
        {
            list.Add(item);
        }*/

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(A item)
        {
            return list.Contains(item);
        }

        public void CopyTo(A[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(A item)
        {
            return list.Remove(item);
        }

        public void Add(A item)
        {
            list.Add(item);
        }

        #endregion

        public override IEnumerator<A> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
