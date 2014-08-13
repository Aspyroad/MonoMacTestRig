using System;
using System.Threading; 

namespace Examples.AdvancedProgramming.AsynchronousOperations
{
    public class AsyncDemo 
    {
        // The method to be executed asynchronously. 
        public string TestMethod(int callDuration, out int threadId) 
        {
            Console.WriteLine("Test method begins.");
            Thread.Sleep(callDuration);
            threadId = Thread.CurrentThread.ManagedThreadId;
            return String.Format("My call time was {0}.", callDuration.ToString());
        }
    }
    // The delegate must have the same signature as the method 
    // it will call asynchronously. 
    public delegate string AsyncMethodCaller(int callDuration, out int threadId);

    public class AsyncMain 
    {
        static void Main() 
        {
            // The asynchronous method puts the thread id here. 
            int threadId;

            // Create an instance of the test class.
            Console.WriteLine("Creating AsyncDemo [ThreadID = {0}]", Thread.CurrentThread.ManagedThreadId);
            AsyncDemo ad = new AsyncDemo();

            // Create the delegate.
            Console.WriteLine("Assign AsyncDemo method to delegate [ThreadID = {0}]", Thread.CurrentThread.ManagedThreadId);
            AsyncMethodCaller caller = new AsyncMethodCaller(ad.TestMethod);

            // Initiate the asychronous call.
            IAsyncResult result = caller.BeginInvoke(10000, out threadId, null, null);

            // Initiate on the same thread as main with Invoke only.
            string strResult = caller.Invoke(100 , out threadId);

            //Thread.Sleep(0);
            Console.WriteLine("Main thread {0} does some work.", Thread.CurrentThread.ManagedThreadId);

            // Wait for the WaitHandle to become signaled.
            //result.AsyncWaitHandle.WaitOne();

            // Perform additional processing here. 
            // Call EndInvoke to retrieve the results. 
            //string returnValue = caller.EndInvoke(out threadId, result);


            // Close the wait handle.
            //result.AsyncWaitHandle.Close();

            //Console.WriteLine("The call executed on thread {0}, with return value \"{1}\".", threadId, returnValue);
            Console.WriteLine("The call executed on thread {0}, with return value \"{1}\".", threadId, strResult);

            Console.ReadLine();
        }
    }
}

/* This example produces output similar to the following:

Main thread 1 does some work.
Test method begins.
The call executed on thread 3, with return value "My call time was 3000.".
 */

