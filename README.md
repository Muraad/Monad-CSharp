Monad-CSharp
============

Monad like programming with C#.

An abstract base class IMonad that extends IEnumerable, IObservable and IObserver
Some implementations of the IMonad:
- Maybe
- Identity
- List monad
- (Either is mostly untested, some kind of decorator) (Update: currently not working!)

Lots of extension methods (LiftMonad, Comparator functions, easy thread safe actions).

See Doc folder for detailed description an introduction.

Have a look at the Playground.cs for more examples.

A little intro how to use it:


    Maybe<int> justInt = 5;
    Maybe<int> nothingInt = 0;
    
    var intToDoubleFunction = new Func<int, double>(x => { return x * 0.5; });
    Just<double> justDouble = justInt.Fmap(intToDoubleFunction);
  
  
:

    ListMonad<int> listMonadInt = new ListMonad<int>() { 1, 2, 3, 4, 5 };
    ListMonad<double> listMonadDouble = new ListMonad<double>() {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
    listMonadInt.Fmap((x) => { return 0.5 * x;});
  
  
    Func<int, double> intDoubleFunc1 = (x) => { return 0.5 * x; };
    Func<int, double> intDoubleFunc2 = (x) => { return 0.7 * x; };
    var lmIntDblFunc = new ListMonad<Func<int, double>>() { intDoubleFunc1, intDoubleFunc2 };
    ListMonad<Double> result = listMonadInt.App(lmIntDblFunc);

  
:
    
    Identity<string> id = "string";
    Identity<string> observer = "";
    observer.Disposable = id.Subscribe(observer);
    // or observer.SubscribeAt(id);

    observer.NextAction = (m, x) => Console.WriteLine("Received next: " + x);
    observer.CompleteAction = (m) => Console.WriteLine("Completed, last value: " + m.Return();
    observer.ErrorAction = (e) => Console.WriteLine(e.Message);
    
    // Using Subscribe from reactive extensions
    id.Subscribe((s) => Console.WriteLine("Anonymour Observer Received next: " + s),
                 (e) => Console.WriteLine(e.Message),
                 () => Console.WriteLine("Completed"));

    // Both lines will change the value inside the id monad.
    // The observer monads OnNext will be called.
    
    id.ActionW((i) => i.Pure("1"));
    id.ActionW(() => id.Pure("2"));
         
    public static Func<string, string> setString = (s) => s += "foobar ";
    
    // Adding the string foobar to the current string inside the identity two times,
    // in different ways.
    id.MethodW(() => { return id.Pure(id.Fmap(setString).Return()); });
    id.ActionW(() => id.Pure(id.Fmap(setString).Return()));
            
    id.EndTransmission();   
            
:

    // Functions for combination with IMonad as result.
    Func<int, double, IMonad<double>> intDblIMonadDblF1 = 
        (x, y) => { return new Just<double>((double)x + y); };
  
    Func<int, double, IMonad<double>> intDblIMonadDblF2 = 
        (x, y) => { return new Just<double>((double)x - y); };
  
    Func<int, double, IMonad<double>> intDblIMonadDblF3 = 
        (x, y) => { return new Just<double>((double)x * y); };
  
    Func<int, double, IMonad<double>> intDblIMonadDblF4 = 
        (x, y) => { return new Just<double>((double)x / y); };
  
    Func<int, double, IMonad<double>> intDblIMonadDblF5 = 
        (x, y) => { return new ListMonad<double>(){(double)x % y}; };
  
    Func<int, double, IMonad<double>> intDblIMonadDblF6 = 
        (x, y) => { return new ListMonad<double>() { (double)x * y * y, (double) x * y * y * y }; };
    
    Func<int, double, IMonad<double>> intDblIMonadDblF7 = 
        (x, y) => { return new Nothing<double>(); };
  
    var listMonadIntDblIMonadDblFunc = new ListMonad<Func<int, double, IMonad<double>>>()
                                            {intDblIMonadDblF1,
                                            intDblIMonadDblF2,
                                            intDblIMonadDblF3,
                                            intDblIMonadDblF4,
                                            intDblIMonadDblF5,
                                            intDblIMonadDblF6,
                                            intDblIMonadDblF7};
                       
    ListMonad<int> listMonadInt = new ListMonad<int>() { 1, 2, 3, 4, 5 };
    ListMonad<double> listMonadDouble = new ListMonad<double>() {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
  
  
    Console.WriteLine("Function applying with Linq: \n" + 
                                " from f in [+, -, *, %]\n" +
                                " from x in [1,..,5]\n" +
                                " from y in [1.0,..,9.0] \n"+
                                " select f(x,y) \n");
                              
    var query = from f in listMonadIntDblIMonadDblFunc
                from x in listMonadInt
                from y in listMonadDouble
                select f(x, y);
    counter = 0;
    // Query is a IMonad again
    query.Visit((x) =>
                {
                    Console.Out.Write(x + ", ");
                    counter++;
                    if (counter % 9 == 0)
                        Console.WriteLine("");
                    if (counter % (5 * 9) == 0)
                        Console.WriteLine("------------------------------------------------");
                          });
