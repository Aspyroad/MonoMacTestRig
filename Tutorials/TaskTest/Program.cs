using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTest
{
    class Program
    {
        static void Main1(string[] args)
        {
            Task.Factory.StartNew(() => Console.WriteLine("Hello from taskland fuckers!"));

            var task1 = new Task(() => Console.WriteLine("Penis"));
            task1.Start();

            var task2 = Task.Factory.StartNew(Greet, "Hello");
            task2.Wait();

            var task3 = Task.Factory.StartNew(state => Greet2("Hello fag"), "FagTask");
            Console.WriteLine(task3.AsyncState);
            task3.Wait();

            Console.ReadLine();
        }

        static void Greet(object state)
        {
            Console.WriteLine(state);
        }
        static void Greet2(string message)
        {
            Console.WriteLine(message);
        }
    }
}
