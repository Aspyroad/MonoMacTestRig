
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMacTest
{

	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
				
		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController(
			IntPtr handle
		) : base(
				handle
			)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(
			NSCoder coder
		) : base(
				coder
			)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController() : base(
				"MainWindow"
			)
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
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
		
		partial void btn_execute(NSObject sender)
		{
			//avoid cross thread accessing of controls by storing values from controls in variables     
			int lower = int.Parse(this.txtLow.StringValue);
			int upper = int.Parse(this.txtHigh.StringValue);
			
			Task.Factory.StartNew<int>(() => this.getPrimesInRange(lower, upper).Count());
			
			//this.TxtNoOfPrimes.Text += this.getPrimesInRange(int.Parse(this.NudLower.Value.ToString()), int.Parse(this.NudUpper.Value.ToString())).Count().ToString() + Environment.NewLine;
			this.txtView.Value += this.getPrimesInRange(int.Parse(this.txtLow.StringValue), int.Parse(this.txtHigh.StringValue)).Count().ToString() + Environment.NewLine;
			
		}
		
		private IEnumerable<int> getPrimesInRange(int inclusiveStartValue, int exclusiveEndValue)
		{
			System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(2000));
			return getAllPrimes(Enumerable.Range(inclusiveStartValue, (exclusiveEndValue - inclusiveStartValue)));
		}

		private IEnumerable<int> getAllPrimes(IEnumerable<int> numbers)
		{
			var ii = from n in numbers where (!(n <= 1) && Enumerable.Range(2, (int)Math.Sqrt(n == 2 ? 0 : n)).All(i => n % i > 0)) select n;
			return ii;
		}	
		


		//start a new task, and then display result in text box
		
		//Task.TaskFactory.StartNew<int>(() => this.getPrimesInRange(lower, upper).Count());
		


		
	}
}

