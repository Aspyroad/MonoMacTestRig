using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace ThreadTest
{
    class Program
    {

        static void Main(string[] args)
        {
            for (int i = 1; i <= 6; i++)
            {
                string threadname = "Thread " + i.ToString();
                int secondsToWait = (2 + (2 * i));
                var t = new Thread(() => AccessDatabase(threadname, secondsToWait));
                t.Start();

            }
        }

        static SemaphoreSlim _semaphore = new SemaphoreSlim(2);
        static void AccessDatabase(string name, int seconds)
        {
            Console.WriteLine("{0} waits to access a database", name);
            _semaphore.Wait();
            Console.WriteLine("{0} was granted access to the database", name);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            _semaphore.Release();
        }
    }
}
