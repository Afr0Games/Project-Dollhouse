using System;
using System.Collections.Generic;
using System.Text;

namespace Parlo.Docker
{
#if WINDOWS
    using System.Management;
#endif

    /// <summary>
    /// Helper class for getting processor information.
    /// </summary>
    internal class ProcessorInfo
    {
        /// <summary>
        /// Gets the physical core count of the current machine.
        /// </summary>
        /// <returns>The number of cores as an int.</returns>
        public static int GetPhysicalCoreCount()
        {
            int CoreCount = 0;

#if WINDOWS
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                CoreCount += int.Parse(item["NumberOfCores"].ToString());
            }
#elif LINUX
            CoreCount = GetPhysicalCoreCountLinux();
#else
            CoreCount = Environment.ProcessorCount;
#endif

            return CoreCount;
        }

#if LINUX
    private static int GetPhysicalCoreCountLinux()
    {
        int CoreCount = 0;
        try
        {
            using (System.Diagnostics.Process Proc = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"grep -c ^processor /proc/cpuinfo\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            })
            {
                Proc.Start();
                int.TryParse(Proc.StandardOutput.ReadToEnd(), out CoreCount);
            }
        }
        catch (Exception ex)
        {
            Logger.Log("Error while getting physical core count: " + ex.Message, LogLevel.error);
        }
        return CoreCount;
    }
#endif
    }
}
