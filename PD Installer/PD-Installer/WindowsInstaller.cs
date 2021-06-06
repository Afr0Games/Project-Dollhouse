using System;
using System.IO;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PD_Installer
{
    /// <summary>
    /// Deals with Windows-specific installer tasks.
    /// </summary>
    public class WindowsInstaller
    {
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
        private static bool InternalCheckIsWow64()
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

        /// <summary>
        /// Creates the registry for a TSO installation.
        /// </summary>
        /// <param name="InstallPath">The path to the TSO installation.</param>
        public static void CreateRegistry(string InstallPath)
        {
            string Software = "";

            if ((is64BitOperatingSystem == false) && (is64BitProcess == false))
                Software = "SOFTWARE";
            else
                Software = "SOFTWARE\\Wow6432Node";

            //Find the path to TSO on the user's system.
            RegistryKey SoftwareKey = Registry.LocalMachine.OpenSubKey(Software, true);

            RegistryKey MaxisKey = SoftwareKey.CreateSubKey("Maxis", true);
            RegistryKey TSOKey = MaxisKey.CreateSubKey("The Sims Online", true);
            TSOKey.SetValue("GameDir", "TSOClient", RegistryValueKind.String);
            TSOKey.SetValue("GameExe", "TSOClient.exe", RegistryValueKind.String);
            //Assumes that TSO has been installed.
            TSOKey.SetValue("InstallDir", InstallPath, RegistryValueKind.String);
            TSOKey.SetValue("PatchDir", "TSOPatch", RegistryValueKind.String);
            TSOKey.SetValue("PatchExe", "TSO.exe", RegistryValueKind.String);
            TSOKey.SetValue("ProgramFolder", "Maxis\\The Sims Online\\", RegistryValueKind.String);
            TSOKey.SetValue("TotalBytes", 0x61c27ce7, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Checks the registry for a TSO installation.
        /// </summary>
        public static bool CheckRegistry()
        {
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
                    return true;
                else
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Creates a desktop icon.
        /// </summary>
        /// <param name="ShortcutName">The name of the shortcut to create-</param>
        /// <param name="ExePath">The path to the executable the shortcut leads to.</param>
        /// <param name="WorkingDir">The directory that the executable resides in.</param>
        public static void CreateDesktopIcon(string ShortcutName, string ExePath, string WorkingDir)
        {
            try
            {
                var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var shell = new WshShell();
                var shortCutLinkFilePath = Path.Combine(startupFolderPath, ShortcutName + ".lnk");
                var windowsApplicationShortcut = (IWshShortcut)shell.CreateShortcut(shortCutLinkFilePath);
                windowsApplicationShortcut.Description = "Project Dollhouse";
                windowsApplicationShortcut.WorkingDirectory = WorkingDir;
                windowsApplicationShortcut.TargetPath = ExePath;
                windowsApplicationShortcut.Save();
            }
            catch(Exception E)
            {
                Debug.WriteLine("Exception in CreateDesktopIcon()\r\n" +
                    E.ToString());
            }
        }
    }
}
