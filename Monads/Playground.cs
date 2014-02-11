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

            var intToDoubleFunctin = new Func<int, double>(x => { return x * 0.5; });
            Console.WriteLine("A new Just with a function inside: f(x) = x * 0.5 ");
            var justFunction = new Just<Func<int, double>>(intToDoubleFunctin);
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
            var justDouble = justInt.Fmap(intToDoubleFunctin).Visit((x) => { Console.Out.Write(x + "\n"); });
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();

            Console.Write("App Just<Func>(f(x) = x * 0.5) over the Just<int>(5): ");
            justDouble = justInt.App(justFunction).Visit((x) => { Console.Out.Write(x + "\n"); });
            Console.WriteLine("___________________________________________________________");
            Console.ReadLine();

            Console.Write("App Just<Func> over the Just<int>(5), \n where the functions returns a new " + 
                            "ListMonad<int>() \n with two times the value inside the Just 5. Output: ");
            var function = new Just<Func<int, IMonad<int>>>((x) => { return new ListMonad<int>(){x, x};});
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
            var functionListMonadTwo = new ListMonad<Func<int, double, IMonad<double>>>();
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

            Console.Write("Combinate Just 5 and the ListMonad<int> with only one function ( f(x,y) = x+y ): ");
            var resultThree = justInt.Com((x, y) => { return x + y; }, intListMonad)
                                .Visit((x) => { Console.Out.Write(x); });
            Console.WriteLine("\n___________________________________________________________");
            Console.ReadLine();

            Console.Write("Maping a f(x, y) = x*y over the Just 5 and a new Just<int>(10) using LINQ: ");
            var query = from f in new Just<Func<int, int, int>>((x, y) => { return x * y; })
                        from x in justInt
                        from y in new Just<int>(10)
                        select f(x, y);

            query.Visit((x) => { Console.Out.Write(x + ", "); });
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
            Func<int, double> intDoubleFunc1 = (x) => { return 0.5 * (double)x; };
            Func<int, double> intDoubleFunc2 = (x) => { return 0.7 * (double)x; };

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
            Func<int, IMonad<double>> intIMonadIntDoubleFunc1 = 
                                        (x) => { return new Just<double>(x * x); };
            Func<int, IMonad<double>> intIMonadIntDoubleFunc2 = 
                                        (x) => { return new Just<double>(x * x *x); };
            Func<int, IMonad<double>> intIMonadIntDoubleFunc3 = 
                                        (x) => { return new Just<double>(x * x * x * x); };
            Func<int, IMonad<double>> intIMonadIntDoubleFunc4 = 
                                        (x) => { return new Just<double>(x * x * x * x * x); };
            Func<int, IMonad<double>> intIMonadIntDoubleFunc5 = 
                                        (x) => { return new ListMonad<double>(){x+1, x-1}; };

            var listMonadIMonadIntDoubleFunc = new ListMonad<Func<int, IMonad<double>>>();
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
            Func<int, double, IMonad<double>> intDoubleIMonadDoubleFunc1 = 
                (x, y) => { return new Just<double>((double)x + y); };

            Func<int, double, IMonad<double>> intDoubleIMonadDoubleFunc2 = 
                (x, y) => { return new Just<double>((double)x - y); };

            Func<int, double, IMonad<double>> intDoubleIMonadDoubleFunc3 = 
                (x, y) => { return new Just<double>((double)x * y); };

            Func<int, double, IMonad<double>> intDoubleIMonadDoubleFunc4 = 
                (x, y) => { return new Just<double>((double)x / y); };

            Func<int, double, IMonad<double>> intDoubleIMonadDoubleFunc5 = 
                (x, y) => { return new ListMonad<double>(){(double)x % y}; };

            Func<int, double, IMonad<double>> intDoubleIMonadDoubleFunc6 = 
                (x, y) => { return new ListMonad<double>() { (double)x * y * y, (double) x * y * y * y }; };
            
            Func<int, double, IMonad<double>> intDoubleIMonadDoubleFunc7 = 
                (x, y) => { return new Nothing<double>(); };

            var listMonadIntDoubleIMonadDoubleFunc = new ListMonad<Func<int, double, IMonad<double>>>()
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
            Func<double, IMonad<double>> doubleIMonadDoubleFun1 =
                                        (x) => { return new Just<double>(x * x); };
            Func<double, IMonad<double>> doubleIMonadDoubleFun2 =
                                        (x) => { return new Just<double>(x * x * x); };
            Func<double, IMonad<double>> doubleIMonadDoubleFun3 =
                                        (x) => { return new Just<double>(x * x * x * x); };
            Func<double, IMonad<double>> doubleIMonadDoubleFun4 =
                                        (x) => { return new Just<double>(x * x * x * x * x); };
            Func<double, IMonad<double>> doubleIMonadDoubleFun5 =
                                        (x) => { return new ListMonad<double>() { x + 1, x - 1 }; };

            var listMonadFunc1 = new ListMonad<Func<double, IMonad<double>>>();
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
            Func<double, double, IMonad<double>> intDoubleIMonadDoubleFunc1 =
                (x, y) => { return new Just<double>(x + y); };

            Func<double, double, IMonad<double>> intDoubleIMonadDoubleFunc2 =
                (x, y) => { return new Just<double>(x - y); };

            Func<double, double, IMonad<double>> intDoubleIMonadDoubleFunc3 =
                (x, y) => { return new Just<double>(x * y); };

            Func<double, double, IMonad<double>> intDoubleIMonadDoubleFunc4 =
                (x, y) => { return new Just<double>(x / y); };

            Func<double, double, IMonad<double>> intDoubleIMonadDoubleFunc5 =
                (x, y) => { return new ListMonad<double>() { x % y }; };

            Func<double, double, IMonad<double>> intDoubleIMonadDoubleFunc6 =
                (x, y) => { return new ListMonad<double>() { x * y * y, x * y * y * y }; };

            Func<double, double, IMonad<double>> intDoubleIMonadDoubleFunc7 =
                (x, y) => { return new Nothing<double>(); };

            var listMonadFunc3 = new ListMonad<Func<double, double, IMonad<double>>>()
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
            var funcMonadTupel = new Tuple<IMonad<Func<double, double, double>>,
                                            IMonad<double>
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


            var funcMonadTupelTwo = new Tuple<IMonad<Func<double, double, IMonad<double>>>,
                                            IMonad<double>>
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
            //resultSix = resultSix / ((x) => { return x * 100.0; }) * funcMonadTupel *funcMonadTupelTwo;

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

            var bindResult = "Hello World!".ToIdentity().Bind(                                                  a =>
                                " This is a bind test".ToIdentity().Bind(                                       b =>
                                    (new DateTime(2010, 1, 11)).ToMaybe().Bind(                                 c =>
                                        (a + ", " + b.ToString() + ", " + c.ToShortDateString()).ToIdentity())));

            Console.WriteLine(bindResult.Return());

            Console.ReadLine();
        }

        public static void ListMonadBindTest()
        {
            var bindResult = "Hello World!".ToIdentity().Bind(a =>
                                " This is a bind test".ToIdentity().Bind(b =>
                                    (new DateTime(2010, 1, 11)).ToMaybe().Bind(c =>
                                        (a + ", " + b.ToString() + ", " + c.ToShortDateString()).ToIdentity())));

            Console.WriteLine(bindResult.Return());

            Console.ReadLine();

            var result =    from a in "Hello World!".ToIdentity()
                            from b in 7.ToIdentity()
                            from c in (new DateTime(2010, 1, 11)).ToIdentity()
                            select a + ", " + b.ToString() + ", " + c.ToShortDateString();

            Console.WriteLine(result.Return());

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
    }
}
