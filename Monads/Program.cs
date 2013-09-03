using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monads
{
    class Program
    {
        static void Main(string[] args)
        {
            Playground.MaybePlayaround();
            Playground.ListMonadPlayground();

            /*Console.Out.WriteLine("Test static functions for lists. \n Map x*x over list [1..5] and print out result with visit.");
            List<int> list = new List<int>() { 1, 2, 3, 4, 5 };
            list = (List<int>)FunctionalProgramming.ListFunctions.map<int, int>((x) => { return x * x; }, list);
            FunctionalProgramming.ListFunctions.visit<int>((x) => { Console.Out.Write(x.ToString() + ", "); }, list);
            Console.In.ReadLine();

            Console.Out.WriteLine("Create two lists [1..5] and [J(1)..(J5)]: ");
            ListMonad<int> listMonadInt = new ListMonad<int>(){1, 2, 3, 4, 5};
            // Because Maybe class has an implicit operator it can be written very cool and easy like a normal list.
            ListMonad<Maybe<int>> listMonadMaybeInt = new ListMonad<Maybe<int>>() { 1, 2, 3, 4, 5 };
            
            // Print out both lists using Visit method and lambda.
            Console.Out.WriteLine("[1..5]: ");
            listMonadInt.Visit((x) => { Console.Out.Write(x + ", "); });
            Console.Out.WriteLine("");

            Console.Out.WriteLine("[J(1)..J(5)]: ");
            listMonadMaybeInt.Visit((x) => { Console.Out.Write(x + ", "); });
            Console.Out.WriteLine("\n");

            Console.Out.WriteLine("Fmap x*x over the list monad [1..5] and convert to double. Result is ListMonad<double>:");
            var resDblListMonad = listMonadInt.Fmap<double>((x) => {return System.Convert.ToDouble(x * x);}).Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
            Console.In.ReadLine();
            Console.Out.WriteLine("\n");

            Console.Out.WriteLine("Fmap x*x over the list monad [J(1)..J(5)]:");
            listMonadMaybeInt.Fmap<Maybe<double>>(  
                                            (x) =>
                                            {
                                                if (!x.isNothing)
                                                    return new Just<double>(System.Convert.ToDouble(x.Return() * x.Return()));
                                                else
                                                    return new Nothing<double>();         // Nothing propagation in function.
                                            }
                                         ).Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
            Console.In.ReadLine();
            Console.Out.WriteLine("\n");

            Console.Out.WriteLine("Convert the int list monad to a double list monad using a lambda:");
            Console.Out.WriteLine("New ListFunctor<double> the is the result of applying Convert.ToDouble() to all the ints in the first ListFunctor<int>.");
            ListMonad<double> doubleListMonad = (ListMonad<double>)listMonadInt.Fmap<double>((x) => { return System.Convert.ToDouble(x); });
            doubleListMonad.Visit((x) => { Console.Out.Write(x + ", "); });
            Console.In.ReadLine();
            Console.Out.WriteLine("\n");

            Console.Out.WriteLine("Make a ListMonad filled with functions, that take a double and return a double that is packed in another IMonad<double>\n");
            Console.Out.WriteLine("The list monad has three functions inside, where all of them return a new monad (Maybes in this case) [x*x, x*x*x, x*x*x*x, Nothing]");
            ListMonad<Func<double, IMonad<double>>> funcListMonad = new ListMonad<Func<double, IMonad<double>>>();
            funcListMonad.Add((x) => { return new Just<double>(x * x); });
            funcListMonad.Add((x) => { return new Just<double>(x / x); });
            funcListMonad.Add((x) => { return new Just<double>(x % x); });
            funcListMonad.Add((x) => { return new Nothing<double>(); });
            Console.Out.WriteLine("");
            funcListMonad.Visit((x) => { Console.Out.WriteLine(x + ", "); });
            Console.Out.WriteLine("\n");
            Console.In.ReadLine();

            Console.Out.WriteLine("Apply each function in the ListMonad to each double in the ListMonad<double>:");
            doubleListMonad.Visit((x) => { Console.Out.Write(x + ", "); });
            Console.Out.WriteLine("");
            // Process every function in the first ListFunctor with every double in the second ListFunctor
            // Basically here happens some kind of list comprehension, every possible combination is calculated.
            var result = doubleListMonad.App<double>(funcListMonad);
            Console.Out.WriteLine("Output ");
            result.Visit((x) => { Console.Out.Write(x + ", "); });
            Console.In.ReadLine();

            // Make a single Maybe (Just) that holds a Function that returns a IMonad<double>
            Func<double, Maybe<double>> dblFuncMaybe = (x) => { return new Just<double>(x * x * x * x); };
            Maybe<Func<double, IMonad<double>>> maybeFunctionMonad = new Func<double, IMonad<double>>(dblFuncMaybe);
            
            Console.Out.WriteLine("__________________________________________");
            Console.Out.WriteLine("Input: ");
            doubleListMonad.Visit((x) => { Console.Out.Write(x + ", "); });

            // Process this function to all double in the list.
            result = doubleListMonad.App<double>(maybeFunctionMonad);
            
            Console.Out.WriteLine("Output: ");
            result.Visit((x) => { Console.Out.Write(x + ", "); });
            Console.In.ReadLine();

            // Make a Maybe that holds a Nothing<Func<double, IMonad<double>>
            // Result is Nothing too, no matter what input there is.
            Maybe<Func<double, IMonad<double>>> nothingFunctionFunctor = new Nothing<Func<double, IMonad<double>>>();
            Console.Out.WriteLine("Input (List app with Nothing Function): ");
            doubleListMonad.Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
            result = doubleListMonad.App<double>(nothingFunctionFunctor);
            result.Visit((x) => { Console.Out.WriteLine(x.ToString()); });
            Console.In.ReadLine();
            Console.Out.WriteLine("Output: ");
            result.Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
            Console.In.ReadLine();

            Maybe<int> intMaybe = new Nothing<int>();
            Console.Out.WriteLine("A new Nothing<int>:");
            Console.Out.WriteLine(intMaybe);
            Maybe<Func<int, IMonad<double>>> intMaybeFunc = new Func<int, IMonad<double>>((x) => { return new Just<double>(x / 2.0); });
            // Apply a function to the Nothing<int> intMaybe, that returns a maybe int. 
            // The Maybe<int> return value is only for correctness. The function is never called because the intMaybe is Nothing<int>
            // so intMaybe.App will return a new Nothing<int> directly.
            var tmp = intMaybe.App<double>(intMaybeFunc);
            Console.Out.WriteLine("A function applied to the Nothing<int>:");
            Console.Out.WriteLine(tmp);
            
            // Apply a function to the Nothing<int> intMaybe with fmap
            // result is Nothing<int> again.
            var tmp2 = intMaybe.Fmap<double>( (x) => { return x / 2.0; });
            Console.Out.WriteLine("A function proccessed to to intMaybe (Nothing<int>) with fmap:");
            Console.Out.WriteLine(tmp2);
            Console.Out.WriteLine("\n");
            Console.In.ReadLine();

            var query = from x in intMaybe
                        where x == 1
                        select x;
            Console.Out.WriteLine("Linq query:");
            Console.Out.WriteLine(query);
            Console.Out.WriteLine("\n");
            Console.In.ReadLine();

            ListMonad<Func<int, int, int>> functionListMonad = new ListMonad<Func<int,int,int>>();
            functionListMonad.Add( (x, y) => { return x*y;});
            functionListMonad.Add( (x, y) => { return x/y;});
            functionListMonad.Add( (x, y) => { return x%y;});
            ListMonad<int> intListMonad = new ListMonad<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            ListMonad<int> intListMonadTwo = new ListMonad<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

            Console.Out.WriteLine("Combination:");
            //intListMonad.Com<int, int>(functionListMonad, intListMonadTwo).Visit(x => Console.Out.Write(x + ", "));

            intListMonad.Com(functionListMonad, intListMonadTwo).Visit(x => Console.Out.Write(x + ", "));

            // Combination with multiply operator
            var tupel = new Tuple<IMonad<Func<int, int, int>>, IMonad<int>>(functionListMonad, intListMonadTwo);
            var comResult = intListMonad * tupel;
            Console.In.ReadLine();

            var queryTwo_1 = from f in functionListMonad
                             from x in intListMonadTwo
                             from y in intListMonad
                             select f(x, y);

            Console.Out.WriteLine("Linq query(2_1):");
            queryTwo_1.Visit(x => Console.Out.Write(x + ", "));
            Console.In.ReadLine();

            var queryTwo = from f in funcListMonad
                           from x in intListMonadTwo
                           from y in intListMonad //.Where( z => { return z > 4; })
                           where y > 7
                           where x > 7
                           select f(x * y);

            Console.Out.WriteLine("Linq query(2):");
            queryTwo.Visit(x => Console.Out.Write(x + ", "));
            Console.In.ReadLine();

            var queryThree = from x in listMonadInt
                             where x > 2
                             select x;
            Console.Out.WriteLine("Linq query:");
            queryThree.Visit(x => Console.Out.Write(x + ", "));
            Console.In.ReadLine();

            var queryFour = from x in listMonadMaybeInt
                            select x;
            Console.Out.WriteLine("Linq query:");
            Console.Out.WriteLine(queryFour);
            Console.In.ReadLine();

            var resultIntMonad = intListMonad.Com<int, int>((x, y) => { return x % y; }, intListMonadTwo)
                                 .Fmap<double>((x) => { return System.Convert.ToDouble(x); })
                                 .App<double>(funcListMonad);

            Console.Out.WriteLine("ZipWith two ListMonad<int>´s:");
            resultIntMonad.Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
            Console.In.ReadLine();

            // make a List comprehension of every possible combination of two lists.
            // the listmonad result from lambda is flattened out.
            var resultMonad = intListMonad.Com<int, int>((x, y) => { return new ListMonad<int>() { x, y }; }, intListMonadTwo);
            Console.Out.WriteLine("ZipWith two ListMonad<int>´s:");
            resultMonad.Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
            Console.In.ReadLine();

            EitherDecorator<double, int> either = new EitherDecorator<double, int>(1.5, 7);
            double dblValue = (double)either.Return();
            int intValue = either.Return();
            Console.Out.WriteLine(dblValue + " : " + intValue);
            Console.In.ReadLine();

            var tmpResult = intListMonad / ((x) => {return x*x*x*x*x*x*x*x*x;});
            tmpResult.Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
            Console.In.ReadLine();

            Maybe<int> intM = 5;
            Maybe<int> intM2 = new Just<int>(5);
            Maybe<int> nothingIntM = 0;

            Maybe<int> justInt = 1;
            var listMonadChar = new ListMonad<char>(){'a', 'b', 'c', 'd', 'e'};
            justInt.Visit(new Action<int>((x) => { Console.Out.Write(x); }));
            Console.Out.WriteLine();
            justInt.Visit(new Action<int,char>((i, c) => {Console.Out.Write(i + "" + c + ", ");}), listMonadChar);
            Console.Out.WriteLine();
            justInt.Fmap<double>((x) => { return x * 0.5; }).Visit((x) => { Console.Out.Write(x); });


            int integer = intM;                 // From Just to int.
            nothingIntM = -1;
            int integer1 = intM2;
            int integerNothing = nothingIntM;    // Gets default(int) value.

            Maybe<int> intN = null;       // be carefull here the int it self is null
            Maybe<int> nothingInt = 0;    // same as with null in line below
            Maybe<Action<int>> nothingAction = new Nothing<Action<int>>();  // here it is a Nothing<Action<int>> only the value inside the nothing is null
            Maybe<Action<int>> justAction = new Just<Action<int>>((x) => { Console.Out.Write(x.ToString() + ", "); });
            ListMonad<Func<int, int>> fListMonad = new ListMonad<Func<int, int>>();
            fListMonad.Add((x) => { return x * x; });
            fListMonad.Add((x) => { return x / x; });
            fListMonad.Add((x) => { return x * x * x; });
            Func<int, IMonad<int>> function = (x) => { return new Just<int>(x * x * x * x * x * x); };
            var maybeFuncMonad = new Just<Func<int, IMonad<int>>>(function);
            resDblListMonad = new ListMonad<Double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            var tmpResult2 = (ListMonad<double>)resDblListMonad * funcListMonad;
           // var tmpResult2 = (ListMonad<int>)intListMonad <= function;
            tmpResult2.Visit((x) => { Console.Out.Write(x.ToString() + ", "); });
             Console.In.ReadLine();
             */
        }
    }
}
