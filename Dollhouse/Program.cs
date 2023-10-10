using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using Files.Manager;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Avalonia.Controls;

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
        static async Task Main()
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
                    var MsgBox = MessageBoxManager.GetMessageBoxStandard("Error",
                        "No Maxis products were found on your system. Do you want to find it manually?",
                        ButtonEnum.YesNo);
                    ButtonResult MsgBoxResult = await MsgBox.ShowAsync();

                    //TODO: Maybe change messagebox to ask "Do you want to download it?"
                    if (MsgBoxResult == ButtonResult.No)
                        Exit = true;
                    else
                    {
                        Window Wnd = new Window();
                        Wnd.Width = 1;
                        Wnd.Height = 1;
                        Wnd.WindowState = WindowState.Minimized;

                        OpenFolderDialog FDialog = new OpenFolderDialog();
                        string SelectedPath = await FDialog.ShowAsync(Wnd);
                        if (SelectedPath != null) //FDialog.ShowAsync returns null if the dialog was cancelled.
                            GlobalSettings.Default.StartupPath = SelectedPath;
                        else
                            Exit = true;
                    }
                }
            }
            else
            {
                var MsgBox = MessageBoxManager.GetMessageBoxStandard("No installation found",
                    "Error: No Maxis products were found on your system!", ButtonEnum.Ok);
                await MsgBox.ShowAsync();
                Exit = true;
            }

            //The minimum supported horizontal resolution is 720.
            if(GlobalSettings.Default.ScreenWidth < 1024 || GlobalSettings.Default.ScreenHeight < 720)
            {
                var MsgBox = MessageBoxManager.GetMessageBoxStandard("Wrong resolution",
                    "The resolution must be minimum 1024 x 768!", ButtonEnum.Ok);
                await MsgBox.ShowAsync();
                Exit = true;
            }

            if (!Exit)
            {
                using (var game = new Game1())
                {
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