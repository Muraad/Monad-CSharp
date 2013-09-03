###[Main](Index.html)_____________________________________________________________________________[Next: Single Enumerator](SingleEnumerator.html)

--------------------------

#The IMonad Interface

	public interface IMonad<A> : IEnumerable<A>
    {

        #region IMonad_Core_Interface_Function_Definitions

        IMonad<B> Fmap<B>(Func<A, B> function);
        IMonad<A> Pure(A parameter);
        A Return();
        IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad);
        IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad);
        IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther);
        IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther);
        IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther);
        IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther);
        IMonad<A> Visit(Action<A> function);
        IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther);
        IMonad<A> Concat(IMonad<A> otherMonad);

        #endregion

      
        #region Linq_Enumerable_Connection

        IMonad<A> Where(Func<A, bool> predicate);
        IMonad<A> Where(Func<A, int, bool> predicate);
        IMonad<B> Select<B>(Func<A, B> f);
        IMonad<B> Select<B>(Func<A, Int32, B> f);
        IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f);
        IMonad<B> SelectMany<B>(Func<A, Int32, IMonad<B>> f);
        IMonad<B> SelectMany<TMonad, B>(Func<A, IMonad<TMonad>> selector, Func<A, TMonad, B> function);
        IMonad<B> SelectMany<TMonad, B>(Func<A, Int32, IMonad<TMonad>> selector, Func<A, TMonad, B> function);

        #endregion
    }

IMonad extends IEnumerable, that means every monad is an enumerable even if it holds only one value. I found out that is is very usefull for example or the list monad *App* function. Because then *foreach* can be used on the other monad no matter if it holds only one value or more.
The IMonad interface also defines LINQ functions, that every Monad has to implement. 
When i was playing around i recognized that IEnumerable is almost a Monad. But because an Applicative Functor is a monad too, i saw that there is no "App" like function defined in IEnumerable. And IEnumerable normaly is for types like collections that holds one ore MORE values. So my IMonad extends IEnumerable. Onyl difference is that some function definitions are "overridden", that they take IMonad´s as arguments and return values instead of IEnumerable´s.

With the *Haskell* type classes in front of me, i was starting to design this interface.
There were a few changes over time. For example the *IEnumerable* extension was not there from the beginning on.

---------------

## Function explanations:
### Pure
	IMonad<A> Pure(A parameter);
This function simply takes a value and puts it in the minimal context of the (new created) monad.

### Return
	A Return();
Returns the value inside this monad.
A problem here is the *ListMonad*. Because it holds more than one values of type A. 
The workaround i do is to return the head of the internal list.

### Fmap

	IMonad<B> Fmap<B>(Func<A, B> function);

Basically fmap is the same as the simple Select from IEnumerable.
It maps a function over the value(s) inside this monad. The result is packed from fmap into a new monad that is returned. 
I was starting to write the IMonad interace before i was getting more into LINQ and recognized that there is alread the Select. But i let it inside the interface because i think fmap is a better and more understable name here. In Haskell its called fmap too, and is defined in the functor typeclass.

### Fmap 2nd

	IMonad<B> Fmap<B>(Func<A, IMonad<B>> function); 

Same as *Fmap* above. Difference is that the function returns a new monad.

### App

	IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad);

This is the interesting function definition. It comes from the Haskell Applicative typeclass.
Thats the function i dont find in IEnumerable. It is like the fmap function, the only difference is that the function that is applied to the value(s) inside the monad, is also inside another monad. So you can have a List(Monad) with functions and apply them to all values in another list(monad) with one line. 
Or you can have a function that is packed in a maybe and apply them to the value(s) inside of this monad.
The result of applying the function(s) to the value(s) inside the monad is packed into a new monad that is returned.

### App 2nd
	IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad);

The second version of the App function (like its defined in Haskell) is even more interesting. Here the function(s) inside the given monad also returns a monad. 
So the result is flattened out if there are more than one result values.
If the function returns a list(monad) of type B and this monad is a list(monad) of type A then the result type is also a list(monad) of type B and not a list(monad) of list(monad) of type B.
The inner monad is always flattened out.
This function is something special. It can "break out" of the monad type that function is called from. That means *App* on a *Maybe* can return a *ListMonad* for example. 
At first it seems there is a problem. How to concatenate the values inside the function result monads of type B together? If you are inside a *ListMonad* that would be no problem. But what about a *Maybe* that can only store one value? The trick here is to use the first *IMonad* that is returned from the first function. 
All other values inside the next returned *monads* are concatenated together. 
Thats why there is a *Concat* function in the *IMonad* interface. 
This is a pattern i will use in all functions, that work with new monads as internal function results.
What *Concat* means for a individual monad is self defined. For a *ListMonad* it is what someone would expect. For a *Maybe* for example, i simply return a new *Maybe* with the value inside that is returned from the *Return()* function from the given *Monad*.

### Com

	IMonad<C> ZipWith<B, C>(Func<A, B, C> function, IMonad<B> mOther);

Put every possible combination of the values inside this *ListMonad* and the other given *IMonad* as arguments inside the given function. All function results are put into a new *ListMonad*.

### Com 2nd

	IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther);

Same as above only difference is that the function itselfs return  IMonad. So the result *IMonad* is flattned out, and every value inside of it is added to the new result *ListMonad*.


### Com 3d

        IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther);

Same as above, where the given function(s) is(are) inside a *IMonad*.

### Com 4th
        IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther);

Same as *Com 2d* only that the functions inside the monad returns a monad them self.
The same procedere as used in App is used to produce the result monad.
This is the most complex function.

### Visit
	IMonad<A> Visit(Action<A> function);
Apply the given Action to the value(s) inside of this monad.
During my testing i found that function very usefull. For example you can apply a lambda with Console.Out.Write... over the value(s) inside the monad.

### Visit 2nd

	IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther);

Visit each combination of the values inside this monad and the other given monad.

### Concat

    IMonad<A> Concat(IMonad<A> otherMonad);

Concatenates two monads together. What this mean for every monad is individual.

The rest of the function definitions are for the connection to LINQ. That every monad can be used with Linq. 
What would already possible because IMonad extends IEnumerable. And as long as every monad has a Enumerator, there could be Linq querys on it. 
But i want that the Linq functions return and work with IMonad´s.

-------------------
## Linq connection

	#region Linq_Enumerable_Connection

    IMonad<A> Where(Func<A, bool> predicate);   // filter.
    IMonad<A> Where(Func<A, int, bool> predicate);

    IMonad<B> Select<B>(Func<A, B> f);       // fmap
    IMonad<B> Select<B>(Func<A, Int32, B> f);   // fmap with index.

    IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f);
    IMonad<B> SelectMany<B>(Func<A, Int32, IMonad<B>> f);
    IMonad<B> SelectMany<TMonad, B>(Func<A, IMonad<TMonad>> selector, Func<A, TMonad, B> function);
    IMonad<B> SelectMany<TMonad, B>(Func<A, Int32, IMonad<TMonad>> selector, Func<A, TMonad, B> function);

    #endregion

Same defintions like in *IEnumerable* only *IEnumerable* is replaced with *IMonad*. 
So Linq and Monad functions can be mixed up.

Something to mention here is the case where a Linq function takes a function as argument that uses an index of the current value inside the *IEnumerable*. For single value monads like *Maybe* or *Identity* the index is always zero.


------------
## Single and multi value monads

There are monads that hold only one value (single enumerable) like maybe and identity, and there are monads that hold one or more like a list monad (or every normal enumerable or a collection).

-----------
## Breaking out the the current monad type
Ever function where a *Func<...,IMonad<..>>* is involved, has the ability to "break out" of the current monad type, that means the result monad type can be different than the monad where *Func* was used. All other functions return a monad of the same type.

## Convention

No monad knows another monad. They all only know *IMonad* and them self. So there is no special handling between different combination of monads.
To change the result monad even if its unknow, a trick described at function *App 2nd* is used. In short the *IMonad* that is returned from the first given function is used to concatenate with the next function results.