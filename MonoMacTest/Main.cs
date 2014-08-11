using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace MonoMacTest
{

	class MainClass
	{
		static void Main(
			string[] args
		)
		{
			NSApplication.Init();
			NSApplication.Main(args);
		}
	}
}

