using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monads
{
    public class Right<T> : EitherValue<T>
    {
        T value;
        public Right(T tValue)
        {
            value = tValue;
        }
        public T Value()
        {
            return value;
        }
    }

    public class Left<T> : EitherValue<T>
    {
        T value;
        public Left(T lValue)
        {
            value = lValue;
        }
        public T Value()
        {
            return value;
        }
    }

    public interface EitherValue<T>
    {
        T Value();
    }

    public class EitherDecorator<L, R> : AMonadDecorator<L>, IMonad<R>
    {
        private Identity<L> left = new Identity<L>();
        private Identity<R> right = new Identity<R>();

        public EitherDecorator(L lValue, R rValue)
        {
            left = new Identity<L>(lValue);
            right = new Identity<R>(rValue);
        }

        public void Value(ref L lValue)
        {

        }
        public void Value(ref R rValue)
        {

        }

        #region Right

        public IMonad<B> Fmap<B>(Func<R, B> function)
        {
            return right.Fmap(function);
        }

        public IMonad<R> Pure(R parameter)
        {
            return right.Pure(parameter);
        }

        public R Return()
        {
            return right.Return();
        }

        public IMonad<B> App<B>(IMonad<Func<R, B>> functionMonad)
        {
            return right.App(functionMonad);
        }

        public IMonad<B> App<B>(IMonad<Func<R, IMonad<B>>> functionMonad)
        {
            return right.App(functionMonad);
        }

        public IMonad<C> Com<B, C>(IMonad<Func<R, B, C>> functionMonad, IMonad<B> mOther)
        {
            return right.Com(functionMonad, mOther);
        }

        public IMonad<C> Com<B, C>(IMonad<Func<R, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            return right.Com(functionMonad, mOther);
        }

        public IMonad<R> Visit(Action<R> function)
        {
            return right.Visit(function);
        }

        public IMonad<C> Com<B, C>(Func<R, B, C> function, IMonad<B> mOther)
        {
            return right.Com(function, mOther);
        }

        public IMonad<C> Com<B, C>(Func<R, B, IMonad<C>> function, IMonad<B> mOther)
        {
            return right.Com(function, mOther);
        }

        public IMonad<R> Concat(IMonad<R> otherMonad)
        {
            return right.Concat(otherMonad);
        }

        public IMonad<R> Where(Func<R, bool> predicate)
        {
            return right.Where(predicate);
        }

        public IMonad<R> Where(Func<R, int, bool> predicate)
        {
            return right.Where(predicate);
        }

        public IMonad<B> Select<B>(Func<R, B> f)
        {
            return right.Select(f);
        }

        public IMonad<B> Select<B>(Func<R, int, B> f)
        {
            return right.Select(f);
        }

        public IMonad<B> SelectMany<B>(Func<R, IMonad<B>> f)
        {
            return right.SelectMany(f);
        }

        public IMonad<B> SelectMany<B>(Func<R, int, IMonad<B>> f)
        {
            return right.SelectMany(f);
        }

        public IMonad<B> SelectMany<TMonad, B>(Func<R, IMonad<TMonad>> selector, Func<R, TMonad, B> function)
        {
            return right.SelectMany(selector, function);
        }

        public IMonad<B> SelectMany<TMonad, B>(Func<R, int, IMonad<TMonad>> selector, Func<R, TMonad, B> function)
        {
            return right.SelectMany(selector, function);
        }

        public  new IEnumerator<R> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Left

        /*public override L Return()
        {
            return left.Return();
        }*/

        public override IMonad<B> Fmap<B>(Func<L, B> function)
        {
            throw new NotImplementedException();
        }

        public override IMonad<L> Pure(L parameter)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> App<B>(IMonad<Func<L, B>> functionMonad)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> App<B>(IMonad<Func<L, IMonad<B>>> functionMonad)
        {
            throw new NotImplementedException();
        }

        public override IMonad<C> Com<B, C>(IMonad<Func<L, B, C>> functionMonad, IMonad<B> mOther)
        {
            throw new NotImplementedException();
        }

        public override IMonad<C> Com<B, C>(IMonad<Func<L, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            throw new NotImplementedException();
        }

        public override IMonad<L> Visit(Action<L> function)
        {
            throw new NotImplementedException();
        }

        public override IMonad<C> Com<B, C>(Func<L, B, C> function, IMonad<B> mOther)
        {
            throw new NotImplementedException();
        }

        public override IMonad<C> Com<B, C>(Func<L, B, IMonad<C>> function, IMonad<B> mOther)
        {
            throw new NotImplementedException();
        }

        public override IMonad<L> Concat(IMonad<L> otherMonad)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> Fmap<B>(Func<L, IMonad<B>> f)
        {
            throw new NotImplementedException();
        }

        public override IMonad<L> Where(Func<L, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public override IMonad<L> Where(Func<L, int, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> Select<B>(Func<L, B> f)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> Select<B>(Func<L, int, B> f)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> SelectMany<B>(Func<L, IMonad<B>> f)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> SelectMany<B>(Func<L, int, IMonad<B>> f)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> SelectMany<TMonad, B>(Func<L, IMonad<TMonad>> selector, Func<L, TMonad, B> function)
        {
            throw new NotImplementedException();
        }

        public override IMonad<B> SelectMany<TMonad, B>(Func<L, int, IMonad<TMonad>> selector, Func<L, TMonad, B> function)
        {
            throw new NotImplementedException();
        }

        #endregion


        public IMonad<R> Visit<B>(Action<R, B> action, IMonad<B> mOther)
        {
            foreach (var element in mOther)
                action(right.Return(), element);
            return this;
        }

        public override IMonad<L> Visit<B>(Action<L, B> action, IMonad<B> mOther)
        {
            foreach (var element in mOther)
                action(left.Return(), element);
            return this;
        }
    }

    public class Either<L, R> : Identity<L>, IMonad<R>
    {
        private Identity<R> right;

        public Either(L lValue) : base(lValue)
        {
        }

        public Either(R rValue)
        {
            right = new Identity<R>(rValue);
        }

        public Either(L lValue, R rValue) : base(lValue)
        {
            right = new Identity<R>(rValue);
        }

        public Either<R, L> EitherFlip()
        {
            return new Either<R,L>(right.Return(), base.Return());
        }

        public Identity<L> Left
        {
            get { return this; }
        }

        public Identity<R> Right
        {
            get { return right; }
        }

        #region Left_Monad_Wraper

        public new IMonad<B> Fmap<B>(Func<L, B> function)
        {
            return new Either<B, R>(function(base.Return()), right.Value);
        }

        public new IMonad<L> Pure(L parameter)
        {
            return new Either<L, R>(parameter, right.Value);
        }

        public L ReturnL()
        {
            return base.Return();
        }

        public new IMonad<B> App<B>(IMonad<Func<L, B>> functionMonad)
        {
            Identity<B> result = new Either<B, R>(default(B), right.Value);
            foreach (var function in functionMonad)
            {
                if (function != null)
                    result = new Either<B, R>(function(base.Return()), right.Value);
            }
            return result;
        }

        public new IMonad<B> App<B>(IMonad<Func<L, IMonad<B>>> functionMonad)
        {
            IMonad<B> result = null;
            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    if (result == null)
                        result = function(base.Return());
                    else
                        result = result.Concat(function(base.Return()));
                }
            }
            if (result == null)
                result = new Either<B, R>(default(B), right.Value);
            return result;
        }

        public new IMonad<C> Com<B, C>(Func<L, B, C> function, IMonad<B> mOther)
        {
            IMonad<C> result = new Either<C, R>(default(C), right.Value);

            if (function != null)
                foreach (B element in mOther)
                    result = new Either<C, R>(function(base.Return(), element), right.Value);

            if (result == null)
                result = new Either<C, R>(default(C), right.Value);
            return result;
        }

        public new IMonad<C> Com<B, C>(IMonad<Func<L, B, C>> functionMonad, IMonad<B> mOther)
        {
            IMonad<C> result = new Either<C, R>(default(C), right.Value);
            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    foreach (B element in mOther)
                    {
                        result = new Either<C, R>(function(base.Return(), element), right.Value);
                    }
                }
            }
            if (result == null)
                result = new Either<C, R>(default(C), right.Value);
            return result;
        }

        public new IMonad<C> Com<B, C>(Func<L, B, IMonad<C>> function, IMonad<B> mOther)
        {
            IMonad<C> result = null;

            if (function != null)
                foreach (B element in mOther)
                    result = function(base.Return(), element);

            if (result == null)
                result = new Either<C, R>(default(C), right.Value);
            return result;
        }

        public new IMonad<C> Com<B, C>(IMonad<Func<L, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            IMonad<C> result = null;
            foreach (var function in functionMonad)
            {
                if (function != null)
                {
                    foreach (B element in mOther)
                    {
                        if (result == null)
                            result = function(base.Return(), element);
                        else
                            result = result.Concat(function(base.Return(), element));
                    }
                }
            }
            if (result == null)
                result = new Either<C, R>(default(C), right.Value);
            return result;
        }

        public new IMonad<L> Visit(Action<L> action)
        {
            base.Visit(action);
            return this;
        }

        public new IMonad<L> Visit<B>(Action<L, B> action, IMonad<B> mOther)
        {
            foreach (B element in mOther)
                action(base.Return(), element);
            return this;
        }

        public new IMonad<L> Concat(IMonad<L> otherMonad)
        {
            base.Value = otherMonad.Return();
            return this;
        }

        #endregion 

        #region Right_Monad_Wrapper

        public IMonad<B> Fmap<B>(Func<R, B> function)
        {
            return new Either<L, B>(function(right.Return()));
        }

        public IMonad<R> Pure(R parameter)
        {
            return new Either<L, R>(parameter);
        }

        public new R Return()
        {
            return right.Return();
        }

        public IMonad<B> App<B>(IMonad<Func<R, B>> functionMonad)
        {
            IMonad<B> result = new Either<L, B>(base.Return(), default(B));
            foreach(var function in functionMonad)
                if (function != null)
                    result = new Either<L, B>(base.Return(), function(right.Return()));
            return result;
        }

        public IMonad<B> App<B>(IMonad<Func<R, IMonad<B>>> functionMonad)
        {
            IMonad<B> result = null;
            foreach(var function in functionMonad)
            {
                if (function != null)
                {
                    if (result == null)
                        result = function(right.Return());
                    else
                        result = result.Concat(function(right.Return()));
                }
            }
            if (result == null)
                result = new Either<L, B>(base.Return(), default(B));
            return result;
        }

        public IMonad<C> Com<B, C>(IMonad<Func<R, B, C>> functionMonad, IMonad<B> mOther)
        {
            IMonad<C> result = null;

            foreach (var function in functionMonad)
                if (function != null)
                    foreach (B element in mOther)
                        result = new Either<L, C>(base.Return(),function(right.Return(), element));

            if (result == null)
                result = new Either<L, C>(base.Return(), default(C));
            return result;
        }

        public IMonad<C> Com<B, C>(IMonad<Func<R, B, IMonad<C>>> functionMonad, IMonad<B> mOther)
        {
            IMonad<C> result = null;

            foreach (var function in functionMonad)
                if (function != null)
                    foreach (B element in mOther)
                    {
                        if (result == null)
                            result = function(right.Return(), element);
                        else
                            result = result.Concat(function(right.Return(), element));
                    }

            if (result == null)
                result = new Either<L, C>(base.Return(), default(C));
            return result;
        }

        
        public IMonad<R> Visit(Action<R> function)
        {
            function(right.Return());
            return this;
        }

        public IMonad<C> Com<B, C>(Func<R, B, C> function, IMonad<B> mOther)
        {
            return new Either<L, C>(base.Return(), function(right.Return(), mOther.Return()));
        }

        public IMonad<C> Com<B, C>(Func<R, B, IMonad<C>> function, IMonad<B> mOther)
        {
            IMonad<C> result = null;

            if (function != null)
            {
                foreach (B element in mOther)
                {
                    if (result == null)
                        result = function(right.Return(), element);
                    else
                        result = result.Concat(function(right.Return(), element));
                }
            }
            if (result == null)
                result = new Either<L, C>(base.Return(), default(C));
            return result;
        }

        public IMonad<R> Concat(IMonad<R> otherMonad)
        {
            return new Either<L, R>(base.Return(), otherMonad.Return());
        }

        #endregion

        #region Left_IEnumerable_And_Linq_implementation

        public new IMonad<L> Where(Func<L, bool> predicate)
        {
            return base.Where(predicate);
        }

        public new IMonad<L> Where(Func<L, int, bool> predicate)
        {
            return base.Where(predicate);
        }

        public new IMonad<B> Select<B>(Func<L, B> f)
        {
            return base.Select(f);
        }

        public new IMonad<B> Select<B>(Func<L, int, B> f)
        {
            return base.Select(f);
        }

        public new IMonad<B> SelectMany<B>(Func<L, IMonad<B>> f)
        {
            return base.SelectMany(f);
        }

        public new IMonad<B> SelectMany<B>(Func<L, int, IMonad<B>> f)
        {
            return base.SelectMany(f);
        }

        public new IMonad<B> SelectMany<TMonad, B>(Func<L, IMonad<TMonad>> selector, Func<L, TMonad, B> function)
        {
            return base.SelectMany(selector, function);
        }

        public new IMonad<B> SelectMany<TMonad, B>(Func<L, int, IMonad<TMonad>> selector, Func<L, TMonad, B> function)
        {
            return base.SelectMany(selector, function);
        }

        /*public new IEnumerator<L> GetEnumerator()
        {
            return base.GetEnumerator();
        }*/

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion

        #region Right_IEnumerable_And_Linq_implementation

        public IMonad<R> Where(Func<R, bool> predicate)
        {
            return right.Where(predicate);
        }

        public IMonad<R> Where(Func<R, int, bool> predicate)
        {
            return right.Where(predicate);
        }

        public IMonad<B> Select<B>(Func<R, B> f)
        {
            return right.Select(f);
        }

        public IMonad<B> Select<B>(Func<R, int, B> f)
        {
            return right.Select(f);
        }

        public IMonad<B> SelectMany<B>(Func<R, IMonad<B>> f)
        {
            return right.SelectMany(f);
        }

        public IMonad<B> SelectMany<B>(Func<R, int, IMonad<B>> f)
        {
            return right.SelectMany(f);
        }

        public IMonad<B> SelectMany<TMonad, B>(Func<R, IMonad<TMonad>> selector, Func<R, TMonad, B> function)
        {
            return right.SelectMany(selector, function);
        }

        public IMonad<B> SelectMany<TMonad, B>(Func<R, int, IMonad<TMonad>> selector, Func<R, TMonad, B> function)
        {
            return right.SelectMany(selector, function);
        }

        public new IEnumerator<R> GetEnumerator()
        {
            return right.GetEnumerator();
        }

        /*System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return right.GetEnumerator();
        }*/

        #endregion


        public IMonad<R> Visit<B>(Action<R, B> action, IMonad<B> mOther)
        {
            if (action != null)
                foreach (B element in mOther)
                    action(right.Return(), element);
            return this;
        }
    }
}
