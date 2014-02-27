/*
 *  Copyright (C) 2014  Muraad Nofal

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

namespace Monads
{
    public static class MonadSafeExtensions
    {
        #region Do_Write_Or_Read_Action_On_Model_Safe

        public static Monad<A> ActionW<A>(this Monad<A> monad, Action expr, bool updateNext = true)
        {
            monad.Lock.EnterWriteLock();

            try
            {
                expr();
                if (updateNext)
                    monad.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return monad;
        }

        public static Monad<A> ActionW<A>(this Monad<A> monad, Action<Monad<A>> expr, bool updateNext = true)
        {
            monad.Lock.EnterWriteLock();

            try
            {
                expr(monad);
                if (updateNext)
                    monad.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return monad;
        }

        public static Monad<A> ActionR<A>(this Monad<A> monad, Action<A> action)
        {
            monad.Lock.EnterReadLock();
            try
            {
                monad.Visit(action);
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitReadLock();
            }
            return monad;
        }

        #endregion

        #region Call_Method_From_Model_By_Name_Write_And_Read_Safe

        public static Monad<A> MethodW<A>(this Monad<A> monad, Func<Monad<A>> expr, bool updateNext = true)
        {
            monad.Concatenate(monad.MethodW2<A, A>(expr, false));
            if (updateNext)
                monad.UpdateValue();
            return monad;
        }

        public static Monad<B> MethodW2<A, B>(this Monad<A> monad, Func<Monad<B>> expr, bool updateNext = true)
        {
            Monad<B> result = null;
            monad.Lock.EnterWriteLock();
            try
            {
                result = expr();
                if (updateNext)
                    result.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static Monad<A> MethodW<A>(this Monad<A> monad, Func<A, Monad<A>> expr, bool updateNext = true)
        {

            monad.Concatenate(monad.MethodW2<A, A>(expr, false));
            if (updateNext)
                monad.UpdateValue();
            return monad;
        }
        public static Monad<B> MethodW2<A, B>(this Monad<A> monad, Func<A, Monad<B>> expr, bool updateNext = true)
        {
            Monad<B> result = null;
            monad.Lock.EnterWriteLock();
            try
            {
                result = expr(monad.Return());
                if (updateNext)
                    result.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return result;
        }

        public static Monad<A> MethodW<A>(this Monad<A> monad, Func<Monad<A>, Monad<A>> expr, bool updateNext = true)
        {
            monad.Concatenate(monad.MethodW2<A, A>(expr, false));
            if (updateNext)
                monad.UpdateValue();
            return monad;
        }

        public static Monad<B> MethodW2<A, B>(this Monad<A> monad, Func<Monad<A>, Monad<B>> expr, bool updateNext = true)
        {
            Monad<B> result = null;
            monad.Lock.EnterWriteLock();
            try
            {
                result = expr(monad);
                if (updateNext)
                    result.UpdateValue();
            }
            catch (Exception exception)
            {
                monad.UpdateValue(exception);
            }
            finally
            {
                monad.Lock.ExitWriteLock();
            }
            return result;
        }

        #endregion
    }
}
