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
		
		#region Event Handling
		// LogHandler
		public event LogHandler Log;
		public delegate void LogHandler(object sender, LogDataEventArgs _message);
		// *********
		
		protected virtual void OnLog(object sender, LogDataEventArgs _message)
		{
			if (Log != null)
			{
				Log(sender, _message);// Raise the event
			}
		}
		
		// ConpletionHandler
		public event EventHandler<CompletedEventArgs> AllTasksCompleted;
		// *********
		protected virtual void OnAllTasksCompleted(CompletedEventArgs e)
		{
			EventHandler<CompletedEventArgs> handler = AllTasksCompleted;
			if (handler != null)
			{
				handler(this, e);
			}
		}		
		#endregion
		
		#region Variables
		object lockObject = new object();		
		private string _message= "";
		private string mesheader = "T" + Thread.CurrentThread.ManagedThreadId.ToString() + " BlockingClass." + Environment.NewLine;
		#endregion
		
		public BlockingCollectionClass()
		{
			// Need to learn a little more about this task scheduler
			//this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
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
					// Raise the event.
					this.OnLog(this, new LogDataEventArgs(_message));
				}
			}
		}		

		public void ClearMessage()
		{
			this.message = "";
		}
				
		/*
		WithBlocking demonstrates two ways to consume a BlockingCollection.
		Task1, utilizes GetConsumingEnumerables and a foreach loop. 
		Employing GetConsumingEnumerables is pretty common in all the samples you'll see among the resources at the end of this article.
		Take, in Task2, is the second way to consume a BlockingCollection. 
		Like GetConsumingEnumerables, take blocks until data is available in the collection. 
		The try/catch block is required because, according to the documentation, InvalidOperationException is thrown when the collection is marked complete.
		*/
			
		public void WithBlocking(CancellationTokenSource tokenSource)
		{

			int[] items = { 1, 2, 3, 4, 5, 6, 7, 8 };
			var startTime = DateTime.Now;

			message += mesheader;
			message += "Starting...WithBlocking" + Environment.NewLine;

			var stage1 = new BlockingCollection<int>();
			var stage2 = new BlockingCollection<int>();

			//Producer - Pulls numbers from the array
			var task0 = Task.Factory.StartNew(() =>
			{
				
				foreach (int i in items)
				{
					stage1.Add(i);
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Producer - Add:" + i.ToString() + " Count=" + stage1.Count.ToString() + Environment.NewLine);
				}

				message += "Producer - Finished - Loaded all items into stage 1..." + Environment.NewLine;
				stage1.CompleteAdding();
				//return null;

			}, tokenSource.Token);

			// Consumer1 - Reads and passes data onto next stage
			var task1 = Task.Factory.StartNew(() =>
			{
				
				foreach (var i in stage1.GetConsumingEnumerable())
				{					
					//Pause 2 seconds simulating work
					Thread.Sleep(new TimeSpan(0, 0, 1));
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Consumer1 - Stage 1 Work Completed: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
					
					// ****** This will instantly start task 2...
					stage2.Add(i);				
				}
				message += "Consumer1 - Emptied all items from Stage 1..." + Environment.NewLine;
				stage2.CompleteAdding();
			},tokenSource.Token);
			
			// Consumer2 - Reads prints data
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
						message += "Consumer2 - Emptied all items from Stage 2..." + Environment.NewLine;
						break;
					}

					//Pause a little over half second to simulate work
					Thread.Sleep(new TimeSpan(0, 0, 0, 0, 300));
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Consumer2 - Stage 2 Work Completed: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);

				}
			}, tokenSource.Token)
				.ContinueWith((prevtask) => {message += "WithBlocking Example - Completed." + Environment.NewLine;});

			
			Task.WhenAll(task0, task1, task2);

		}
		
		/*
		One other interesting constructor parameter is a bounding value. 
		The Sample.WithBounding demonstrates BlockingCollection Bounding.
		Earlier in my manufacturing analogy I mentioned material piling up 
		in a stage of the assembly line requiring the prior stage to halt until the pile was consumed. 
		Bounding exists to define the size of the pile.
		You'll get some interesting results when executing this sample. 
		Steps in Task 1 and 2 will complete before Task0 finishes loading the Stage1 BlockingCollection. 
		Stage1 BlockingCollection is constrained to containing 2 integers at a time. 
		This means the integers must be consumed before more can be added. 
		Thus, the Add method in Task0 halts until Task1 consumes an integer.
		*/
		
		public void WithBounding(int boundvalue)
		{

			int[] items = { 1, 2, 3, 4, 5, 6, 7, 8 };
			var startTime = DateTime.Now;
			
			message += mesheader;
			message += "Producer...Starting...WithBounding" + Environment.NewLine;

			var stage1 = new BlockingCollection<int>(boundvalue);
			var stage2 = new BlockingCollection<int>();

			//Pulls numbers from the array
			var task0 = Task.Factory.StartNew(() =>
			{
				foreach (int i in items)
				{
					stage1.Add(i);
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Producer - Add:" + i.ToString() + " Count=" + stage1.Count.ToString() + Environment.NewLine);
				}

				message += "Producer - Loaded all items into stage 1..." + Environment.NewLine;
				stage1.CompleteAdding();

			});

			//Reads and passes data onto next stage
			var task1 = Task.Factory.StartNew(() =>
			{

				foreach (var i in stage1.GetConsumingEnumerable())
				{

					//Pause 2 seconds simulating work
					Thread.Sleep(new TimeSpan(0, 0, 2));
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ " Consumer1 - Stage 1 Work Complete: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
					
					stage2.Add(i);

				}

				message += "Consumer1 - Emptied all items from Stage 1..." + Environment.NewLine;
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
						message += "Consumer2 - Emptied all items from Stage 2..." + Environment.NewLine;
						break;
					}

					//Pause a little over half second to simulate work
					Thread.Sleep(new TimeSpan(0, 0, 0, 0, 600));
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+  " Consumer2 - Stage 2 Work Complete: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
				
				}

			}).ContinueWith((prevtask) => {message += "WithBounding Example - Completed." + Environment.NewLine;});

			Task.WhenAll(task0, task1, task2);
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
		
		public void WithoutBlocking(int boundvalue, int stage1timeout, int stage2timeout, CancellationTokenSource tokenSource)
		{

			int[] items = { 1, 2, 3, 4, 5, 6, 7, 8 };
			var startTime = DateTime.Now;
			

			message += "Producer...Starting...WithoutBlocking" + Environment.NewLine;

			var stage1 = new BlockingCollection<int>();
			var stage2 = new BlockingCollection<int>(boundvalue);//Required to force the TryAdd delay

			//Pulls numbers from the array
			var task0 = Task.Factory.StartNew(() =>
			{
				foreach (int i in items)
				{
					if (tokenSource.IsCancellationRequested)
					{
						break;
					}
					
					stage1.Add(i);
					message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString() 
						+ "Producer - Add:" + i.ToString() + " Count=" + stage1.Count.ToString() + Environment.NewLine);
				}
				
				if (tokenSource.IsCancellationRequested)
				{
					message += "Producer - Cancellation requested, exiting..." + Environment.NewLine;
				}
				else
				{
					message += "Producer - Loaded all items into stage 1..." + Environment.NewLine;
					stage1.CompleteAdding();
				}				

			}, tokenSource.Token);

			//Reads and passes data onto next stage
			var task1 = Task.Factory.StartNew(() =>
			{
				int i = -1;
				while (!stage1.IsCompleted)
				{
					if (tokenSource.IsCancellationRequested)
					{
						break;
					}
					
					if (stage1.TryTake(out i, stage1timeout))
					{
						message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString()
							+ " Consumer1 - Stage 1 Process: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);

							while (!stage2.TryAdd(i, new TimeSpan(0, 0, 0, 0, stage2timeout))) 
						{ 
							message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString()
								+ " Consumer1 - Attempt to add " + i.ToString() + " expired elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine); 
						}

						//Pause X miliseconds simulating work
						//change to as high as 2000 and low as 100
						//to see impact
						Thread.Sleep(new TimeSpan(0, 0, 0,0,2000));
					}
					else
					{
						message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString()
							+ " Consumer1 - TIMEOUT Stage1 - trytake: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);						
					}
				}
				if (tokenSource.IsCancellationRequested)
				{
					message += "Consumer1 - Cancellation requested, exiting..." + Environment.NewLine;
				}
				else
				{
					message += "Consumer1 - Emptied all items from Stage 1..." + Environment.NewLine;
					stage2.CompleteAdding();
				}
			}, tokenSource.Token);

			//Reads prints data
			var task2 = Task.Factory.StartNew(() =>
			{
				int i = -1;
				while (!stage2.IsCompleted)
				{
					if (tokenSource.IsCancellationRequested)
					{
						break;
					}
					
					if (stage2.TryTake(out i,300))
					{
						message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString()
							 + " Consumer2 - Stage 2 Process: " + i.ToString() + " elapsed " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
						//Pause a little over half second to simulate work
						Thread.Sleep(new TimeSpan(0, 0, 0, 0, 600));
					}
					else
					{
						message += ("T" + Thread.CurrentThread.ManagedThreadId.ToString()
								+ " Consumer2 - TIMEOUT Stage2 [Try take]: exceeded at " + DateTime.Now.Subtract(startTime).TotalSeconds.ToString() + Environment.NewLine);
					}
				}
			}, tokenSource.Token)
				.ContinueWith((prevtask) => 
				{
					message += "WithoutBlocking Example - Completed." + Environment.NewLine;
					
					// Simple test for checking propagation of exceptions.
					// This works fine, simply check the task.WhenAll result, it will say faulted with a list of the exceptions thrown...easy
					//throw new System.NotImplementedException();
									
				}, tokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

			Task.WhenAll(task0, task1, task2)
				.ContinueWith((prevTask) =>
				{
					CompletedEventArgs e = new CompletedEventArgs(prevTask);
					OnAllTasksCompleted(e);
				});				
		}
	}
	
	// The class to hold the information about the event
	// in this case it will hold only information
	// available in the clock class, but could hold
	// additional state information
	public class LogDataEventArgs : EventArgs
	{
		public LogDataEventArgs(string _message)
		{
			this.Message = _message;
		}
		public readonly string Message;
	}
	
	public class CompletedEventArgs : EventArgs
	{
		public CompletedEventArgs(Task _task)
		{
			task = _task;
		}		
		public Task task;		
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

