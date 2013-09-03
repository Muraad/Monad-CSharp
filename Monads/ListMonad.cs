using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monads
{
    public class ListMonad<A> : List<A>, IMonad<A>
    {
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
        public static IMonad<A> operator *(ListMonad<A> firstM, Tuple<IMonad<Func<A, A, A>>, IMonad<A>> tupel )
        {
            return firstM.Com<A, A>(tupel.Item1, tupel.Item2);
        }

        public static IMonad<A> operator *(ListMonad<A> firstM, Tuple<IMonad<Func<A, A, IMonad<A>>>, IMonad<A>> tupel)
        {
            return firstM.Com<A, A>(tupel.Item1, tupel.Item2);
        }

        public static IMonad<A> operator /(ListMonad<A> firstM, Func<A, A> functionMonad)
        {
            return firstM.Fmap(functionMonad);
        }

        public static IMonad<A> operator +(ListMonad<A> firstM, ListMonad<A> otherMonad)
        {
            return firstM.Concat(otherMonad);
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
        public IMonad<B> Fmap<B>(Func<A, B> function)
        {
            ListMonad<B> resultEnumerable = new ListMonad<B>();
            foreach (A element in this)
                resultEnumerable.Add(function(element));
            return resultEnumerable;
        }

        /// <summary>
        /// The minimal context of a ListMonad is a list with only one value inside.
        /// </summary>
        /// <param name="parameter">The value to put inside the new ListMonad.</param>
        /// <returns>A new ListMonad.</returns>
        public IMonad<A> Pure(A parameter)
        {
            ListMonad<A> list = new ListMonad<A>();
            list.Add(parameter);
            return list;
        }

        public IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (Func<A, B> function in functionMonad)
            {
                // function can be null, for example when the functionMonad is a Maybe with Nothing<Func<A, IMonad<B>> then default(Func<A, IMonad<B>>) returns null
                // we could check for IMonad as Maybe and then check for isNothing, but then ListMonad have to "know" Maybe, i dont like that.
                if (function != null)
                {
                    foreach (A element in this)     // calculate function result for each element in this ListFunctor<T>
                        resultListMonad.Add(function(element));
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
        public IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (Func<A, IMonad<B>> function in functionMonad)
            {
                // function can be null, for example when the functionMonad is a Maybe with Nothing<Func<A, IMonad<B>> then default(Func<A, IMonad<B>>) returns null
                // we could check for IMonad as Maybe and then check for isNothing, but then ListMonad have to "know" Maybe, i dont like that.
                if (function != null) 
                {
                    foreach (A element in this)     // calculate function result for each element in this ListFunctor<T>
                    {
                        //IMonad<B> funcResult = function(element);
                        //foreach (B resElement in funcResult)
                        foreach (B resElement in function(element))
                            resultListMonad.Add(resElement);
                    }
                }
            }
            return resultListMonad;
        }

        public IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther)
        {
            ListMonad<C> resultListMonad = new ListMonad<C>();
            foreach (Func<A, B, C> f in functionMonad)
                foreach (A a in this)
                    foreach (B b in mOther)
                        resultListMonad.Add(f(a, b));
            return resultListMonad;
        }

        public IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            ListMonad<C> resultListMonad = new ListMonad<C>();
            foreach (Func<A, B, IMonad<C>> f in functionMonad)
                foreach (A a in this)
                    foreach (B b in mOther)
                    {
                        IMonad<C> fResult = f(a, b);
                        foreach (C c in fResult)
                            resultListMonad.Add(c);
                    }
            return resultListMonad;
        }

        public IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther)
        {
            ListMonad<C> resultListMonad = new ListMonad<C>();

            foreach (A elementThis in this)
                foreach (B elementOther in mOther)
                    resultListMonad.Add(function(elementThis, elementOther));

            return resultListMonad;
        }
        public IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther)
        {
            ListMonad<C> resultListMonad = new ListMonad<C>();

            foreach (A elementThis in this)
            {
                foreach (B elementOther in mOther)
                {
                    var fResult = function(elementThis, elementOther);
                    foreach (C resultElement in fResult)
                        resultListMonad.Add(resultElement);
                }
            }

            return resultListMonad;
        }

        public IMonad<A> Visit(Action<A> function)
        {
            foreach (A element in this)
                function(element);
            return this;
        }

        public IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther)
        {
            foreach (var aElement in this)
                foreach (var otherElement in mOther)
                    action(aElement, otherElement);
            return this;
        }

        // Here is a "problem", this should return List<T> but this is a generall problem,
        // because we cannt have a IFunctor<List<T>>
        // because in c# you are not able to extend original List<T> with static methods AND let it implement (:) IFunctor<List<T>> at the same time.
        // This would be very cool generally, becaus then Maybe would be IFunctor<Maybe<T>>
        public A Return()
        {
            return this.First<A>();
        }

        public IMonad<A> Concat(IMonad<A> otherMonad)
        {
            ListMonad<A> resultListMonad = new ListMonad<A>();

            // Copy all references in this ListMonad to the new ListMonad.
            foreach(A element in this)
                resultListMonad.Add(element);

            // Get all values from the other monad and put them in the result list monad too.
            foreach (A element in otherMonad)
                resultListMonad.Add(element);

            return resultListMonad;
        }

        #endregion

        #region Linq_Enumerable_Functions

        public IMonad<A> Where(Func<A, bool> predicate)
        {
            ListMonad<A> resultListMonad = new ListMonad<A>();
            foreach (A element in this)
                if (predicate(element))
                    resultListMonad.Add(element);
            return resultListMonad;
        }

        public IMonad<A> Where(Func<A, int, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IMonad<B> Select<B>(Func<A, B> f)
        {
            return Fmap<B>(f);
        }

        public IMonad<B> Select<B>(Func<A, int, B> function)
        {
            ListMonad<B> resultMonad = new ListMonad<B>();
            int index = 0;
            foreach (A element in this)
            {
                resultMonad.Add(function(element, index));
                index++;
            }
            return resultMonad;
        }


        public IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (A element in this)
                resultListMonad.Concat(f(element));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
            return resultListMonad;
        }

        public IMonad<B> SelectMany<B>(Func<A, int, IMonad<B>> function)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            int index = 0;
            foreach (A element in this)
            {
                resultListMonad.Concat(function(element, index));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
                index++;
            }
            return resultListMonad;
        }

        public IMonad<B> SelectMany<R, B>(Func<A, IMonad<R>> selector, Func<A, R, B> function)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            foreach (A element in this)
                foreach(R rValue in selector(element))          
                    resultListMonad.Add(function(element, rValue));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
            return resultListMonad;
        }

        public IMonad<B> SelectMany<R, B>(Func<A, int, IMonad<R>> selector, Func<A, R, B> function)
        {
            ListMonad<B> resultListMonad = new ListMonad<B>();
            int index = 0;
            foreach (A element in this)
            {
                foreach (R rValue in selector(element, index))             // 
                    resultListMonad.Add(function(element, rValue));        // Get the value of type B out of the result IMonad<B>, no matter what monad the result is.
                index++;
            }
            return resultListMonad;
        }

        #endregion
    }
}
