using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SemaphoreDemo
{

    class SemaphoreExample
    {
        // Three reserved slots for threads
        public static Semaphore Pool = new Semaphore(2, 2);
        public static void Main(string[] args)
        {
            // Create and start 20 threads
            for (int i = 0; i < 20; i++)            
            {
                Thread t = new Thread(new ThreadStart(DoWork));
                t.Start();
            }
            Console.ReadLine();
        }

        private static void DoWork()
        {
            // Wait on a semaphore slot to become available
            SemaphoreExample.Pool.WaitOne();

            #region Area Protected By Semaphore

            Console.WriteLine("Acquired slot...");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i + 1);
            }
            Console.WriteLine("Released slot...");

            #endregion

            // Release the semaphore slot
            SemaphoreExample.Pool.Release();
        }

    }

}