#define LINUX

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;

namespace GonzoTest
{

	#if WINDOWS || LINUX
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (var game = new Game1())
			game.Run();
		}
	}
	#endif
}
