using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using Files.Manager;

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
            //Controls whether the application is allowed to start.
            bool Exit = false;
            string Software = "";

            if ((is64BitOperatingSystem == false) && (is64BitProcess == false))
                Software = "SOFTWARE";
            else
                Software = "SOFTWARE\\Wow6432Node";

            //Find the path to TSO on the user's system.
            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey(Software);

            if (Array.Exists(softwareKey.GetSubKeyNames(), delegate (string s) { return s.Equals("Maxis", StringComparison.InvariantCultureIgnoreCase); }))
            {
                RegistryKey maxisKey = softwareKey.OpenSubKey("Maxis");
                if (Array.Exists(maxisKey.GetSubKeyNames(), delegate (string s) { return s.Equals("The Sims Online", StringComparison.InvariantCultureIgnoreCase); }))
                {
                    RegistryKey tsoKey = maxisKey.OpenSubKey("The Sims Online");
                    string installDir = (string)tsoKey.GetValue("InstallDir");
                    installDir += "TSOClient\\";
                    GlobalSettings.Default.StartupPath = installDir;
                }
                else
                {
                    //TODO: Maybe change messagebox to ask "Do you want to download it?"
                    if (MessageBox.Show("No Maxis products were found on your system. Do you want to find it manually?", 
                        "Error", MessageBoxButtons.YesNo) == DialogResult.No)
                        Exit = true;
                    else
                    {
                        FolderBrowserDialog FBrowser = new FolderBrowserDialog();
                        if (FBrowser.ShowDialog() == DialogResult.OK)
                            GlobalSettings.Default.StartupPath = FBrowser.SelectedPath;
                        else
                            Exit = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: No Maxis products were found on your system.");
                Exit = true;
            }

            //The minimum supported horizontal resolution is 720.
            if(GlobalSettings.Default.ScreenWidth < 1024 || GlobalSettings.Default.ScreenHeight < 720)
            {
                MessageBox.Show("The resolution must be minimum 1024 x 768!");
                Exit = true;
            }

            if (!Exit)
            {
                using (var game = new Game1())
                {
                    Application.ApplicationExit += Application_ApplicationExit;
                    game.Run();
                }
            }
        }

        /// <summary>
        /// The application is exiting, so dispose of resources.
        /// </summary>
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            FileManager.Instance.Dispose();
        }

        private static bool is64BitProcess = (IntPtr.Size == 8);
        private static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process);

        /// <summary>
        /// Determines if this process is run on a 64bit OS.
        /// </summary>
        /// <returns>True if it is, false otherwise.</returns>
        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
    }
#endif
}