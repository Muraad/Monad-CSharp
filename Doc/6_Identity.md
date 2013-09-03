--------------------
## Identity monad

I will not explain it in more detail because its very the same like the *Maybe* without the *Nothing* part.

-------------------
## Implementation

	public class Identity<A> : IMonad<A>
    {
        public static implicit operator Identity<A>(A value)
        {
            return new Identity<A>(value);
        }

        private A idValue;
        
        public Identity(A aValue)
        {
            this.idValue = aValue;
        }

        public Identity()
        {
            this.idValue = default(A);
        }

        public A Value
        {
            get { return idValue; }
            set { this.idValue = value; }
        }

        #region IMonad_Interface_Implementation

        public IMonad<B> Fmap<B>(Func<A, B> function)
        {
            return new Identity<B>(function(idValue));
        }

        public IMonad<A> Pure(A parameter)
        {
            return new Identity<A>(parameter);
        }

        public A Return()
        {
            return idValue;
        }

        public IMonad<B> App<B>(IMonad<Func<A, B>> functionMonad)
        {
            Identity<B> resultIdentity = new Identity<B>();
            foreach (var function in functionMonad)
                if (function != null)
                    resultIdentity = function(idValue);
            return resultIdentity;
        }

        public IMonad<B> App<B>(IMonad<Func<A, IMonad<B>>> functionMonad)
        {
            IMonad<B> resultMonad = null;
            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    if (resultMonad == null)
                        resultMonad = function(idValue);
                    else
                        resultMonad = resultMonad.Concat(function(idValue));
                }
            }
            if (resultMonad == null)
                resultMonad = new Identity<B>();
            return resultMonad;
        }

        public IMonad<C> Com<B, C>(IMonad<Func<A, B, C>> functionMonad, IMonad<B> mOther)
        {
            Identity<C> resultMonad = new Identity<C>();

            foreach (var function in functionMonad)
                if (function != null)
                    foreach (var value in mOther)
                        resultMonad = function(idValue, value);

            return resultMonad;
        }

        public IMonad<C> Com<B, C>(IMonad<Func<A, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            IMonad<C> resultMonad = null;

            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    foreach (var value in mOther)
                    {
                        if (resultMonad == null)
                            resultMonad = function(idValue, value);
                        else
                            resultMonad = resultMonad.Concat(function(idValue, value));
                    }
                }
            }
            if (resultMonad == null)
                resultMonad = new Identity<C>();

            return resultMonad;
        }

        public IMonad<C> Com<B, C>(Func<A, B, C> function, IMonad<B> mOther)
        {
            Identity<C> resultMonad = new Identity<C>();

            foreach (var value in mOther)
                resultMonad = function(idValue, value);

            if (resultMonad == null)
                resultMonad = new Identity<C>();

            return resultMonad;
        }

        public IMonad<C> Com<B, C>(Func<A, B, IMonad<C>> function, IMonad<B> mOther)
        {
            IMonad<C> resultMonad = null;

            foreach (var value in mOther)
                if (resultMonad == null)
                    resultMonad = function(idValue, value);
                else
                    resultMonad = resultMonad.Concat(function(idValue, value));

            if (resultMonad == null)
                resultMonad = new Identity<C>();

            return resultMonad;
        }

        public IMonad<A> Visit(Action<A> action)
        {
            action(idValue);
            return this;
        }

        public IMonad<A> Visit<B>(Action<A, B> action, IMonad<B> mOther)
        {
            foreach (var element in mOther)
                action(idValue, element);
            return this;
        }

        public IMonad<A> Concat(IMonad<A> otherMonad)
        {
            this.idValue = otherMonad.Return();
            return this;
        }

        #endregion

        #region Linq_Enumerable_Interface_implemenation

        public IMonad<A> Where(Func<A, bool> predicate)
        {
            IMonad<A> resultMonad = new Identity<A>();
            if (predicate(idValue))
                resultMonad = new Identity<A>(idValue);
            return resultMonad;
        }

        public IMonad<A> Where(Func<A, int, bool> predicate)
        {
            IMonad<A> resultMonad = new Identity<A>();
            if (predicate(idValue, 0))
                resultMonad = new Identity<A>(idValue);
            return resultMonad;
        }

        public IMonad<B> Select<B>(Func<A, B> function)
        {
            return Fmap<B>(function);
        }

        public IMonad<B> Select<B>(Func<A, int, B> function)
        {
            return new Identity<B>(function(idValue, 0));
        }

        public IMonad<B> SelectMany<B>(Func<A, IMonad<B>> function)
        {
            return function(idValue);
        }

        public IMonad<B> SelectMany<B>(Func<A, int, IMonad<B>> function)
        {
            return function(idValue, 0);
        }

        public IMonad<B> SelectMany<R, B>(Func<A, IMonad<R>> selector, Func<A, R, B> function)
        {
            IMonad<R> tmp = selector(idValue);
            B result = function(idValue, tmp.Return());
            return new Identity<B>(result);
        }

        public IMonad<B> SelectMany<R, B>(Func<A, int, IMonad<R>> selector, Func<A, R, B> function)
        {
            IMonad<R> tmp = selector(idValue, 0);
            B result = function(idValue, tmp.Return());
            return new Identity<B>(result);
        }

        #endregion

        #region IEnumerator_Implementation

        public IEnumerator<A> GetEnumerator()
        {
            return new SingleEnumerator<A>(idValue);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SingleEnumerator<A>(idValue);
        }
        #endregion
    }

