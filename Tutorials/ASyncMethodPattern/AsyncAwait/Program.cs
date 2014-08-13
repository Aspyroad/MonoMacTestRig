using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Diagnostics;

namespace AsyncAwait
{
    class TaskTester
    {
        static void Main(string[] args)
        {
            var threadid = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread ID = " + threadid.ToString());
            MyMethod();
            Console.ReadLine();

        }

        static async void MyMethod()
        {
            Console.WriteLine("Making the webcall now...");
            Task<int> myTask = AccessTheWebAsync();
            Console.WriteLine("Waiting...");
            var result = await myTask;

            Console.WriteLine(result.ToString());

        }

        // Three things to note in the signature: 
        //  - The method has an async modifier.  
        //  - The return type is Task or Task<T>. (See "Return Types" section.)
        //    Here, it is Task<int> because the return statement returns an integer. 
        //  - The method name ends in "Async."
        static async Task<int> AccessTheWebAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();

            // You need to add a reference to System.Net.Http to declare client.
            HttpClient client = new HttpClient();
            // GetStringAsync returns a Task<string>. That means that when you await the 
            // task you'll get a string (urlContents).
            Console.WriteLine("Webcall has been made at - " + sw.ElapsedMilliseconds + "ms");
            Task<string> getStringTask = client.GetStringAsync("http://msdn.microsoft.com");

            // You can do work here that doesn't rely on the string from GetStringAsync.
            
            // The await operator suspends AccessTheWebAsync. 
            //  - AccessTheWebAsync can't continue until getStringTask is complete. 
            //  - Meanwhile, control returns to the caller of AccessTheWebAsync. 
            //  - Control resumes here when getStringTask is complete.  
            //  - The await operator then retrieves the string result from getStringTask.
            string urlContents = await getStringTask;
            Console.WriteLine("Webcall has just returned at - " + sw.ElapsedMilliseconds + "ms");

            // The return statement specifies an integer result. 
            // Any methods that are awaiting AccessTheWebAsync retrieve the length value. 
            return urlContents.Length;
        }

        static void DoIndependentWork(string message)
        {
            Console.WriteLine(message);
        }

    }
}
