using System;
using System.Threading;

namespace Chapter1.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Starting program...");

			Thread t1 = new Thread(PrintNumbersWithStatus);
			Thread t2 = new Thread(DoNothing);

			Console.WriteLine(t1.ThreadState.ToString());

			t2.Start();
			t1.Start();

			for (int i = 1; i < 30; i++)
			{
				Console.WriteLine(t1.ThreadState.ToString());
			}
            
			Thread.Sleep(TimeSpan.FromSeconds(6));
			t1.Abort();
			Console.WriteLine("A thread has been aborted");
			Console.WriteLine(t1.ThreadState.ToString());
			Console.WriteLine(t2.ThreadState.ToString());
            Console.ReadLine();
		}

		static void DoNothing()
		{
			Thread.Sleep(TimeSpan.FromSeconds(2));
		}

		static void PrintNumbersWithStatus()
		{
			Console.WriteLine("Starting...");
			Console.WriteLine(Thread.CurrentThread.ThreadState.ToString());
			for (int i = 1; i < 10; i++)
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
				Console.WriteLine(i);
			}
		}
	}
}
