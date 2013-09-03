--------------------------
#Single value enumerator



A monad is like a box that has something (a value ore more) inside. The box analogous is used very often in the Haskell world. For *Functors*, *Applicative Functors* and *Monads*. 
Which are in my case all packed into one Interface the *IMonad*

Monads like *Identity* and *Maybe* can always have only maximum one value inside their box. 
Because *IMonad* extends *IEnumerable* every monad have to be able to return an *Enumerator*.
So there is a need for an Enumerator that can iterate over this single value.

Its very simple. The constructor takes that single value. Than the needed functions are implemented. A flag *next* is set to false during the first *Current*. Which is returned from *MoveNext()* and set to true again in *Reset()*.


	public class SingleEnumerator<A> : IEnumerator<A>
	{
	    private A aValue = default(A);
	    private bool next = true;
	
	    public SingleEnumerator(A value)
	    {
	        aValue = value;
	    }
	
	    public A Current
	    {
	        get
	        {
	            next = false;
	            return aValue;
	        }
	    }
	
	    public void Dispose()
	    {
	        return;
	    }
	
	    object System.Collections.IEnumerator.Current
	    {
	        get
	        {
	            next = false;
	            return aValue;
	        }
	    }
	
	    public bool MoveNext()
	    {
	        return next;
	    }
	
	    public void Reset()
	    {
	        next = true;
	    }
	}
