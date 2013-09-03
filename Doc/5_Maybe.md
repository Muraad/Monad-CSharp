--------------------
## Maybe monade

### What is the *Maybe* type. 

A maybe is a type that can have one value inside, or it can have nothing.

For example a maybe with generic type double, can have one double value inside or it can have nothing inside. Nothing means its undefiend. 

Basically its like the *Nullable* in C#. Or generally to clearly have an undefiend value, or for basic types, to make them be able to have an undef. state.

A double cannt be *null*. 

And you have some rules for maybes. For example if you have two maybe´s and a function that does some action with both of them. If one of them is *Nothing* then the result will always be nothing again. 
For example:
	
	Nothing(double) + Just(5.0) = Nothing(double)

It not that difficult at all. But you will see there are some problems i came up with. 
And i had to make my own *definitions*.
One think to mention here is the question, what to do if someone ask a Maybe for its value with the function *Return()* from *IMonad* and it is nothing? 
You cannt say "Hey i dont give you a value" if the type definition of the *A Return()* function requires it.

Now lets start with some class definitions. 

----------------
### Implementation background
My first *Maybe* implementation had a abstract base class *AMaybeValue* that was basically a container (box) for a single value.
Then i had two classe *Just* and *Nothing* that extended this abstract class. 
And my maybe class was storing only a *AMaybeValue* and used *is* to check if it is nothing.

I´v defined implicit operators (both directions) for *Just/Nothing* and for *Maybe*. 
For the last one *to/from AMaybeValue* and *to/from A* (A generic value inside). 
This made the usage of the *Maybe* class very nice and easy. 

When i started to overload the *implicit operator* for *Maybe*, i made the *IMaybeValue* to *AMaybeValue*. Then i was able to overload the operators for *Just* and *Nothing* at the same time.

Here i was doing something thats normally no good programming style.
Inside the implicit operator for *AMaybeValue* i was returning a *Just* or *Nothing* directly, based on the input A value is *null* or *default(A)*.
That means the base class *AMaybeValue* knows its upper class. Its some kind of *Cycle*. Because of the small and easy classes and the great usage i thought its ok this time.

Then i realiezed that i can make things even more easy and get rid of the *AMaybeValue* completely.

I put the *AMaybeValue* and the *Maybe* (implements *IMonad*) together. Made the new *Maybe* and abstract class and let *Just* and *Nothing* extend this new *Maybe*.

This way its much easier and cooler. Now there is a *Just* and a *Nothing* that are both of type *Maybe* and of type *IMonad*. 

Before there was only a *Maybe* of type IMonad, that could have been *Just* or *Nothing*.

Now together with implicit operators the *Just* and *Nothing* can be eased very comfortable.

#### The *Maybe* class

	public abstract class Maybe<A> : IMonad<A>
    {
        public static implicit operator A(Maybe<A> instance)
        {
            return instance.Return();
        }

        public static implicit operator Maybe<A>(A value)
        {
            if (value == null || value.Equals(default(A)))
                return new Nothing<A>();
            else
                return new Just<A>(value);
        }

        #region Maybe_Class_Implementation

        private A aValue;

        protected Maybe()
        {
            aValue = default(A);        // Nothing/Default constructor creates default(A) as value.
        }

        protected Maybe(A value)
        {
            aValue = value;
        }

        public A Value()
        {
            return aValue;
        }

        public bool isNothing
        {
            get
            {
                return this is Nothing<A>;
            }
        }

        #endregion 
		
		#region IMonad_Implementation
		...
		#endregion

		#region IEnumerator_Implementation
		...
		#endregion

		#Linq_Enumerable_Implementation
		...
		#endregion
	}
A thing to mention here is the case when the generic value on the right side of the *=* is null or default(T). Its my definition that i make a *Nothing* then. I think for complex types and *null* it is what someone would think at first. But what about *int* and *double* for example ? Is it really always wanted a *Maybe(0)* to be a *Nothing(0)*? 

I think its ok, or better its a definition of my monad world. 
Becaus of the problem mentioned above, when someone askes a *Nothing* with *Return()* for its value, I return *default(A)* even for *Nothing*. For complex types (*Func<..>*), default(Func<...>) is *null* for example. And *default(int)* or *default(double)* is 0 or 0.0.

It was a workaround i had to do. So why not doing it the other way round too.

#### *Just* and *Nothing*

They are both now very simple:

	public class Just<T> : Maybe<T>
    {
        public Just(T jValue) : base(jValue)
        {
        }
    }

    public class Nothing<T> : Maybe<T>
    {
    }

------------------------

## IMonad implementation

When an other *IMonad* is involved in an operation then it is always checked if it is *Nothing*.

### Visit

		public IMonad<A> Visit(Action<A> action)
        {
            if(this is Just<A> && action != null )
                action(aValue);
            return this;
        }
Maps the given *Action* over the value inside this *Maybe* if it is not nothing.

### Visit together with other monad.

        public IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther)
        {
            if (this is Just<A> && action != null && mOther != null)
                foreach(var element in mOther)
                    action(aValue, element);
            return this;
        }

### *Fmap*

	public IMonad<B> Fmap<B>(Func<A, B> function)
    {
        Maybe<B> resMaybe = new Nothing<B>();
        if (this is Just<A> && function != null)
            resMaybe = function(aValue);
        if (resMaybe == null)
            resMaybe = new Nothing<B>();
        return resMaybe;
    }

Map a given function over the value inside this monad, and pack the result in a new *Maybe*.
The implicit initializing of *resMaybe* with *resMaybe = function(aValue);* automatically makes the result *Maybe* a *Nothing* if the function result is *null* or *default(B)*.
To be really sure everything stays ok, check for *null* again.

### *Pure* and *Return*

	public IMonad<A> Pure(A parameter)
    {
        Maybe<A> result = parameter;        // Use implicit operator to make Just or Nothing 
        return result;
    }

    public A Return()
    {
        return aValue;
    }

The minimal context of a *Maybe* is a new one with the given value inside.
A new *Maybe* is returned, so its not like a *Setter*. Thats more the functional way.
With *Return* you can get the value out of the *Maybe*.
For a *Nothing* it is *default(A)*. It can be *null* for complex types.

### *App*
It is the same as Fmap (Select in Linq) only that the give function is inside another *Monad*. And because every *IMonad* is an *IEnumerable* there can be one ore MORE functions inside the other *Monad*. Thats why the foreach is used. The result will always be a *Maybe* with the result of the last function inside. So why iterate over each function? Hmm.. for completeness, and maybe something is done with the value of this *Maybe* inside the function(s) that is independend from the result *Monad*.

	public IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad)
    {
        Maybe<B> result = new Nothing<B>();
        if (this is Just<A> && functionMonad != null)
        {
            foreach (var function in functionMonad)
            {
                if (function != null)
                    result = new Just<B>(functionMonad.Return()(aValue));
            }
            if (result == null)
                result = new Nothing<B>();
        }
        return result;
    }

### *App* 2nd
This is the more complex *App* version.
Here every function inside the given monad returns also a monad of the result monad type.
The result monads of the functions have to be concatenated somewho together.
But how to do that. *Maybe monad* is a single enumerable. You cannt simply create a new *Maybe* with the result type and add the values of the function result monad.
The trick is to use the result monad of the first function. It has the right result type. 
And because every monad has a concat method, this monad can be used to flatten the result monads out and concatenate them together.
What *Concat* means for a *Monad* is self defined.
But if the result monad is a *ListMonad* for example, than it can be concatenated as someone would expect. 
And here is something new. This *App* function can break out of the *Maybe* (this). 
Most other monad functions return a monad of the same type. 
Here its the type of the function result monad.

	public IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad)
    {
        IMonad<B> result = new Nothing<B>();
        if (this is Just<A> && functionMonad != null)
        {
            result = null;
            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    if(result == null)  // if first time or first time (and second...) was Nothing
                        result = function(aValue); 
                    else
                    {
                        var fResult = function(aValue);
                        if(!(fResult is Nothing<B>))        // skip if result is nothing
                            result = result.Concat(fResult);
                    }
                }
            }
            if (result == null) // If some function returned null
                result = new Nothing<B>();
        }
        
        return result;
    }

### Combinate *Maybe* and other *IMonad* via *IMonad* with *Func(s)* inside.
	
	public IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther)
    {
        Maybe<C> result = new Nothing<C>();

        if (!isNothing && !(mOther is Nothing<B>))
        {
            foreach (var function in functionMonad)
            {
                if(function != null)
                    foreach (var otherValue in mOther)
                        result = function(aValue, otherValue);
            }
            if (result == null)
                result = new Nothing<C>();
        }

        return result;
    }

The first *Com* functin takes a *IMonad* with funtions inside and another *IMonad* as arguments. The functions inside the first argument takes two Arguments. The first argument is the type of the value inside this monad. The second argument is of the type of the value inside the given monad. *A* and *B* can be the same type (will be normal case i think).
The function puts each combination of the value inside this *Maybe* and all values in the given other monad as arguments inside the given functions.
A new *Maybe* with the result of the last function inside, is returned. Or a new *Nothing* is something is *null*. 

	public IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
	{
	    IMonad<C> result = new Nothing<C>();
	
	    if (!isNothing && !(mOther is Nothing<B>))         // other is no maybe and this is not nothing.
	    {
	        result = null;
	        foreach (var function in functionMonad)
	        {
	            foreach (var otherValue in mOther)
	            {
	                if (result == null)       // Make result monad the monad type of the function result
	                    result = function(aValue, otherValue);
	                else
	                {
	                    var fResult = function(aValue, otherValue);
	                    if (!(fResult is Nothing<B>))
	                        result.Concat(fResult);
	                }
	            }
	        }
	        if (result == null)
	            result = new Nothing<C>();
	    }
	
	    return result;
	}


It is the same as the first *Com* function only that the given functions (inside the first argument) return an *IMonad* them self. 
Its the same as in the second *App* function. The result monad of the first function is used as result monad. And is concatenated together with all other function result monad´s that follows.

-------------

## Connecting to Linq

### Where

    public IMonad<A> Where(Func<A, bool> predicate)
    {
        Maybe<A> result = new Nothing<A>();
        if (!this.isNothing && predicate(aValue))
            return new Just<A>(aValue);
        return result;
    }

Its like a filter function in *Haskell* return the value(s) where the given predicate is true.

### Where 2nd

    public IMonad<A> Where(Func<A, int, bool> predicate)
    {
        Maybe<A> result = new Nothing<A>();
        if (!this.isNothing && predicate(aValue, 0))
            result = new Just<A>(aValue);
        return result;
    }

The give predicate function uses the index too. 

### Select

    public IMonad<B> Select<B>(Func<A, B> f)
    {
        return Fmap<B>(f);
    }

Same as *Fmap*

### Select 2nd

    public IMonad<B> Select<B>(Func<A, int, B> function)
    {
        Maybe<B> resMaybe = new Nothing<B>();
        if (!this.isNothing)
            resMaybe = function(aValue, 0);
        return resMaybe;
    }

### SelectMany

    public IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f)
    {
        IMonad<B> result = new Nothing<B>();
        if(!this.isNothing)
            result = f(aValue);
        return result;
    }

Like a upgraded *Fmap* version where the function returns the new monad not the *Fmap*.
This can change the monad type.

    public IMonad<B> SelectMany<B>(Func<A, int, IMonad<B>> f)
    {
        IMonad<B> result = new Nothing<B>();
        if (!this.isNothing)
            result = f(aValue, 0);
        return result;
    }

### SelectMany with selector

That becomes very easy now. First normal *SelectMany* and then *ZipWith* given function.


	public IMonad<B> SelectMany<TMonad, B>(Func<A, IMonad<TMonad>> selector
									 , Func<A, TMonad, B> function)
    {
        return ZipWith<TMonad, B>(function, SelectMany(selector));
    }

    public IMonad<B> SelectMany<TMonad, B>(Func<A, int, IMonad<TMonad>> selector
										 , Func<A, TMonad, B> function)
    {
        return ZipWith<TMonad, B>(function, SelectMany(selector));
    }

---------------
## *Maybe* Examples
			
        Maybe<int> justInt = 5;
        Console.WriteLine("A new Just<double>: " + justInt.ToString());
        Maybe<int> nothingInt = 0;      // same as nothingInt = new Nothing<int>();
        Console.WriteLine("A new Nothing<double>: " + nothingInt.ToString());


        // justInt = 0; or justInt = new Nothing<int>() would make a Nothing out of the justInt

        Console.WriteLine("A new ListMonad<char>: ");
        var listMonadChar = new ListMonad<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g' }.Visit((x) => { Console.Out.Write(x + ", "); });
        Console.WriteLine("\n");

        Console.WriteLine("A new ListMonad<int>: ");
        var listMonadInt = new ListMonad<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Visit((x) => { Console.Out.Write(x + ", "); });
        Console.WriteLine("\n");

        Console.WriteLine("A new ListMonad<double>: ");
        var listMonadDouble = new ListMonad<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Visit((x) => { Console.Out.Write(x + ", "); });
        Console.WriteLine("\n");

        Console.ReadLine();

        var intToDoubleFunctin = new Func<int, double>(x => { return x * 0.5; });
        Console.WriteLine("A new Just with a function inside: f(x) = x * 0.5 ");
        var justFunction = new Just<Func<int, double>>(intToDoubleFunctin);
        Console.WriteLine(justFunction.ToString());
        Console.ReadLine();

        Console.WriteLine("Visits each combination of the Just 5 and the ListMonad<char> \n  using a lambda and Console.Write inside: ");
        justInt.Visit((i, c) => { Console.Out.Write(i + "" + c + ", "); }, listMonadChar);
        Console.WriteLine();
        // Outputs: 1a, 1b, 1c, 1d, 1e,

        Console.WriteLine("Same with Nothing<int> will output nothing: ");
        nothingInt.Visit((i, c) => { Console.Out.Write(i + "" + c + ", "); }, listMonadChar);
        Console.WriteLine("\n");
        Console.ReadLine();

        Console.WriteLine("Visits each combination of the Just 5 and the ListMonad<int> \n using a lambda and Console.Write inside. Add both values in print out: ");
        justInt.Visit((x, y) => { Console.Out.Write( x + y + ", "); }, listMonadInt);
        // Outputs: 5, 6, 7, 8, 9, 10, 11, 12, 13, 14

        Console.WriteLine("\nSame with Nothing<int>:");
        nothingInt = (Maybe<int>)nothingInt.Visit((x, y) => { Console.Out.Write(x + y + ", "); }, listMonadInt);
        Console.WriteLine(nothingInt.ToString());
        Console.WriteLine("\n");
        Console.ReadLine();


        Console.WriteLine("Fmap f(x) = x * 0.5 over the Just<int>(5).");
        var justDouble = justInt.Fmap<double>(intToDoubleFunctin).Visit((x) => { Console.Out.Write(x + "\n"); });
        // Output: 2.5
        Console.WriteLine();
        Console.ReadLine();

        Console.WriteLine("App Just<Func>(f(x) = x * 0.5) over the Just<int>(5).");
        justDouble = justInt.App(justFunction).Visit((x) => { Console.Out.Write(x + "\n"); });
        Console.WriteLine();

        Console.WriteLine("App Just<Func> over the Just<int>(5), \n where the functions returns a new ListMonad<int>() \n with two times the value inside the Just 5.");
        var function = new Just<Func<int, IMonad<int>>>((x) => { return new ListMonad<int>(){x, x};});
        var intListMonad = justInt.App<int>(function).Visit( (x) => { Console.Out.Write(x + ", "); } );
        Console.WriteLine("\n");
        // The result is a ListMonad<int>
        // Output: 5, 5,

        var functionListMonad = new ListMonad<Func<int, int, double>>();
        functionListMonad.Add( (x, y) => { return x*y;});
        functionListMonad.Add( (x, y) => { return x/ (y==0 ? 1 : y);});
        functionListMonad.Add( (x, y) => { return x% (y==0 ? 1 : y);});
        var result = justInt.Com<int, double>(functionListMonad, listMonadInt).Visit((x) => { Console.Out.Write(x + ", "); });
        Console.WriteLine();
        // Output: 5

        var functionListMonadTwo = new ListMonad<Func<int, double, IMonad<double>>>();
        functionListMonadTwo.Add((x, y) => { return new ListMonad<double>() { x + y }; });
        functionListMonadTwo.Add((x, y) => { return new ListMonad<double>() { x - y }; });
        functionListMonadTwo.Add((x, y) => { return new ListMonad<double>(){x * y}; });
        functionListMonadTwo.Add((x, y) => { return new ListMonad<double>(){x / (y == 0 ? 1 : y)}; });
        functionListMonadTwo.Add((x, y) => { return new ListMonad<double>(){x % (y == 0 ? 1 : y)}; });
        functionListMonadTwo.Add((x, y) => { return new Nothing<double>(); });
        var resultTwo = justInt.Com<double, double>(functionListMonadTwo, listMonadDouble).Visit((x) => { Console.Out.Write(x + ", "); });
        // Output: 5*0, 5*1, 5*2,... 5*1, 5/1, 5/2, 5/3, ... 5%1, 5%1, 5%2, 5%3,....
        Console.WriteLine();

        resultTwo = nothingInt.Com<double, double>(functionListMonadTwo, listMonadDouble).Visit((x) => { Console.Out.Write(x + ", "); });
        // Output: 5*0, 5*1, 5*2,... 5*1, 5/1, 5/2, 5/3, ... 5%1, 5%1, 5%2, 5%3,....
        Console.WriteLine();

        var resultThree = justInt.ZipWith((x, y) => { return x + y; }, intListMonad).Visit((x) => { Console.Out.Write(x + ", "); });
        Console.WriteLine();

        Console.WriteLine("Maping a f(x, y) = x*y over the Just 5 and a new Just<int>(10) using LINQ:");
        var query = from f in new Just<Func<int, int, int>>((x, y) => { return x * y; })
                    from x in listMonadInt
                    from y in new Just<int>(10)
                    select f(x, y);

        query.Visit((x) => { Console.Out.Write(x + ", "); });
        Console.WriteLine();
        Console.ReadLine();
