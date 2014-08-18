
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;

using MonoMac.Foundation;
using MonoMac.AppKit;

using Sample.WithBlocking;

namespace MonoMacTest
{

	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		private BlockingCollectionClass obj;
		private string message;
		private string mesheader = "T" + Thread.CurrentThread.ManagedThreadId.ToString() + " MainForm." + Environment.NewLine;
		private int counter;
		private TaskScheduler _scheduler;
		CancellationTokenSource tokenSource;
		private bool bCancel = false;
				
		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController() : base("MainWindow")
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
			obj = new Sample.WithBlocking.BlockingCollectionClass();
			// Hook up our logoutput - Message is threadsafe!
			//obj.Log += new BlockingCollectionClass.LogHandler(OnNewMessage);
			obj.Log += OnNewMessage;
			obj.AllTasksCompleted += OnTaskCompletion;
			this._scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window
		{
			get
			{
				return (MainWindow)base.Window;
			}
		}

		private void OnNewMessage(object sender, LogDataEventArgs  e) 
		{
			counter += 1;
			this.message = mesheader + e.Message;
			this.txtView.BeginInvokeOnMainThread(() => (this.txtView.Value = this.message));
		}
		
		private void OnTaskCompletion(object sender, CompletedEventArgs e)
		{
			this.btnWithoutBlocking.BeginInvokeOnMainThread(() => (this.btnWithoutBlocking.Title = "WithoutBlock"));
			this.bCancel = false;
			this.tokenSource = null;
		}
		
		partial void btn_CountPrimes(NSObject sender)
		{
			//avoid cross thread accessing of controls by storing values from controls in variables
			int lower;
			int upper;
			string low = (this.txtLow.StringValue);
			string high = (this.txtHigh.StringValue);
			
			if (low.Length == 0)
			{
				lower = 0;
			}
			else
			{
				lower = int.Parse(low);
			}
			
			if (high.Length == 0)
			{
				upper = 0;
			}
			else
			{
				upper = int.Parse(high);
			}			
					
			// New task version		
			Task.Factory.StartNew<int>(() => this.getPrimesInRange(lower, upper).Count())
				.ContinueWith((i) => this.txtView.Value += i.Result.ToString() + Environment.NewLine, this._scheduler); 
			
			// Original, blocks the UI thread - NO GOOD!
			//this.txtView.Value += this.getPrimesInRange(int.Parse(this.txtLow.StringValue), int.Parse(this.txtHigh.StringValue)).Count().ToString() + Environment.NewLine;
		}
		
		private IEnumerable<int> getPrimesInRange(int inclusiveStartValue, int exclusiveEndValue)
		{
			System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(5000));
			return getAllPrimes(Enumerable.Range(inclusiveStartValue, (exclusiveEndValue - inclusiveStartValue)));
		}

		private IEnumerable<int> getAllPrimes(IEnumerable<int> numbers)
		{
			var ii = (from n in numbers where (!(n <= 1) && Enumerable.Range(2, (int)Math.Sqrt(n == 2 ? 0 : n)).All(i => n % i > 0)) select n).ToList();
			return ii;
		}			
		
		partial void btn_WithBlocking(NSObject sender)
		{
			this.txtView.Value = "";
			obj.WithBlocking(tokenSource);

		}
		
		partial void btn_WithBounding(NSObject sender)
		{
			this.txtView.Value = "";
			int intBound;
			string strBound = this.txtWithBounding.StringValue;
			if (strBound.Length == 0)
			{
				intBound = 8;
			}
			else
			{
				intBound = int.Parse(strBound);
			}
			
			obj.WithBounding(intBound);
		}
		
		partial void btn_WithoutBlocking(NSObject sender)
		{				
			if (!this.bCancel)
			{	
				this.tokenSource = new CancellationTokenSource();  	
				obj.ClearMessage();
				this.txtView.Value = "";
			}
			else
			{
				this.tokenSource.Cancel();
				this.bCancel = false;
				return;				//********EXIT POINT
			}

			#region BoundCheck
			int intBound;
			string strBound = this.txtWithoutBlocking.StringValue;
			if (strBound.Length == 0)
			{
				intBound = 8;
			}
			else
			{
				intBound = int.Parse(strBound);
			}
			#endregion

			#region Stage1TimeoutCheck
			int intStage1Timeout;
			string strStage1Timeout = this.txtStage1Timeout.StringValue;
			if (strStage1Timeout.Length == 0)
			{
				intStage1Timeout = 100;
			}
			else
			{
				intStage1Timeout = int.Parse(strStage1Timeout);
			}
			#endregion

			#region Stage2TimeoutCheck
			int intStage2Timeout;
			string strStage2Timeout = this.txtStage2Timeout.StringValue;
			if (strStage2Timeout.Length == 0)
			{
				intStage2Timeout = 300;
			}
			else
			{
				intStage2Timeout = int.Parse(strStage2Timeout);
			}
			#endregion

			this.btnWithoutBlocking.Title = "Cancel";
			bCancel = true;
			
			obj.WithoutBlocking(intBound, intStage1Timeout, intStage2Timeout, tokenSource);
						
			//this.btnWithoutBlocking.Title = "WithoutBlock";
			//bCancel = false;						
		}
		
	}
}

