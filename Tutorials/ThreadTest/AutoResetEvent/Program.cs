using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Testing.AutoResetEventTut
{
    class Program
    {
        private static AutoResetEvent _workerEvent = new AutoResetEvent(false);
        private static AutoResetEvent _mainEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            var t = new Thread(() => Process(1));
            Console.WriteLine("MainID = {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Starting worker thread...");
            t.Start();

            // Block/Wait the main thread
            //_mainEvent.WaitOne();
            //Now switched to t - when t calls Set() we will start here

            Console.WriteLine("First operation is completed!");
            Console.WriteLine("Performing an operation on a main thread");

            Thread.Sleep(TimeSpan.FromSeconds(.5));
            _workerEvent.Set();

            Console.WriteLine("Now running the second operation on a second thread");

            _mainEvent.WaitOne();

            Console.WriteLine("Second operation is completed!");
            Console.ReadLine();
        }

        static void Process(int seconds)
        {
            Console.WriteLine("WorkerID = {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Starting some work...");

            Thread.Sleep(TimeSpan.FromSeconds(seconds));

            Console.WriteLine("WorkerID = {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Worker Thread - Work is done!");
            // Start main backup...give it a ticket
            _mainEvent.Set();

            //Console.WriteLine("ThreadID = {0}", Thread.CurrentThread.ManagedThreadId);
            //Console.WriteLine("Worker Thread - Waiting for a main thread to complete its work");
            //_mainEvent.WaitOne();
            //Console.WriteLine("Starting second operation...");
            //Thread.Sleep(TimeSpan.FromSeconds(seconds));
            //Console.WriteLine("Work is done!");
            //_workerEvent.Set();
        }



    }
}
