###[Main](Index.html)_____________________________________________________________________________[Next: Linq action](LinqTest.html)
--------------
#Monads in action

## Maybe

### Basics and Visit

    // Just 5, use implicit operator for *Maybe* to make a Just directly.
    Maybe<int> justInt = 5;
    Console.WriteLine("A new Just<double>: " + justInt.ToString());
    Maybe<int> nothingInt = 0;      // same as nothingInt = new Nothing<int>();
    Console.WriteLine("A new Nothing<double>: " + nothingInt.ToString());

    // justInt = 0; or justInt = new Nothing<int>() would make a Nothing out of the justInt

    Console.WriteLine("A new ListMonad<char>: ");
    var listMonadChar = new ListMonad<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g' }
										 .Visit((x) => { Console.Out.Write(x + ", "); });
    Console.WriteLine("\n");

    Console.WriteLine("A new ListMonad<int>: ");
    var listMonadInt = new ListMonad<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
									     .Visit((x) => { Console.Out.Write(x + ", "); });
    Console.WriteLine("\n");

    Console.WriteLine("A new ListMonad<double>: ");
    var listMonadDouble = new ListMonad<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
										 .Visit((x) => { Console.Out.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();

    var intToDoubleFunctin = new Func<int, double>(x => { return x * 0.5; });
    Console.WriteLine("A new Just with a function inside: f(x) = x * 0.5 ");
    var justFunction = new Just<Func<int, double>>(intToDoubleFunctin);
    Console.WriteLine(justFunction.ToString());
    Console.WriteLine("\n");
    Console.ReadLine();

    Console.WriteLine("Visits each combination of the Just 5 and the ListMonad<char>");
    Console.WriteLine("using a lambda and Console.Write inside: ");
    justInt.Visit((i, c) => { Console.Out.Write(i + "" + c + ", "); }, listMonadChar);
    Console.WriteLine("\n");
    Console.ReadLine();
    // Outputs: 1a, 1b, 1c, 1d, 1e,

    Console.WriteLine("Same with Nothing<int> will output nothing: ");
    nothingInt.Visit((i, c) => { Console.Out.Write(i + "" + c + ", "); }, listMonadChar);
    Console.WriteLine("\n");
    Console.ReadLine();

    Console.WriteLine("Visits each combination of the Just 5 and the ListMonad<int>");
	Console.WriteLine("using a lambda and Console.Write inside.")
	Console.WriteLine("Add both values in print out: ");	
    justInt.Visit((x, y) => { Console.Out.Write( x + y + ", "); }, listMonadInt);

    Console.WriteLine("\nSame with Nothing<int>:");
    nothingInt = (Maybe<int>)nothingInt.Visit((x, y) => { Console.Out.Write(x + y + ", "); }, listMonadInt);
    Console.WriteLine(nothingInt.ToString());
    Console.WriteLine("\n");
    Console.ReadLine();

### Fmap

    Console.WriteLine("Fmap f(x) = x * 0.5 over the Just<int>(5).");
    var justDouble = justInt.Fmap(intToDoubleFunctin)
                            .Visit((x) => { Console.Out.Write(x + "\n"); });
    Console.WriteLine("\n");
    Console.ReadLine();

### App 1st

    Console.WriteLine("App Just<Func>(f(x) = x * 0.5) over the Just<int>(5).");
    justDouble = justInt.App(justFunction).Visit((x) => { Console.Out.Write(x + "\n"); });
    Console.WriteLine("\n");
    Console.ReadLine();

### App 2nd, return ListMonad

    Console.WriteLine("App Just<Func> over the Just<int>(5),");
    Console.WriteLine("where the functions returns a new ListMonad<int>()");
    Console.WriteLine("with two times the value inside the Just 5.");
    var function = new Just<Func<int, IMonad<int>>>((x) => { return new ListMonad<int>(){x, x};});

    var intListMonad = justInt.App(function)
                              .Visit( (x) => { Console.Out.Write(x + ", "); } );
    Console.WriteLine("\n");
    Console.ReadLine();
    // The result is a ListMonad<int>


### Combinate 

    Console.WriteLine("Create a new ListMonad with Func<int, int, double> (x*y, x/y, x%y) inside.");
    Console.WriteLine("Combinate Just 5 and that functions. Result is Just<int>.");
    Console.WriteLine("Only last value is returned because this Com function cannot break out of the Just.");
    Console.WriteLine();

    var functionListMonad = new ListMonad<Func<int, int, double>>();
    functionListMonad.Add( (x, y) => { return x*y;});
    functionListMonad.Add( (x, y) => { return x/ (y==0 ? 1 : y);});
    functionListMonad.Add((x, y) => { return x % (y == 0 ? 1 : y); });
    functionListMonad.Visit((x) => { Console.Out.WriteLine("Func: " + x); });
    var result = justInt.Com(functionListMonad, listMonadInt).Visit((x) => { Console.Out.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();


### Com with other *IMonad* and functions that returns *ListMonad´s*

    Console.WriteLine("Create a new ListMonad with Func<int, int, IMonad<double>> (x+y, x-y, x*y, x/y, x%y) inside.");
    Console.WriteLine("Where every function packs the result in a new ListMonad<double>.");
    Console.WriteLine("Combine the Just 5 and the ListMonad<double> with all the functions.");
    Console.WriteLine("The result ListMonad´s are flattned out");
    Console.WriteLine("and only one result ListMonad<double> with all result values is returned: ");
    Console.WriteLine();

    var functionListMonadTwo = new ListMonad<Func<int, double, IMonad<double>>>();
    functionListMonadTwo.Add((x, y) => { return new ListMonad<double>() { x + y }; });
    functionListMonadTwo.Add((x, y) => { return new ListMonad<double>() { x - y }; });
    functionListMonadTwo.Add((x, y) => { return new ListMonad<double>(){x * y}; });
    functionListMonadTwo.Add((x, y) => { return new ListMonad<double>(){x / (y == 0 ? 1 : y)}; });
    functionListMonadTwo.Add((x, y) => { return new ListMonad<double>(){x % (y == 0 ? 1 : y)}; });
    functionListMonadTwo.Add((x, y) => { return new Nothing<double>(); });

    var resultTwo = justInt.Com(functionListMonadTwo, listMonadDouble).Visit((x) => { Console.Out.Write(x + ", "); });
    // Output: 5*0, 5*1, 5*2,... 5*1, 5/1, 5/2, 5/3, ... 5%1, 5%1, 5%2, 5%3,....
    Console.WriteLine("\n");
    Console.ReadLine();

    Console.WriteLine("Do the same with the Nothing<int>: ");
    resultTwo = nothingInt.Com(functionListMonadTwo, listMonadDouble).Visit((x) => { Console.Out.Write(x + ", "); });
    // Output: 5*0, 5*1, 5*2,... 5*1, 5/1, 5/2, 5/3, ... 5%1, 5%1, 5%2, 5%3,....
    Console.WriteLine("\n");
    Console.ReadLine();


    Console.WriteLine("Combinate Just 5 and the ListMonad<int>, ")
	Console.WriteLine("with only one function ( f(x,y) = x+y ): ");
    Console.WriteLine();

    var resultThree = justInt.Com((x, y) => { return x + y; }, intListMonad).Visit((x) => { Console.Out.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();


    Console.WriteLine("Maping a f(x, y) = x*y over the Just 5 and a new Just<int>(10) using LINQ:");
    var query = from f in new Just<Func<int, int, int>>((x, y) => { return x * y; })
                from x in justInt
                from y in new Just<int>(10)
                select f(x, y);

    query.Visit((x) => { Console.Out.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();

---------------
## ListMonad

	ListMonad<int> listMonadInt = new ListMonad<int>() { 1, 2, 3, 4, 5 };
    ListMonad<double> listMonadDouble = new ListMonad<double>(){1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
    // Because Maybe class has an implicit operator it can be written very cool and easy like a normal list.
    ListMonad<Maybe<int>> listMonadMaybeInt = new ListMonad<Maybe<int>>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

### Fmap and App 1st

	// Functions for Fmap and first App function.
    Func<int, double> intDoubleFunc1 = (x) => { return 0.5 * (double)x; };
    Func<int, double> intDoubleFunc2 = (x) => { return 0.7 * (double)x; };

	Console.WriteLine("Fmap f(x) = 0.5 * x over [1,..5,]");
    listMonadInt.Fmap(intDoubleFunc1).Visit((x) => { Console.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();

	Console.WriteLine("App [f(x)=0.5*x, f(x)=0.7*x] over [1,..,5]");
	var listMonadintDoubleFunc = new ListMonad<Func<int, double>>() { intDoubleFunc1, intDoubleFunc2 };
	listMonadInt.App(listMonadintDoubleFunc).Visit((x) => { Console.Write(x + ", "); });
	Console.WriteLine("\n");
	Console.ReadLine();

### App 2nd

	// Functions for second App function.
    Func<int, IMonad<double>> intIMonadIntDoubleFunc1 = (x) => { return new Just<double>(x * x); };
    Func<int, IMonad<double>> intIMonadIntDoubleFunc2 = (x) => { return new Just<double>(x * x *x); };
    Func<int, IMonad<double>> intIMonadIntDoubleFunc3 = (x) => { return new Just<double>(x * x * x * x); };
    Func<int, IMonad<double>> intIMonadIntDoubleFunc4 = (x) => { return new Just<double>(x * x * x * x * x); };
    Func<int, IMonad<double>> intIMonadIntDoubleFunc5 = (x) => { return new ListMonad<double>(){x+1, x-1}; };

    var listMonadIMonadIntDoubleFunc = new ListMonad<Func<int, IMonad<double>>>();
    listMonadIMonadIntDoubleFunc.Add(intIMonadIntDoubleFunc1);
    listMonadIMonadIntDoubleFunc.Add(intIMonadIntDoubleFunc2);
    listMonadIMonadIntDoubleFunc.Add(intIMonadIntDoubleFunc3);
    listMonadIMonadIntDoubleFunc.Add(intIMonadIntDoubleFunc4);
    listMonadIMonadIntDoubleFunc.Add(intIMonadIntDoubleFunc5);

	Console.WriteLine("App [Just(x^2), Just(x^3), Just(x^4), Just(x^5] over [1,..,5]");
    listMonadInt.App(listMonadIMonadIntDoubleFunc).Visit((x) => { Console.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();

### Combinate with normal result type

	// Functions for combination 
    Func<int, double, double> intDoubleDoubleFunc1 = (x, y) => { return (double)x + y; };
    Func<int, double, double> intDoubleDoubleFunc2 = (x, y) => { return (double)x - y; };
    Func<int, double, double> intDoubleDoubleFunc3 = (x, y) => { return (double)x * y; };
    Func<int, double, double> intDoubleDoubleFunc4 = (x, y) => { return (double)x / y; };
    Func<int, double, double> intDoubleDoubleFunc5 = (x, y) => { return (double)x % y; };

    var listMonadIntDoubleDoubleFunc = new ListMonad<Func<int, double, double>>(){intDoubleDoubleFunc1,
                                                                               intDoubleDoubleFunc2,
                                                                               intDoubleDoubleFunc3,
                                                                               intDoubleDoubleFunc4,
                                                                               intDoubleDoubleFunc5};

	Console.WriteLine("Combination with 'normal' result value and function +:");
    listMonadInt.Com(intDoubleDoubleFunc1, listMonadDouble).Visit((x) => { Console.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();

	Console.WriteLine("Combination with 'normal' result value and functions [+, -, *, /, %]: ");
    listMonadInt.Com(listMonadIntDoubleDoubleFunc, listMonadDouble).Visit((x) => { Console.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();

### Combinate with *IMonad* result type

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
    listMonadInt.Com(intDoubleIMonadDoubleFunc1, listMonadDouble);
    Console.WriteLine("\n");
    Console.ReadLine();
    
	Console.WriteLine("Combination with IMonad function results.");
    Console.WriteLine("List1[1,..,5], List2[1.0,..,9.0] and functions [+, -, *, /, %, [x*y*y, x*y*y*y], Nothing]");
    listMonadInt.Com(listMonadIntDoubleIMonadDoubleFunc, listMonadDouble);
    Console.WriteLine("\n");
    Console.ReadLine();

### Visit with other *IMonad*

	Console.WriteLine("Visit with other IMonad and add (+) values in output.");
    listMonadInt.Visit<double>((x, y) => { Console.Write(x * y + ", "); }, listMonadDouble);
    Console.WriteLine("\n");
    Console.ReadLine();

### Linq

	Console.WriteLine("Function applying with Linq: ");
    var query = from f in listMonadIntDoubleDoubleFunc
                from x in listMonadInt
                from y in listMonadDouble
                select f(x, y);
    query.Visit((x) => { Console.Write(x + ", "); });
    Console.WriteLine("\n");
    Console.ReadLine();