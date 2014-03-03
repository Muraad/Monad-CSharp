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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Monads;

namespace Monads
{
    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name + " is " + Age + " years old.";
        }
    }

    public class CacheMonadTest
    {
        public static void Test()
        {
            CacheMonad<string, Person> cache = new CacheMonad<string, Person>(1200, 600);

            //Atomic<int> atomic = 5;
            Identity<int> atomic = 5;
            var value = atomic;

            Console.WriteLine(value);
            Console.ReadLine();

            cache.KeyCalculator = (v) => { return v.Name; };

            cache.Append(new CacheEntry<string, Person>("User1", new Person() { Name = "User1", Age = 42 }));
            cache["User2"] = new CacheEntry<string, Person>("User2", new Person() { Name = "User2", Age = 42 });

            var user = new Person() { Name = "User3", Age = 42 };
            cache[user.Name] = new CacheEntry<string,Person>(user.Name, user);

            cache.Add(new Person() { Name = "User4", Age = 42 });
            cache.Add("User5", new Person() { Name = "User5", Age = 42 });
            cache.Add(new Person() { Name = "User6", Age = 42 });

            for (int i = 0; i < 2000; i++)
                cache.Add(new Person() { Name = "User" + (i + 10), Age = i });

            Console.WriteLine(cache["User1"]);
            Console.WriteLine(cache["User2"]);
            Console.WriteLine(cache["User3"]);
            Console.WriteLine(cache["User4"]);
            Console.WriteLine(cache["User5"]);
            Console.WriteLine(cache["User6"]);

            //foreach (var entry in cache)
            //    Console.WriteLine(entry.Value.Name + " is " + entry.Value.Age + " years old.");

            var query = from v in cache
                        where v.Value.Age % 2 == 0
                        select v;

            query.Visit((entry) => Console.WriteLine(entry.Value.Name + " is " + entry.Value.Age + " years old."));

            cache["User2"] = new CacheEntry<string, Person>("User2", new Person() { Name = "User2", Age = 99 });
            Console.WriteLine(cache["User2"]);


            System.Threading.Thread.Sleep(500);
            //cache.CleanUp();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Cache count = " + cache.Count);
            Console.WriteLine(cache["User1"]);
            Console.WriteLine(cache["User2"]);
            Console.WriteLine(cache["User3"]);
            Console.WriteLine(cache["User4"]);
            Console.WriteLine(cache["User5"]);
            Console.WriteLine(cache["User6"]);

            System.Threading.Thread.Sleep(500);
            //cache.CleanUp();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Cache count = " + cache.Count);
            Console.WriteLine(cache["User1"]);
            Console.WriteLine(cache["User2"]);
            Console.WriteLine(cache["User3"]);
            Console.WriteLine(cache["User4"]);
            Console.WriteLine(cache["User5"]);
            Console.WriteLine(cache["User6"]);

            System.Threading.Thread.Sleep(500);
            //cache.CleanUp();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Cache count = " + cache.Count);
            Console.WriteLine(cache["User1"]);
            Console.WriteLine(cache["User2"]);
            Console.WriteLine(cache["User3"]);
            Console.WriteLine(cache["User4"]);
            Console.WriteLine(cache["User5"]);
            Console.WriteLine(cache["User6"]);

            System.Threading.Thread.Sleep(500);
            //cache.CleanUp();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Cache count = " + cache.Count);
            Console.WriteLine(cache["User1"]);
            Console.WriteLine(cache["User2"]);
            Console.WriteLine(cache["User3"]);
            Console.WriteLine(cache["User4"]);
            Console.WriteLine(cache["User5"]);
            Console.WriteLine(cache["User6"]);

            foreach (var entry in cache)
                Console.WriteLine(entry.Value.Name + " is " + entry.Value.Age + " years old.");

            System.Threading.Thread.Sleep(500);
            //cache.CleanUp();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Cache count = " + cache.Count);
            Console.WriteLine(cache["User1"]);
            Console.WriteLine(cache["User2"]);
            Console.WriteLine(cache["User3"]);
            Console.WriteLine(cache["User4"]);
            Console.WriteLine(cache["User5"]);
            Console.WriteLine(cache["User6"]);

            System.Threading.Thread.Sleep(500);
            //cache.CleanUp();
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Cache count = " + cache.Count);
            Console.WriteLine(cache["User2"]);
            Console.WriteLine(cache["User3"]);
            Console.WriteLine(cache["User4"]);
            Console.WriteLine(cache["User5"]);
            Console.WriteLine(cache["User6"]);

            foreach (var entry in cache)
                Console.WriteLine(entry.Value.Name + " is " + entry.Value.Age + " years old.");

            Console.ReadLine();
        }
    }
}
