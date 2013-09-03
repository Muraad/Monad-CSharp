-------------------
# Background
What is a Functor, Applicative Functor and a Monad? 

Basically they are all three like interface definitions in C#, called type class definition in haskell. (Or like abstract classes, because type class definitions in Hasell can have default function implementations).

And all three are like a generic interface. So they take a value and put that value inside of them.

And they define functions over this value, like every generic.  

The Functor type class simply has one function called fmap. That function takes the value of the type inside of this functor, and returns a new value of another (could be the same) type. Then fmap takes that  value and puts it inside a new functor and returns him as the last result.

The Applicative type class extends the functor type class, and defines two new functions.                                   The first is called pure, that takes a clean value of the generic type and puts that inside a new (applicative) functor. And an infix function <*> called applicate. That is that same as fmap. Only that that function is inside a functor too.

### Limitations

When i started to think about all that things, and started to program a bit, there were some questions that directly came up to my mind. 

In Haskell you define a type class that defines functions for this type class. the type class constructor gets generic parameter. 

Then you define an instance of this type class for a special data type. 

A short Haskell example:

Given (not real haskell monad) following type class *Monad*

	class Monad a where
	...

In C# this could be something like 

	interface IMonad<A>{..}

Given the following data type.

	data Maybe a = Nothing | Just a  

Now you have a new data type *Maybe* with a generic parameter a, and two constructors. Nothing that takes no arguments, and Just that takes one argument of type a.
The *Maybe* type can have a value then its *Just value* or it is *Nothing*. Then it has no defiened value. It is used to do *null propagation*.

This could be something like the following in C#:

	class Maybe<A>  

This is how you make *Maybe* an instance of Eq in Haskell.

	instance Monad (Maybe m) where
	...

and then you define the needed functions for the special type.

The generic parameter a of the type class Monad is used to "implement" the "interface".
But because Maybe needs a generic parameter too, there is added a new one m.

This is different than in C#. 
You dont write:

	class Maybe<A> : IMonad<Maybe<A>>

And even if someone would do it like this, you cannot add another generic parameter.

	class Maybe<A> : IMonad<Maybe, A>  // NOT ALLOWED!!!
  
Ok then 

	class Maybe<A> : IMonad<A>

Concrete example would be
	
	Maybe<double> dblMaybe = new Maybe<double>...

Have a look at the following Haskell type.

	Either a b = Left a | Right b

This type can "hold" two values of different types, a and b. 
Could be the following in C#:

	class Either<R, L>{...}

But how can you make it a Monad?
In Haskell its easy:

	instance Monad (Either a b) where
	...

But what can you do in C#?

	class Either<R, L> : IMonad<R, L> 	// NOT ALLOWED!

	class Either<R, L> : IMonad<R>, IMonad<L> 		// NOT ALLOWED!

I´v tried the approach 
	
	class Maybe<A> : IMonad<Maybe<A>>
	...

but then it becomes stranger and stranger if you think about a list monad.

	class ListMonad<A> : List<A>, IMonad<List<A>>
	
	or ??
	
	class ListMonad<A> : List<A>, IMonad<ListMonad<A>>

At this point i was stopping and stick with the following:

	interface IMonad<A> {...}

and implementations like

	class Maybe<A> : IMonad<A>
	and
	class ListMonad<A> : List<A>, IMonad<A> 

Even if i knew i cannot have an *Either* this way. (See *Either* example above).

So directly at the beginning i saw some walls that cannt be beared down, because of C# is no "complete" and/or real function programming language. 

And i saw there are no own operators. That would make the things even more funny and clearly.

But now lets go, and see how far someone can get.