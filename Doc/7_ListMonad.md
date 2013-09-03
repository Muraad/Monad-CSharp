--------------------
## ListMonad

### The ListMonad

	public class ListMonad<A> : List<A>, IMonad<A>
    {
		#region IMonad_Interface_Implementation
		...
		#endregion

		#region Linq_Enumerable_Functions
		...
		#endregion
}

To be a *List* the *ListMonad* simply extends *List*.

---------
## Implementation background

I think the implementation for the *ListMonad* is very straight forward, because the *foreach* and that things are intuitive for *List´s*. For me its the main representant for an *IEnumerable*.
*ListMonad* functions never break out of the *ListMonad* because they dont have to.
A *ListMonad* can have more than one value inside so it can be used to store values inside other *IMonads* that are function results.

-----------------------
## IMonad implementation

### Fmap

	public IMonad<B> Fmap<B>(Func<A, B> function)
    {
        ListMonad<B> resultEnumerable = new ListMonad<B>();
        foreach (A element in this)
            resultEnumerable.Add(function(element));
        return resultEnumerable;
    }

Apply the given function over each element in the *ListMonad* and pack each result in the newly created result *ListMonad*.

### Pure
	public IMonad<A> Pure(A parameter)
    {
        ListMonad<A> list = new ListMonad<A>();
        list.Add(parameter);
        return list;
    }

The minimal context for a list is the empty list. So put the given value inside a new *ListMonad* and return it.

### App

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

Apply each function in the given *IMonad* to each element in this *ListMonad*, and pack each result inside the new result *ListMonad*.

### App 2nd

	public IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad)
    {
        ListMonad<B> resultListMonad = new ListMonad<B>();
        foreach (Func<A, IMonad<B>> function in functionMonad)
        {
            if (function != null) 
            {
                foreach (A element in this)
                {
                    foreach (B resElement in function(element))
                        resultListMonad.Add(resElement);
                }
            }
        }
        return resultListMonad;
    }

Same as *App* above. But because given functions also return a *IMonad*, the result is flattned out. So only one *ListMonad* is returned.

-----------------------
### Combination of *ListMonad* with other *IMonad* via given functions

###Com
	public IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther)
    {
        ListMonad<C> resultListMonad = new ListMonad<C>();
        foreach (Func<A, B, C> f in functionMonad)
            foreach (A a in this)
                foreach (B b in mOther)
                    resultListMonad.Add(f(a, b));
        return resultListMonad;
    }

Put each combination of the values inside this *ListMonad* and inside the given *IMonad* in every given function. Put each result the new result *ListMonad*.

### Com 2nd

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

Same as above. Only function result *IMonad´s* are flattned out.
New *ListMonad* with all results is returned.

### Com 3rd

	public IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther)
    {
        ListMonad<C> resultListMonad = new ListMonad<C>();

        foreach (A elementThis in this)
            foreach (B elementOther in mOther)
                resultListMonad.Add(function(elementThis, elementOther));

        return resultListMonad;
    }

Combination with only one given function.

### Com 4th
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

Combinate with only one given function. Function result *IMonad* is flattned out.

### Visit

    public IMonad<A> Visit(Action<A> action)
    {
        foreach (A element in this)
            action(element);
        return this;
    }

Apply given *Action* to each element.

    public IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther)
    {
        foreach (var aElement in this)
            foreach (var otherElement in mOther)
                action(aElement, otherElement);
        return this;
    }

Visig each combination of the values in this *ListMonad* and the given *IMonad* with given *Action*.

### Return 

	public A Return()
    {
        return this.First<A>();
    }

Return is a problem for *ListMonad*. As workaround i return the list head,

### Concat

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

Create and return a new *ListMonad* with each element of this and the given *IMonad* inside.

-------------------


## Connecting to Linq

### Where 
    public IMonad<A> Where(Func<A, bool> predicate)
    {
        ListMonad<A> resultListMonad = new ListMonad<A>();
        foreach (A element in this)
            if (predicate(element))
                resultListMonad.Add(element);
        return resultListMonad;
    }

Filter elements in this *ListMonads*.

    public IMonad<A> Where(Func<A, int, bool> predicate)
    {
        throw new NotImplementedException();
    }

### Select

    public IMonad<B> Select<B>(Func<A, B> f)
    {
        return Fmap<B>(f);
    }

Same as Fmap.

### Select 2nd

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

Fmap where function uses current element index.

### SelectMany

    public IMonad<B> SelectMany<B>(Func<A, IMonad<B>> f)
    {
        ListMonad<B> resultListMonad = new ListMonad<B>();
        foreach (A element in this)
            resultListMonad.Concat(f(element));
        return resultListMonad;
    }

Apply given function to each element. Flattne out results with *Concat*.

### SelectMany 2nd

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

Same as above, where function uses current element index.

### SelectMany 3rd

    public IMonad<B> SelectMany<R, B>(Func<A, IMonad<R>> selector, Func<A, R, B> function)
    {
        ListMonad<B> resultListMonad = new ListMonad<B>();
        foreach (A element in this)
            foreach(R rValue in selector(element))          
                resultListMonad.Add(function(element, rValue));
        return resultListMonad;
    }

First apply selector for each element, then apply function to each combination between elements in this *ListMonad* and the selector result *IMonad*.

### SelectMany 4th

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

Same as above where selector uses current index.