// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace MonoMacTest
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton btnExecute { get; set; }

		[Outlet]
		MonoMac.AppKit.NSScrollView txtBox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtHigh { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtLow { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView txtView { get; set; }

		[Action ("btn_execute:")]
		partial void btn_execute (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnExecute != null) {
				btnExecute.Dispose ();
				btnExecute = null;
			}

			if (txtBox != null) {
				txtBox.Dispose ();
				txtBox = null;
			}

			if (txtHigh != null) {
				txtHigh.Dispose ();
				txtHigh = null;
			}

			if (txtLow != null) {
				txtLow.Dispose ();
				txtLow = null;
			}

			if (txtView != null) {
				txtView.Dispose ();
				txtView = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
