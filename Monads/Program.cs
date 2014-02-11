/*
 *  Copyright (C) 2013  Muraad Nofal

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalProgramming
{

    class Program
    {
        public static Func<string, int> func = (s) => { return s.Length; };
        public static Func<IMonad<string>, IMonad<int>> monadFunc = (m) => { return m.Fmap(func); };

        public static Func<IMonad<string>, Identity<string>> copyFunc = (id) => { return new Identity<string>(id.Return()); };

        public static Func<string, string> setString = (s) => s += "foobar ";

        static void Main(string[] args)
        {
            //Playground.MaybePlayaround();
            //Playground.ListMonadPlayground();
            //Playground.ListMonadOperatorPlayground();
            //Playground.ListMonadBindTest();
            //Playground.ExtensionPlayGround();

            Identity<string> id = "string";
            Identity<string> observer = "";
            id.Subscribe(observer);
            observer.NextAction = (m, x) => Console.WriteLine("Received next: " + x + " from: " + m.ToString());

            id.ActionW((i) => i.Pure("1"));
            Console.ReadLine();

            id.ActionW(() => id.Pure("2"));
            Console.ReadLine();

            //id.MethodW((i) => i.Fmap(func));

            // If the id monad is "here" we can use it directly inside the lambda.
            // All three lines are doing the same
            id.MethodW2(() => { return id.Fmap((s) => s.Length); });
            Console.ReadLine();

            id.MethodW(() => { return id.Pure(id.Fmap(setString).Return()); });
            Console.ReadLine();

            id.MethodW(() => { return id.Pure(id.Fmap(setString).Return()); });
            Console.ReadLine();

            id.MethodW(() => { return id.Pure(id.Fmap(setString).Return()); });
            Console.ReadLine();

            id.MethodW(() => { return id.Pure(id.Fmap(setString).Return()); });
            Console.ReadLine();


            // Will return new Identity with = operator
            //id.MethodW(() => { return id = (Identity<string>)id.Fmap(setString) ; });
            //id.MethodW(() => { return id = id.Fmap(setString).Return(); });

            id.MethodW2(() => { return id.Fmap(func); });
            id.MethodW2((m) => { return m.Fmap(func); });
            id.MethodW2(monadFunc);

            // If there is a function that takes a monad and it is defined somewhere else
            // then this function should be used.

            id.MethodW(() => id, false);
            id.MethodW((m) => m, false);
            id.MethodW(() => { id.Value = "Clear"; return id; });
            Console.WriteLine("Id value=" + id);
            Console.ReadLine();

            id.MethodW((m) => { return m.Pure("foobar"); });
            Console.ReadLine();

            //id.MethodW((str) => { str += "foobar"; return new Identity<string>(str); });
            id.MethodW((m) => { return m.Pure(m.Return() + "foobar"); });
            Console.ReadLine();

            id.MethodW(() => { return new Identity<string>(id.Return()); }, false);
            id.MethodW((m) => { return new Identity<string>(m.Return()); }, false);
            id.MethodW(copyFunc, false);       // If copyFunc is Func<Identity<string>, Identity<string>>, this wont work.
            //Identity<string> idCopy = (Identity<string>)id.MethodW(copyFunc);
            Identity<string> idCopy = id.MethodW2(copyFunc, false).Return().ToIdentity();

            Console.ReadLine();
        }
    }
}
