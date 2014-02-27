﻿/*
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

namespace Monads
{
    public static class Playground
    {
        public static void MaybePlayaround()
        {
            Console.Out.WriteLine("\n-------------------------------------------------------------");
            Console.Out.WriteLine("------------------------Maybe playground-----------------------");
            Console.Out.WriteLine("-------------------------------------------------------------\n");

            // Just 5, use implicit operator for *Maybe* to make a Just directly.
            Maybe<int> justInt = 5;
            Console.WriteLine("A new Just<double>: " + justInt.ToString());
            Maybe<int> nothingInt = 0;      // same as nothingInt = new Nothing<int>();
            Console.WriteLine("A new Nothing<double>: " + nothingInt.ToString());

            // justInt = 0; or justInt = new Nothing<int>() would make a Nothing out of the justInt

            Console.WriteLine("A new ListMonad<char>: ");
            var listMonadChar = new ListMonad<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g' }
                                .Visit((x) => { Console.Out.Write(x + ", "); });
            Console.WriteLine("\n___________________________________________________________");

            Console.WriteLine("A new ListMonad<int>: ");
            var listMonadInt = new ListMonad<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                                .Visit((x) => { Console.Out.Write(x + ", "); });
            Console.WriteLine("\n___________________________________________________________");

            Console.WriteLine("A new ListMonad<double>: ");
            var listMonadDouble = new ListMonad<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                                    .Visit((x) => { Console.Out.Write(x + ", "); });
            Console.WriteLine("\n___________________________________________________________");

            var intToDoubleFunction = new Func<int, double>(x => { return x * 0.5; });
            Console.WriteLine("A new Just with a function inside: f(x) = x * 0.5 ");
            var justFunction = new Just<Func<int, double>>(intToDoubleFunction);
            Console.WriteLine(justFunction.ToString());
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Visits each combination of the Just 5 and the ListMonad<char>" + 
                                "using a lambda and Console.Write inside: ");
            justInt.Visit((i, c) => { Console.Out.Write(i + "" + c + ", "); }, listMonadChar);
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();
            // Outputs: 1a, 1b, 1c, 1d, 1e,

            Console.WriteLine("Same with Nothing<int> will output nothing: ");
            nothingInt.Visit((i, c) => { Console.Out.Write(i + "" + c + ", "); }, listMonadChar);
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Visits each combination of the Just 5 and the ListMonad<int> \n" +
                                "using a lambda and Console.Write inside. Add both values in print out: ");
            justInt.Visit((x, y) => { Console.Out.Write( x + y + ", "); }, listMonadInt);

            Console.WriteLine("\nSame with Nothing<int>:");
            nothingInt = (Maybe<int>)nothingInt
                            .Visit((x, y) => { Console.Out.Write(x + y + ", "); }, listMonadInt);
            Console.WriteLine(nothingInt.ToString());
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();


            Console.Write("Fmap f(x) = x * 0.5 over the Just<int>(5): ");
            var justDouble = justInt.Fmap(intToDoubleFunction).Visit((x) => { Console.Out.Write(x + "\n"); });
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();

            Console.Write("App Just<Func>(f(x) = x * 0.5) over the Just<int>(5): ");
            justDouble = justInt.App(justFunction).Visit((x) => { Console.Out.Write(x + "\n"); });
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();

            Console.Write("App Just<Func> over the Just<int>(5), \n where the functions returns a new " + 
                            "ListMonad<int>() \n with two times the value inside the Just 5. Output: ");
            var function = new Just<Func<int, Monad<int>>>((x) => { return new ListMonad<int>(){x, x};});
            var intListMonad = justInt.App(function).Visit( (x) => { Console.Out.Write(x + ", "); } );
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();
            // The result is a ListMonad<int>
            // Output: 5, 5,

            Console.WriteLine("Create a new ListMonad with Func<int, int, double> (x*y, x/y, x%y) inside.");
            Console.WriteLine("Combinate Just 5 and that functions. Result is Just<int>.");
            Console.WriteLine("Only last value is returned because this Com function cannot break out of the Just.");
            Console.WriteLine();
            var functionListMonad = new ListMonad<Func<int, int, double>>();
            functionListMonad.Append( (x, y) => { return x*y;});
            functionListMonad.Append( (x, y) => { return x/ (y==0 ? 1 : y);});
            functionListMonad.Append((x, y) => { return x % (y == 0 ? 1 : y); });
            functionListMonad.Visit((x) => { Console.Out.WriteLine("Func: " + x); });
            var result = justInt.Com(functionListMonad, listMonadInt).Visit((x) => { Console.Out.Write(x + ", "); });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();
            // Output: 5

            Console.WriteLine("Create a new ListMonad with \n" +
                                "Func<int, int, IMonad<double>> (x+y, x-y, x*y, x/y, x%y) inside.\n" +
                                "Where every function packs the result in a new ListMonad<double>.\n" +
                                "Combine the Just 5 and the ListMonad<double> with all the functions.\n" +
                                "The result ListMonad´s are flattned out, and only one result ListMonad<double> \n"+
                                " with all result values is returned: ");
            Console.WriteLine();
            var functionListMonadTwo = new ListMonad<Func<int, double, Monad<double>>>();
            functionListMonadTwo.Append((x, y) => { return new ListMonad<double>() { x + y }; });
            functionListMonadTwo.Append((x, y) => { return new ListMonad<double>() { x - y }; });
            functionListMonadTwo.Append((x, y) => { return new ListMonad<double>(){x * y}; });
            functionListMonadTwo.Append((x, y) => { return new ListMonad<double>(){x / (y == 0 ? 1 : y)}; });
            functionListMonadTwo.Append((x, y) => { return new ListMonad<double>(){x % (y == 0 ? 1 : y)}; });
            functionListMonadTwo.Append((x, y) => { return new Nothing<double>(); });
            int counter = 0;
            var resultTwo = justInt.Com(functionListMonadTwo, listMonadDouble)
                            .Visit((x) => {
                                              Console.Out.Write(x + ", ");
                                              counter++;
                                              if (counter % 10 == 0)
                                                  Console.WriteLine("");
                                          });
            // Output: 5*0, 5*1, 5*2,... 5*1, 5/1, 5/2, 5/3, ... 5%1, 5%1, 5%2, 5%3,....
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Do the same with the Nothing<int>: ");
            resultTwo = nothingInt.Com(functionListMonadTwo, listMonadDouble)
                        .Visit((x) => { Console.Out.Write(x + ", "); });
            // Output: 5*0, 5*1, 5*2,... 5*1, 5/1, 5/2, 5/3, ... 5%1, 5%1, 5%2, 5%3,....
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Combinate Just 5 and the ListMonad<int> with only one function ( f(x,y) = x+y ): ");
            var resultThree = justInt.Com((x, y) => { return x + y; }, intListMonad)
                                .Visit((x) => { Console.Out.WriteLine(x); });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Maping a f(x, y) = x*y over the Just 5 and a new Just<int>(10) using LINQ: ");
            var query = from f in new Just<Func<int, int, int>>((x, y) => { return x * y; })
                        from x in justInt
                        from y in new Just<int>(10)
                        select f(x, y);

            query.Visit((x) => { Console.Out.WriteLine(x + ", "); });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();
        }

        public static void ListMonadPlayground()
        {
            Console.Out.WriteLine("\n-------------------------------------------------------------");
            Console.Out.WriteLine("------------------------ListMonad playground-------------------");
            Console.Out.WriteLine("-------------------------------------------------------------\n");

            Console.Out.WriteLine("Create two lists [1..5] and [J(1)..(J5)]: ");
            ListMonad<int> listMonadInt = new ListMonad<int>() 
                                            { 1, 2, 3, 4, 5 };

            ListMonad<double> listMonadDouble = new ListMonad<double>()
                                                {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};

            // Because Maybe class has an implicit operator it can be written very cool and easy like a normal list.
            ListMonad<Maybe<int>> listMonadMaybeInt = new ListMonad<Maybe<int>>() 
                                                        { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Functions for Fmap and first App function.
            Func<int, double> intDoubleFunc1 = (x) => { return 0.5 * x; };
            Func<int, double> intDoubleFunc2 = (x) => { return 0.7 * x; };

            Console.WriteLine("Fmap f(x) = 0.5 * x over [1,..5,]");
            int counter = 0;
            listMonadInt.Fmap(intDoubleFunc1).Visit((x) =>
                                                    {
                                                        Console.Out.Write(x + ", ");
                                                        counter++;
                                                        if (counter % 5 == 0)
                                                            Console.WriteLine("");
                                                    });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("App [f(x)=0.5*x, f(x)=0.7*x] over [1,..,5]");
            var listMonadintDoubleFunc = new ListMonad<Func<int, double>>() { intDoubleFunc1, intDoubleFunc2 };
            counter = 0;
            listMonadInt.App(listMonadintDoubleFunc).Visit((x) =>
                                                    {
                                                        Console.Out.Write(x + ", ");
                                                        counter++;
                                                        if (counter % 5 == 0)
                                                            Console.WriteLine("");
                                                        if (counter % (5*5) == 0)
                                                            Console.WriteLine("-----------------------------------------");
                                                    });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            // Functions for second App function.
            Func<int, Monad<double>> intIMonadIntDoubleFunc1 = 
                                        (x) => { return new Just<double>(x * x); };
            Func<int, Monad<double>> intIMonadIntDoubleFunc2 = 
                                        (x) => { return new Just<double>(x * x *x); };
            Func<int, Monad<double>> intIMonadIntDoubleFunc3 = 
                                        (x) => { return new Just<double>(x * x * x * x); };
            Func<int, Monad<double>> intIMonadIntDoubleFunc4 = 
                                        (x) => { return new Just<double>(x * x * x * x * x); };
            Func<int, Monad<double>> intIMonadIntDoubleFunc5 = 
                                        (x) => { return new ListMonad<double>(){x+1, x-1}; };

            var listMonadIMonadIntDoubleFunc = new ListMonad<Func<int, Monad<double>>>();
            listMonadIMonadIntDoubleFunc.Append(intIMonadIntDoubleFunc1);
            listMonadIMonadIntDoubleFunc.Append(intIMonadIntDoubleFunc2);
            listMonadIMonadIntDoubleFunc.Append(intIMonadIntDoubleFunc3);
            listMonadIMonadIntDoubleFunc.Append(intIMonadIntDoubleFunc4);
            listMonadIMonadIntDoubleFunc.Append(intIMonadIntDoubleFunc5);

            Console.WriteLine("App [Just(x^2), Just(x^3), Just(x^4), Just(x^5) ListMonad{x+1, x-1} over [1,..,5]");
            listMonadInt.App(listMonadIMonadIntDoubleFunc).Visit((x) =>
                                                                {
                                                                    Console.Out.Write(x + ", ");
                                                                    counter++;
                                                                    if (counter % 5 == 0)
                                                                        Console.WriteLine("");
                                                                });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            // Functions for combination 
            Func<int, double, double> intDoubleDoubleFunc1 = 
                                        (x, y) => { return (double)x + y; };
            Func<int, double, double> intDoubleDoubleFunc2 = 
                                        (x, y) => { return (double)x - y; };
            Func<int, double, double> intDoubleDoubleFunc3 = 
                                        (x, y) => { return (double)x * y; };
            Func<int, double, double> intDoubleDoubleFunc4 = 
                                        (x, y) => { return (double)x / y; };
            Func<int, double, double> intDoubleDoubleFunc5 = 
                                        (x, y) => { return (double)x % y; };

            var listMonadIntDoubleDoubleFunc = new ListMonad<Func<int, double, double>>()
                                                {intDoubleDoubleFunc1,
                                                intDoubleDoubleFunc2,
                                                intDoubleDoubleFunc3,
                                                intDoubleDoubleFunc4,
                                                intDoubleDoubleFunc5};

            Console.WriteLine("Combinate [1..5] and [1.0,..,9.0] with 'normal' result value and function +:");
            counter = 0;
            listMonadInt.Com(intDoubleDoubleFunc1, listMonadDouble)
                        .Visit((x) =>
                        {
                            Console.Out.Write(x + ", ");
                            counter++;
                            if (counter % 9 == 0)
                                Console.WriteLine("");
                        });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Combinate [1..5] and [1.0,..,9.0] 'normal' result value and functions [+, -, *, /, %]: ");
            counter = 0;
            listMonadInt.Com(listMonadIntDoubleDoubleFunc, listMonadDouble)
                            .Visit((x) =>
                            {
                                Console.Out.Write(x + ", ");
                                counter++;
                                if (counter % 9 == 0)
                                    Console.WriteLine("");
                                if (counter % (5 * 9) == 0)
                                    Console.WriteLine("------------------------------------------------");
                            });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            // Functions for combination with IMonad as result.
            Func<int, double, Monad<double>> intDoubleIMonadDoubleFunc1 = 
                (x, y) => { return new Just<double>((double)x + y); };

            Func<int, double, Monad<double>> intDoubleIMonadDoubleFunc2 = 
                (x, y) => { return new Just<double>((double)x - y); };

            Func<int, double, Monad<double>> intDoubleIMonadDoubleFunc3 = 
                (x, y) => { return new Just<double>((double)x * y); };

            Func<int, double, Monad<double>> intDoubleIMonadDoubleFunc4 = 
                (x, y) => { return new Just<double>((double)x / y); };

            Func<int, double, Monad<double>> intDoubleIMonadDoubleFunc5 = 
                (x, y) => { return new ListMonad<double>(){(double)x % y}; };

            Func<int, double, Monad<double>> intDoubleIMonadDoubleFunc6 = 
                (x, y) => { return new ListMonad<double>() { (double)x * y * y, (double) x * y * y * y }; };
            
            Func<int, double, Monad<double>> intDoubleIMonadDoubleFunc7 = 
                (x, y) => { return new Nothing<double>(); };

            var listMonadIntDoubleIMonadDoubleFunc = new ListMonad<Func<int, double, Monad<double>>>()
                                                    {intDoubleIMonadDoubleFunc1,
                                                    intDoubleIMonadDoubleFunc2,
                                                    intDoubleIMonadDoubleFunc3,
                                                    intDoubleIMonadDoubleFunc4,
                                                    intDoubleIMonadDoubleFunc5,
                                                    intDoubleIMonadDoubleFunc6,
                                                    intDoubleIMonadDoubleFunc7};

            Console.WriteLine("Combination with IMonad function results.");
            Console.WriteLine("List1[1,..,5], List2[1.0,..,9.0] and function +");
            counter = 0;
            listMonadInt.Com(intDoubleIMonadDoubleFunc1, listMonadDouble)
                            .Visit((x) =>
                            {
                                Console.Out.Write(x + ", ");
                                counter++;
                                if (counter % 9 == 0)
                                    Console.WriteLine("");
                                if (counter % (5 * 9) == 0)
                                    Console.WriteLine("------------------------------------------------");
                            });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Combination with IMonad function results.");
            Console.WriteLine("List1[1,..,5], List2[1.0,..,9.0] and " +
                                "functions [+, -, *, /, %, [x*y*y, x*y*y*y], Nothing]");
            counter = 0;
            listMonadInt.Com(listMonadIntDoubleIMonadDoubleFunc, listMonadDouble)
                        .Visit((x) =>
                        {
                            Console.Out.Write(x + ", ");
                            counter++;
                            if (counter % 9 == 0)
                                Console.WriteLine("");
                            if (counter % (5 * 9) == 0)
                                Console.WriteLine("------------------------------------------------");
                        });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            // Visit with other IMonad
            Console.WriteLine("Visit with other IMonad and add (+) values in output.");
            counter = 0;
            listMonadInt.Visit<double>((x, y) => 
                                       { 
                                           Console.Write(x * y + ", ");
                                           counter++;
                                           if (counter % 9 == 0)
                                               Console.WriteLine("");
                                       }, listMonadDouble);
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Function applying with Linq: \n" + 
                              " from f in [+, -, *, %]\n" +
                              " from x in [1,..,5]\n" +
                              " from y in [1.0,..,9.0] \n"+
                              " select f(x,y) \n");
            var query = from f in listMonadIntDoubleDoubleFunc
                        from x in listMonadInt
                        from y in listMonadDouble
                        select f(x, y);
            counter = 0;
            query.Visit((x) =>
                        {
                            Console.Out.Write(x + ", ");
                            counter++;
                            if (counter % 9 == 0)
                                Console.WriteLine("");
                            if (counter % (5 * 9) == 0)
                                Console.WriteLine("------------------------------------------------");
                        });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("Function applying with Linq: ");
            var query2 = from f in listMonadIntDoubleIMonadDoubleFunc
                            from x in listMonadInt
                            from y in listMonadDouble
                            select f(x, y);
            counter = 0;
            query2.Visit((x) =>
            {
                Console.Out.Write(x + ", ");
                counter++;
                if (counter % 9 == 0)
                    Console.WriteLine("");
                if (counter % (5 * 9) == 0)
                    Console.WriteLine("------------------------------------------------");
            });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

        }

        public static void ListMonadOperatorPlayground()
        {
            Console.Out.WriteLine("\n-------------------------------------------------------------");
            Console.Out.WriteLine("------------------------Operator playground-------------------");
            Console.Out.WriteLine("-------------------------------------------------------------\n");

            int counter = 0;

            Console.Out.WriteLine("Create two lists [0..9]: ");
            ListMonad<double> listMonadDouble = new ListMonad<double>() 
                                                { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0 };

            ListMonad<double> listMonadDoubleTwo = new ListMonad<double>() 
                                                    { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0 };

            // Functions for second App function.
            Func<double, Monad<double>> doubleIMonadDoubleFun1 =
                                        (x) => { return new Just<double>(x * x); };
            Func<double, Monad<double>> doubleIMonadDoubleFun2 =
                                        (x) => { return new Just<double>(x * x * x); };
            Func<double, Monad<double>> doubleIMonadDoubleFun3 =
                                        (x) => { return new Just<double>(x * x * x * x); };
            Func<double, Monad<double>> doubleIMonadDoubleFun4 =
                                        (x) => { return new Just<double>(x * x * x * x * x); };
            Func<double, Monad<double>> doubleIMonadDoubleFun5 =
                                        (x) => { return new ListMonad<double>() { x + 1, x - 1 }; };

            var listMonadFunc1 = new ListMonad<Func<double, Monad<double>>>();
            listMonadFunc1.Append(doubleIMonadDoubleFun1);
            listMonadFunc1.Append(doubleIMonadDoubleFun2);
            listMonadFunc1.Append(doubleIMonadDoubleFun3);
            listMonadFunc1.Append(doubleIMonadDoubleFun4);
            listMonadFunc1.Append(doubleIMonadDoubleFun5);

            // Functions for combination 
            Func<double, double, double> doubleDoubleDoubleFunc1 =
                                        (x, y) => { return (x + y); };
            Func<double, double, double> doubleDoubleDoubleFunc2 =
                                        (x, y) => { return x - y; };
            Func<double, double, double> doubleDoubleDoubleFunc3 =
                                        (x, y) => { return x * y; };
            Func<double, double, double> doubleDoubleDoubleFunc14 =
                                        (x, y) => { return x / y; };
            Func<double, double, double> doubleDoubleDoubleFunc5 =
                                        (x, y) => { return x % y; };

            var listMonadFunc2 = new ListMonad<Func<double, double, double>>()
                                                {doubleDoubleDoubleFunc1,
                                                doubleDoubleDoubleFunc2,
                                                doubleDoubleDoubleFunc3,
                                                doubleDoubleDoubleFunc14,
                                                doubleDoubleDoubleFunc5};


            // Functions for combination with IMonad as result.
            Func<double, double, Monad<double>> intDoubleIMonadDoubleFunc1 =
                (x, y) => { return new Just<double>(x + y); };

            Func<double, double, Monad<double>> intDoubleIMonadDoubleFunc2 =
                (x, y) => { return new Just<double>(x - y); };

            Func<double, double, Monad<double>> intDoubleIMonadDoubleFunc3 =
                (x, y) => { return new Just<double>(x * y); };

            Func<double, double, Monad<double>> intDoubleIMonadDoubleFunc4 =
                (x, y) => { return new Just<double>(x / y); };

            Func<double, double, Monad<double>> intDoubleIMonadDoubleFunc5 =
                (x, y) => { return new ListMonad<double>() { x % y }; };

            Func<double, double, Monad<double>> intDoubleIMonadDoubleFunc6 =
                (x, y) => { return new ListMonad<double>() { x * y * y, x * y * y * y }; };

            Func<double, double, Monad<double>> intDoubleIMonadDoubleFunc7 =
                (x, y) => { return new Nothing<double>(); };

            var listMonadFunc3 = new ListMonad<Func<double, double, Monad<double>>>()
                                                    {intDoubleIMonadDoubleFunc1,
                                                    intDoubleIMonadDoubleFunc2,
                                                    intDoubleIMonadDoubleFunc3,
                                                    intDoubleIMonadDoubleFunc4,
                                                    intDoubleIMonadDoubleFunc5,
                                                    intDoubleIMonadDoubleFunc6,
                                                    intDoubleIMonadDoubleFunc7};

            Console.WriteLine("fmap f(x) = x * 0.5 over [1,0..9.0] with \" / \" operator");

            var result = (listMonadDouble / ((x) => { return x * 0.5; })).Visit((x) =>
                                                                    {
                                                                        Console.Out.Write(x + ", ");
                                                                        counter++;
                                                                        if (counter % 9 == 0)
                                                                            Console.WriteLine("");
                                                                    });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.WriteLine("App functions [x^2, x^3, x^4, x^5, [x+1, x-1]] \n" +
                                " over [1,0..9.0] with \" * \" operator");

            var resultTwo = (listMonadDouble * listMonadFunc1).Visit((x) =>
                                                                    {
                                                                        Console.Out.Write(x + ", ");
                                                                        counter++;
                                                                        if (counter % 9 == 0)
                                                                            Console.WriteLine("");
                                                                    });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            // Create a tupel with the ListMonad with functions inside 
            // and with the other ListMonad with double values inside
            // because the \"*\" operator can have only one other argument.
            // it sad there are no way for custom operators in C#
            // and there are no operator that take tree arguments and can be overloaded.
            var funcMonadTupel = new Tuple<Monad<Func<double, double, double>>,
                                            Monad<double>
                                            >(listMonadFunc2, 
                                            listMonadDoubleTwo);

            counter = 0;
            Console.WriteLine("Combinate [1.0,..,9.0] with [1.0,..,9.0] and functions \n" +
                    " [x+y, x-y, x*y, x/y, x%y]");
            var resultThree = (listMonadDouble * funcMonadTupel)
                                .Visit((x) =>
                                {
                                    Console.Out.Write(x + ", ");
                                    counter++;
                                    if (counter % 9 == 0)
                                        Console.WriteLine("");

                                    if (counter % (9 * 9) == 0)
                                        Console.WriteLine("-------------------------------------------");
                                });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();


            var funcMonadTupelTwo = new Tuple<Monad<Func<double, double, Monad<double>>>,
                                            Monad<double>>
                                            (listMonadFunc3,
                                            listMonadDoubleTwo);

            Console.WriteLine("Combinate [1.0,..,9.0] with [1.0,..,9.0] and functions \n" +
                    " [x+y, x-y, x*y, x/y, x%y, [x*y^2, x*y^3]]:");
            var resultFour = (listMonadDouble * funcMonadTupelTwo)
                                .Visit((x) =>
                                {
                                    Console.Out.Write(x + ", ");
                                    counter++;
                                    if (counter % 9 == 0)
                                        Console.WriteLine("");

                                    if (counter % (9 * 9) == 0)
                                        Console.WriteLine("-------------------------------------------");
                                });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();


            Console.WriteLine("[1.0,..,9.0] + [1.0,..,9.0] + Just(1000.0) + Nothing \n" +
                                " Fmap -> App -> Com -> Com2nd -> Visit \n" + 
                                " This will take a while!! Are you ready, then press enter :-D :");
            Console.ReadLine();

            // Concat both double ListMonad´s to get a bigger list
            // and only to show that its possible 
            // concat a Just(1000.0) and a Nothing<double> to the result list too.
            var resultFive = (listMonadDouble + listMonadDoubleTwo)
                                .Append(new Just<double>(1000.0))
                                .Append(new Nothing<double>());

            var resultSix = (ListMonad<double>)resultFive;

            // This line is done the whole operatione!
            // Without one loop.
            resultSix = resultSix / ((x) => { return x * 100.0; }) * funcMonadTupel *funcMonadTupelTwo;

            resultSix.Visit((x) =>
                            {
                                Console.Out.Write(x + ", ");
                                counter++;
                                if (counter % 9 == 0)
                                    Console.WriteLine("");

                                if (counter % (9 * 9) == 0)
                                    Console.WriteLine("-------------------------------------------");
                            });
            Console.WriteLine("\n___________________________________________________________");

            Console.ReadLine();
        }

        public static void ListMonadLinqAndBindPlayground()
        {
            var bindResult1 = new ListMonad<string>(){"1. ", "2. "}.Fmap(a =>
                                "Hello World!".ToIdentity().Fmap(b =>
                                    (new DateTime(2010, 1, 11)).ToMaybe().Fmap(c =>
                                        a + ", " + b.ToString() + ", " + c.ToShortDateString()).Return()).Return());

            Console.WriteLine(bindResult1);
            Console.ReadLine();

            var bindResult2 = from str in new ListMonad<string>(){"1. ", "2. "}
                              from h in "Hello World!".ToIdentity()
                              from b in 7.ToIdentity()
                              from dt in (new DateTime(2010, 1, 11)).ToMaybe()
                              select str + h + " " + b.ToString() + " " + dt.ToShortDateString();

            Console.WriteLine(bindResult2);
            Console.ReadLine();

            var bindResult3 = " This is a bind test".ToIdentity().Bind(a =>
                                "Hello World!".ToIdentity().Bind( b =>
                                    (new DateTime(2010, 1, 11)).ToMaybe().Bind(c =>
                                        (a + ", " + b.ToString() + ", " + c.ToShortDateString()).ToIdentity())));

            Console.WriteLine(bindResult3);

            Console.ReadLine();

            Monad<string> idString = from a in "Hello World!".ToIdentity()
                                      from b in 7.ToIdentity()
                                      from c in (new DateTime(2010, 1, 11)).ToIdentity()
                                      select a + ", " + b.ToString() + ", " + c.ToShortDateString();

            Console.WriteLine(idString);
            Console.ReadLine();

            var listMInt = new ListMonad<int>() { 1, 2, 3, 4, 5 };
            var listMDbl = new ListMonad<double> { 1.5, 2.5, 3.5, 4.5, 5.5 };
            var listMChar = new ListMonad<Char>() { 'a', 'b', 'c', 'd', 'e' };
            
            // The query is a ListMonad<String> because the monad that calles select,
            // is a ListMonad (listMInt). 
            var listMString = from i in listMInt
                                from c in listMChar
                                select i + ":" + c;

            Console.WriteLine(listMString);
            Console.ReadLine();

            var result3 = from i in listMInt
                          from d in listMDbl
                          from c in listMChar
                          select c + ") " + Math.Pow(i, d) + ", ";

            Console.WriteLine(result3);
            Console.ReadLine();

            var result4 = from i in listMInt
                          from d in listMDbl
                          from c in listMChar
                          select (c + ") " + Math.Pow(i, d) + ", ").ToIdentity();

            Console.WriteLine(result4);
            Console.ReadLine();

            var result5 = from i in listMInt
                          from d in listMDbl
                          select i + ") " + Math.Pow(i, d) + ", ";

            Console.WriteLine(result5);
            Console.ReadLine();

            var result6 = listMInt.Com((i, d) => (i + ") " + Math.Pow(i, d) + ", ").ToIdentity(), listMDbl);
            Console.WriteLine(result6);
            Console.ReadLine();

            var result7 = from i in listMInt
                          where i % 2 == 0
                          from d in new ListMonad<double>(){1.0, 2.0, 3.0, 4.0, 5.0}
                          where d % 2 == 0
                          select i + "^" + d + " = " + Math.Pow(i, d) + ", ";

            Console.WriteLine(result7);
            Console.ReadLine();

        }

        public static void ExtensionPlayGround()
        {
            Identity<int> x = 5;
            Identity<int> y = 10;

            Func<int, int, int> func = (a, b) => { return a + b; };

            // lift the function into the monad. so it will have the signature
            // Func<IMonad<int>, IMonad<int>, IMonad<int>> 
            // use function with the two Identity<int>
            // result will be Identity<int> again
            var result = x.LiftM<int>(func)(x, y);

            Console.WriteLine("Result of Identity(5) + Identity(10) using LiftM: ");
            Console.WriteLine(" = " + result.Return());
            Console.ReadLine();

            bool equals = x.Any(y, (a, b) => { return a == b; });
            Console.WriteLine("\nIdentity<5> equals Identity(10) ? = " + equals);
            Console.ReadLine();

            // The cast to int of argument b is not really neccessary
            equals = x.Contains<int, double>(5.0, (a, b) => {return a == (int)b;});
            Console.WriteLine("\nIdentity<5> equals 5.0 ? = " + equals);
            Console.ReadLine();

        }


        public static Func<string, int> func = (s) => { return s.Length; };
        public static Func<Monad<string>, Monad<int>> monadFunc = (m) => { return m.Fmap(func); };
        public static Func<Monad<string>, Identity<string>> copyFunc = (id) => { return new Identity<string>(id.Return()); };
        public static Func<string, string> setString = (s) => s += "foobar ";

        public static void ObservableAndThreadSafeMethodsPlayGround()
        {
            Identity<string> id = "string";
            Identity<string> observer = "";
            observer.Disposable = id.Subscribe(observer);
            // or observer.SubscribeAt(id);

            observer.NextAction = (m, x) => Console.WriteLine("Received next: " + x);
            id.Subscribe((s) => Console.WriteLine("Anonymour Observer Received next: " + s),
                         (e) => Console.WriteLine(e.Message),
                         () => Console.WriteLine("Completed"));

            Console.WriteLine("Setting to string 1 with ActionW((i) => i.Pure(\"1\"))");
            id.ActionW((i) => i.Pure("1"));
            Console.ReadLine();

            Console.WriteLine("Setting to string 2 with ActionW(() => id.Pure(\"1\"))");
            id.ActionW(() => id.Pure("2"));
            Console.ReadLine();

            // If the id monad is "here" (inside the same scope) we can use it directly inside the lambda.

            Console.WriteLine("MethodW2(() => { return id.Fmap((s) => s.Length); })");
            var result = id.MethodW2(() => { return id.Fmap((s) => s.Length); });
            Console.WriteLine("Result: " + result);
            Console.ReadLine();

            Console.WriteLine("Adding string foobar with via fmap and setString function. ");
            // Fmap returns a new monad because it cannot know that the returned type of the function 
            // has the same type. Pure sets the value inside the Id to the new string.
            id.MethodW(() => { return id.Pure(id.Fmap(setString).Return()); });
            Console.ReadLine();

            Console.WriteLine("Adding string foobar with via fmap and setString function. ");
            id.MethodW((i) => { return i.Pure(i.Fmap(setString).Return()); });
            Console.ReadLine();

            Console.WriteLine("Adding string foobar: ");
            id.MethodW(() => { return id.Pure(id.Fmap(setString).Return()); });
            Console.ReadLine();

            Console.WriteLine("Adding string foobar: ");
            // ActionW always returns the monad it is used on.
            // MethodW returns the type Imonad of the given lambda. So it can be a different monad.
            // Here its doing the same.
            id.ActionW(() => id.Pure(id.Fmap(setString).Return()));
            Console.ReadLine();

            Console.WriteLine("Adding string foobar: ");
            id.ActionW((i) => i.Pure(i.Fmap(setString).Return()));
            Console.ReadLine();


            // Will return new Identity with = operator
            //id.MethodW(() => { return id = (Identity<string>)id.Fmap(setString) ; });
            //id.MethodW(() => { return id = id.Fmap(setString).Return(); });

            id.MethodW2(() => { return id.Fmap(func); });
            id.MethodW2((m) => { return m.Fmap(func); });
            id.MethodW2(monadFunc);

            // Only to show its possible.
            // It simply returns the monad again.
            //id.MethodW(() => id, false);
            //id.MethodW((m) => m, false);

            id.ActionW((i) => i.Pure("Clear"));
            Console.WriteLine("Id value=" + id);
            Console.ReadLine();

            id.MethodW((m) => { return m.Pure("foobar"); });
            Console.ReadLine();

            //id.MethodW((str) => { str += "foobar"; return new Identity<string>(str); });
            id.MethodW((m) => { return m.Pure(m.Return() + "foobar"); });
            Console.ReadLine();

            id.MethodW(() => { return new Identity<string>(id.Return()); }, false);
            id.MethodW((m) => { return new Identity<string>(m.Return()); }, false);
            id.MethodW(copyFunc, false);       // If copyFunc is Func<Identity<string>, Identity<string>>, this wont work.
            //Identity<string> idCopy = (Identity<string>)id.MethodW(copyFunc);
            Identity<string> idCopy = id.MethodW2(copyFunc, false).Return().ToIdentity();

            id.EndTransmission();
            Console.ReadLine();
        }


    }
}
