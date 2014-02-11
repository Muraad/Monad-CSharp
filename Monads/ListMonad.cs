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

        /// <summary>
        /// The minimal context of a ListMonad is a list with only one value inside.
        /// </summary>
        /// <param name="parameter">The value to put inside the new ListMonad.</param>
        /// <returns>A new ListMonad.</returns>
        public override IMonad<A> Pure(A parameter)
        {
            //ListMonad<A> list = new ListMonad<A>();
            //list.Append(parameter);
            //return list;
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
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (Func<A, IMonad<B>> function in functionMonad)
            {
                // function can be null, for example when the functionMonad is a Maybe with Nothing<Func<A, IMonad<B>> then default(Func<A, IMonad<B>>) returns null
                // we could check for IMonad as Maybe and then check for isNothing, but then ListMonad have to "know" Maybe, i dont like that.
                if (function != null) 
                {
                    foreach (A element in list)     // calculate function result for each element in this ListFunctor<T>
                    {
                        //IMonad<B> funcResult = function(element);
                        //foreach (B resElement in funcResult)
                        foreach (B resElement in function(element))
                            resultListMonad.Append(resElement);
                    }
                }
            }
            return resultListMonad;
        }

        public override IMonad<B> Bind<B>(Func<A, IMonad<B>> func)
        {
            IMonad<B> result = null;
            foreach (A element in list)
            {
                if (result == null)
                    result = func(element);
                else
                    result = result.Concatenate(func(element));
            }
            return result;
        }

        public override Func<A, IMonad<C>> Kleisli<B, C>(Func<A, IMonad<B>> fAtB, Func<B, IMonad<C>> fBtC)
        {
            return (a) =>
            {
                IMonad<B> result = null;
                foreach (A element in list)
                {
                    if (result == null)
                        result = fAtB(element);
                    else
                        result = result.Concatenate(fAtB(element));
                }
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
            ListMonad<C> resultListMonad = new ListMonad<C>();
            foreach (Func<A, B, IMonad<C>> f in functionMonad)
                foreach (A a in list)
                    foreach (B b in mOther)
                    {
                        IMonad<C> fResult = f(a, b);
                        foreach (C c in fResult)
                            resultListMonad.Append(c);
                    }
            return resultListMonad;
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
            ListMonad<C> resultListMonad = new ListMonad<C>();

            foreach (A elementThis in list)
            {
                foreach (B elementOther in mOther)
                {
                    var fResult = function(elementThis, elementOther);
                    foreach (C resultElement in fResult)
                        resultListMonad.Append(resultElement);
                }
            }

            return resultListMonad;
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
            return list[list.Count];
        }

        public override IMonad<A> Concatenate(IMonad<A> otherMonad)
        {
            //ListMonad<A> resultListMonad = new ListMonad<A>();

            // Copy all references in this ListMonad to the new ListMonad.
            //foreach (A element in list)
            //    resultListMonad.Append(element);

            // Get all values from the other monad and put them in the result list monad too.
            //foreach (A element in otherMonad)
            //    resultListMonad.Append(element);

            //return resultListMonad;
            foreach (A element in otherMonad)
                this.Add(element);
            return this;
        }

        public override IMonad<A> Append(A value)
        {
            list.Add(value);
            return this;
        }

        #endregion

        #region Linq_Enumerable_Functions

        public override IMonad<A> Where(Func<A, bool> predicate)
        {
            ListMonad<A> resultListMonad = new ListMonad<A>();
            foreach (A element in list)
                if (predicate(element))
                    resultListMonad.Append(element);
            return resultListMonad;
        }

        public override IMonad<A> Where(Func<A, int, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> Select<B>(Func<A, B> f)
        {
            return Fmap<B>(f);
        }

        public override IMonad<B> Select<B>(Func<A, int, B> function)
        {
            ListMonad<B> resultMonad = new ListMonad<B>();
            int index = 0;
            foreach (A element in list)
            {
                resultMonad.Append(function(element, index));
                index++;
            }
            return resultMonad;
        }


        public override IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (A element in list)
                resultListMonad.Concatenate(f(element));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
            return resultListMonad;
        }

        public override IMonad<B> SelectMany<B>(Func<A, int, IMonad<B>> function)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            int index = 0;
            foreach (A element in list)
            {
                resultListMonad.Concatenate(function(element, index));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
                index++;
            }
            return resultListMonad;
        }

        public override IMonad<B> SelectMany<R, B>(Func<A, IMonad<R>> selector, Func<A, R, B> function)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (A element in list)
                foreach(R rValue in selector(element))          
                    resultListMonad.Append(function(element, rValue));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
            return resultListMonad;
        }

        public override IMonad<B> SelectMany<R, B>(Func<A, int, IMonad<R>> selector, Func<A, R, B> function)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            int index = 0;
            foreach (A element in list)
            {
                foreach (R rValue in selector(element, index))             // 
                    resultListMonad.Append(function(element, rValue));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
                index++;
            }
            return resultListMonad;
        }

        #endregion

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
