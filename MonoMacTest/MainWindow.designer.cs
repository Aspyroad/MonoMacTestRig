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
		MonoMac.AppKit.NSButton btnCountPrimes { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnWithBlocking { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnWithBounding { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnWithoutBlocking { get; set; }

		[Outlet]
		MonoMac.AppKit.NSScrollView txtBox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtHigh { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtLow { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView txtView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtWithBounding { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtWithoutBlocking { get; set; }

		[Action ("btn_CountPrimes:")]
		partial void btn_CountPrimes (MonoMac.Foundation.NSObject sender);

		[Action ("btn_WithBlocking:")]
		partial void btn_WithBlocking (MonoMac.Foundation.NSObject sender);

		[Action ("btn_WithBounding:")]
		partial void btn_WithBounding (MonoMac.Foundation.NSObject sender);

		[Action ("btn_WithoutBlocking:")]
		partial void btn_WithoutBlocking (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCountPrimes != null) {
				btnCountPrimes.Dispose ();
				btnCountPrimes = null;
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

			if (btnWithBlocking != null) {
				btnWithBlocking.Dispose ();
				btnWithBlocking = null;
			}

			if (btnWithBounding != null) {
				btnWithBounding.Dispose ();
				btnWithBounding = null;
			}

			if (btnWithoutBlocking != null) {
				btnWithoutBlocking.Dispose ();
				btnWithoutBlocking = null;
			}

			if (txtWithBounding != null) {
				txtWithBounding.Dispose ();
				txtWithBounding = null;
			}

			if (txtWithoutBlocking != null) {
				txtWithoutBlocking.Dispose ();
				txtWithoutBlocking = null;
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
