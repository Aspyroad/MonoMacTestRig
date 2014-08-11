using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sample.WithBlocking
{

	public class BlockingCollectionClass
	{
		object lockObject = new object();
		private string _message= "";
		
		public BlockingCollectionClass()
		{

		}
		
		protected string message
		{
			get
			{
				string result;
				lock (lockObject)
				{
					result = _message;
				}
				return result;
			}
			set
			{
				lock (lockObject)
				{
					_message = value;
					// Raise an event
				}
			}
		}
		
		/*
		WithBlocking demonstrates two ways to consume a BlockingCollection.
		Task1, utilizes GetConsumingEnumerables and a foreach loop. 
		Employing GetConsumingEnumerables is pretty common in all the samples you'll see among the resources at the end of this article.
		Take, in Task2, is the second way to consume a BlockingCollection. 
		Like GetConsumingEnumerables, take blocks until data is available in the collection. 
		The try/catch block is required because, according to the documentation, InvalidOperationException is thrown when the collection is marked complete.
		*/
			
		public string WithBlocking()
		{

			int[] items = { 1, 2, 3, 4, 5, 6, 7, 8 };
			var startTime = DateTime.Now;

			message += "Starting...WithBlocking" + Environment.NewLine;

			var stage1 = new BlockingCollection<int>();
			var stage2 = new BlockingCollection<int>();

			//Pulls numbers from the array
			// Technically the producer
			var task0 = Task.Factory.StartNew(() =>
			{
				foreach (int i in items)
				{
					stage1.Add(i);
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Producer - Add:" + i.ToString() + " Count=" + stage1.Count.ToString() + Environment.NewLine);
				}

				message += "Producer Finished - Loaded all items into stage 1..." + Environment.NewLine;
				stage1.CompleteAdding();

			});
			
			//Consumer First Worker
			//Reads and passes data onto next stage
			var task1 = Task.Factory.StartNew(() =>
			{

				foreach (var i in stage1.GetConsumingEnumerable())
				{
					stage2.Add(i);
					
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Consumer1 - Stage 1 WORKING!! Process: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
					
					//Pause 2 seconds simulating work!!!!!****************The real process is finally being done!
					Thread.Sleep(new TimeSpan(0, 0, 1));
				}
				message += "Consumer1 Emptied all items from Stage 1..." + Environment.NewLine;
				stage2.CompleteAdding();
			});
			
			//Consumer
			//Reads prints data
			var task2 = Task.Factory.StartNew(() =>
			{
				int i = -1;
				while (!stage2.IsCompleted)
				{
					try
					{
						i = stage2.Take();
					}
					catch (InvalidOperationException) //Take throws exception when completed
					{
						message += "Consumer2 Emptied all items from Stage 2..." + Environment.NewLine;
						break;
					}
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Consumer2 - Stage 2 WORKING!! Process: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);

					//Pause a little over half second to simulate work
					Thread.Sleep(new TimeSpan(0, 0, 0, 0, 600));
				}

			});

			Task.WaitAll(task0, task1, task2);
			message += "Consumer2 Completed. Fuckoff!" + Environment.NewLine;
			
			return message;
		}
		
		/*
		One other interesting constructor parameter is a bounding value. 
		The Sample.WithBounding demonstrates BlockingCollection Bounding.
		Earlier in my manufacturing analogy I mentioned material piling up in a stage of the assembly line requiring the prior stage to halt until the pile was consumed. 
		Bounding exists to define the size of the pile.
		You'll get some interesting results when executing this sample. 
		Steps in Task 1 and 2 will complete before Task0 finishes loading the Stage1 BlockingCollection. 
		Stage1 BlockingCollection is constrained to containing 2 integers at a time. 
		This means the integers must be consumed before more can be added. 
		Thus, the Add method in Task0 halts until Task1 consumes an integer.
		*/
		
		public string WithBounding()
		{

			int[] items = { 1, 2, 3, 4, 5, 6, 7, 8 };
			var startTime = DateTime.Now;

			message += "Starting...WithBounding" + Environment.NewLine;

			var stage1 = new BlockingCollection<int>(2);
			var stage2 = new BlockingCollection<int>();

			//Pulls numbers from the array
			var task0 = Task.Factory.StartNew(() =>
			{
				foreach (int i in items)
				{
					stage1.Add(i);
					message += ("Add:" + i.ToString() + " Count=" + stage1.Count.ToString() + Environment.NewLine);
				}

				message += "Loaded all items into stage 1..." + Environment.NewLine;
				stage1.CompleteAdding();

			});

			//Reads and passes data onto next stage
			var task1 = Task.Factory.StartNew(() =>
			{

				foreach (var i in stage1.GetConsumingEnumerable())
				{
					stage2.Add(i);

					message += ("Stage 1 Process: " + i.ToString() + "elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);

					//Pause 2 seconds simulating work
					Thread.Sleep(new TimeSpan(0, 0, 2));
				}
				message += "Emptied all items from Stage 1..." + Environment.NewLine;
				stage2.CompleteAdding();
			});

			//Reads prints data
			var task2 = Task.Factory.StartNew(() =>
			{
				int i = -1;
				while (!stage2.IsCompleted)
				{
					try
					{
						i = stage2.Take();
					}
					catch (InvalidOperationException)
					{
						message += "Emptied all items from Stage 2..." + Environment.NewLine;
						break;
					}
					message += ("Stage 2 Process: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
					
					//Pause a little over half second to simulate work
					Thread.Sleep(new TimeSpan(0, 0, 0, 0, 600));
				}

			});

			Task.WaitAll(task0, task1, task2);
			message += "Completed.  Press any key to quit..." + Environment.NewLine;
			return message;

		}
				
		/*
		Bounding is also used to introduce artificial latency into the WithoutBlocking sample.
		One of the major differences between WithoutBlocking and WithBlocking are the TryAdd and TryTake statements. 
		Examination of the parameters reveals timeout values in milliseconds or in a defined TimeSpan class.
		WithoutBlocking Tasks, block for small increments of time before continuing execution. 
		The sample code utilizes seconds or half seconds, but given that a Timespan can be a small interval, a developer is not constrained to milliseconds. 
		Because a method can return before the timeout interval, WithoutBlocking also excludes the try/catch code.
		This is an interesting sample to tinker with. 
		Changing the Sleep values in Task1 and Task2 has some interesting effects and really demonstrates shifting bottlenecks in code.
		*/		
		
		public string WithoutBlocking()
		{

			int[] items = { 1, 2, 3, 4, 5, 6, 7, 8 };
			var startTime = DateTime.Now;

			message += "Starting...WithoutBlocking" + Environment.NewLine;

			var stage1 = new BlockingCollection<int>();
			var stage2 = new BlockingCollection<int>(1);//Required to force the TryAdd delay

			//Pulls numbers from the array
			var task0 = Task.Factory.StartNew(() =>
			{
				foreach (int i in items)
				{
					stage1.Add(i);
					message += ("Add:" + i.ToString() + " Count=" + stage1.Count.ToString() + Environment.NewLine);
				}

				message += "Loaded all items into stage 1..." + Environment.NewLine;
				stage1.CompleteAdding();

			});

			//Reads and passes data onto next stage
			var task1 = Task.Factory.StartNew(() =>
			{
				int i = -1;
				while (!stage1.IsCompleted)
				{
					if (stage1.TryTake(out i,100))
					{
						message += ("Stage 1 Process: " + i.ToString() + "elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);

						while (!stage2.TryAdd(i, new TimeSpan(0, 0, 0,0,300))) 
						{ 
							message += ("Attempt to add " + i.ToString() + " expired elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine); 
						}

						//Pause X miliseconds simulating work
						//change to as high as 2000 and low as 100
						//to see impact
						Thread.Sleep(new TimeSpan(0, 0, 0,0,2000));
					}
				}
				message += "Emptied all items from Stage 1..." + Environment.NewLine;
				stage2.CompleteAdding();
			});

			//Reads prints data
			var task2 = Task.Factory.StartNew(() =>
			{
				int i = -1;
				while (!stage2.IsCompleted)
				{
					if (stage2.TryTake(out i,300))
					{
						message += ("Stage 2 Process: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
						//Pause a little over half second to simulate work
						Thread.Sleep(new TimeSpan(0, 0, 0, 0, 600));
					}
					else
					{
						message += ("Stage 2 Wait timeout exceeded at " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
					}
				}

			});

			Task.WaitAll(task0, task1, task2);
			message += ("Completed.  Press any key to quit..." + Environment.NewLine);	
			return message;
		}
	}

	public class TaskQueue<T> : IDisposable where T : class
		{
			object locker = new object();
			Thread[] workers;
			Queue<T> taskQ = new Queue<T>();

			public TaskQueue(int workerCount)
			{
				workers = new Thread[workerCount];

				// Create and start a separate thread for each worker
				for (int i = 0; i < workerCount; i++)
					(workers[i] = new Thread(Consume)).Start();
			}

			public void Dispose()
			{
				// Enqueue one null task per worker to make each exit.
				foreach (Thread worker in workers) EnqueueTask(null);
				foreach (Thread worker in workers) worker.Join();
			}

			public void EnqueueTask(T task)
			{
				lock (locker)
				{
					taskQ.Enqueue(task);
					Monitor.PulseAll(locker);
				}
			}

			void Consume()
			{
				while (true)
				{
					T task;
					lock (locker)
					{
						while (taskQ.Count == 0) Monitor.Wait(locker);
						task = taskQ.Dequeue();
					}
					if (task == null) return;         // This signals our exit
					Console.Write(task);
					Thread.Sleep(1000);              // Simulate time-consuming task
				}
			}
		}
	
	public sealed class SimpleScheduler : TaskScheduler, IDisposable
	{
		private BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
		private Thread _main = null;

		public SimpleScheduler()
		{
			_main = new Thread(new ThreadStart(this.Main));
		}

		private void Main()
		{
			Console.WriteLine("Starting Thread " + Thread.CurrentThread.ManagedThreadId.ToString());

			foreach (var t in _tasks.GetConsumingEnumerable())
			{
				TryExecuteTask(t);
			}
		}

		/// <summary>
		/// Used by the Debugger
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return _tasks.ToArray<Task>();
		}


		protected override void QueueTask(Task task)
		{
			_tasks.Add(task);

			if (!_main.IsAlive) { _main.Start(); }//Start thread if not done so already
		}


		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return false;
		}


		#region IDisposable Members

		public void Dispose()
		{
			_tasks.CompleteAdding(); //Drops you out of the thread
		}

		#endregion
	}	
	
}

