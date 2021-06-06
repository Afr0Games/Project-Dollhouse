using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Security.Principal;
using Dropbox.Api;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PD_Installer
{
    public enum UIState
    {
        Downloading,
        DownloadedTSO,
        InstallingTSO,
        InstalledTSO
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: Change this for release of PD!
        private static string m_PDEXECUTABLE = "GonzoTest.exe";

        private static object m_StartInstallationLocker = new object(), m_PDInstallationEndedLocker = new object(),
            m_TSODownloadStartedLocker = new object();
        private static bool m_StartInstallation = false, m_PDInstallationEnded = false, 
            m_TSODownloadStarted = false;
        private static string m_PDInstallPath = "", m_TSOInstallPath = "";
        private static DropboxClient m_DBClient;

        private CancellationTokenSource m_InstallationTaskController, m_PDDownloadEndTaskController;
        private CancellationToken m_InstallationTaskToken, m_PDDownloadEndTaskToken;
        private Task m_InstallationStartTask, m_PDDownloadEndTask;

        private FileDownloader m_Downloader;

        /// <summary>
        /// Checks if the app was started by an administrator.
        /// </summary>
        public static bool IsAdministrator =>
            new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public MainWindow()
        {
            InitializeComponent();

            if (!IsAdministrator)
            {
                MessageBox.Show("Error", "You need to run this installer as an administrator!", ButtonType.Ok);
                System.Windows.Application.Current.Shutdown();
            }

            m_PDInstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Project Dollhouse\\";

            m_InstallationTaskController = new CancellationTokenSource();
            m_InstallationTaskToken = m_InstallationTaskController.Token;
            m_InstallationStartTask = new Task(new Action(CheckForInstallationStart));
            m_InstallationStartTask.Start();

            m_PDDownloadEndTaskController = new CancellationTokenSource();
            m_PDDownloadEndTaskToken = m_PDDownloadEndTaskController.Token;
            m_PDDownloadEndTask = new Task(new Action(CheckForPDDownloadEnd));
            m_PDDownloadEndTask.Start();

            m_Downloader = new FileDownloader();
            m_Downloader.DownloadProgressChanged += M_Downloader_DownloadProgressChanged;
            m_Downloader.DownloadFileCompleted += M_Downloader_DownloadFileCompleted;

            m_DBClient = new DropboxClient("YSEFXS6AKlkAAAAAAAAAAVWfOVz8nDitm3CbWZVeSy-mabQ0iN3L7j806B-JAR48");
        }

        private void M_Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            InstallTSO();
        }

        #region Installation

        private void InstallPD()
        {
            ZipArchive Archive = new ZipArchive(File.OpenRead(Path.GetTempPath() + "\\PD.zip"), 
                ZipArchiveMode.Read);
            ZipArchiveExtensions.ExtractToDirectory(Archive, m_PDInstallPath, true);
            Archive.Dispose();
            File.Delete(Path.GetTempPath() + "\\PD.zip");
        }

        private void InstallTSO()
        {
            if (File.Exists(Path.GetTempPath() + "\\The Sims Online Setup Files.zip"))
            {
                this.Dispatcher.Invoke(() =>
                {
                    m_TSOInstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + 
                    "\\Maxis\\The Sims Online";
                    var Result = MessageBox.Show("Installation Path", "The default installation path is: " +
                        m_TSOInstallPath + "\r\nDo you wish to change it?", ButtonType.YesNo);

                    if (Result != false)
                    {
                        FolderBrowserDialog FBrowserDiag = new FolderBrowserDialog();
                        if (FBrowserDiag.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                            m_TSOInstallPath = FBrowserDiag.SelectedPath;
                    }

                    UpdateUI(UIState.DownloadedTSO);
                });

                FileStream FStream = File.Open(Path.GetTempPath() + "\\The Sims Online Setup Files.zip",
                    FileMode.Open);
                ZipArchive Archive = new ZipArchive(FStream, ZipArchiveMode.Update);

                //Normalize the path.
                m_TSOInstallPath = Path.GetFullPath(m_TSOInstallPath);

                // Ensures that the last character on the extraction path
                // is the directory separator char.
                // Without this, a malicious zip file could try to traverse outside of the expected
                // extraction path.
                if (!m_TSOInstallPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    m_TSOInstallPath += Path.DirectorySeparatorChar;

                Directory.CreateDirectory(m_TSOInstallPath);
                int TopmostValue = 100; //In percent.
                int TotalCount = 0;
                int EntryCount = 1;

                //Run through the entries and only count actual files, not dirs.
                foreach (ZipArchiveEntry Entry in Archive.Entries)
                {
                    if (Path.HasExtension(m_PDInstallPath + "\\" + Entry.Name))
                        TotalCount++;
                }

                Debug.WriteLine("Number of entries that are files: " + TotalCount);
                Debug.WriteLine("Total number of entries: " + Archive.Entries.Count);

                foreach (ZipArchiveEntry Entry in Archive.Entries)
                {
                    // Gets the full path to ensure that relative segments are removed.
                    string DestinationPath = Path.GetFullPath(Path.Combine(m_TSOInstallPath, Entry.FullName));

                    if (Path.HasExtension(m_TSOInstallPath + "\\" + Entry.Name))
                    {
                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (DestinationPath.StartsWith(m_TSOInstallPath, StringComparison.Ordinal))
                        {
                            Entry.ExtractToFile(DestinationPath, true);
                            EntryCount++;
                        }
                    }
                    else
                        Directory.CreateDirectory(DestinationPath);

                    UpdateUI(UIState.InstallingTSO, ((TopmostValue * EntryCount) / TotalCount));
                }

                FStream.Close();

                WindowsInstaller.CreateRegistry(m_TSOInstallPath);

                this.Dispatcher.Invoke(() =>
                {
                    var Result = MessageBox.Show("Finished", "Installation completed successfully!\r\n " +
                        "Do you want a desktop icon to Project Dollhouse?",
                        ButtonType.YesNo);

                    if (Result == true)
                    {
                        WindowsInstaller.CreateDesktopIcon("ProjectDollhouse", m_PDInstallPath + m_PDEXECUTABLE,
                            m_PDInstallPath);
                    }

                    UpdateUI(UIState.InstalledTSO);
                });
            }
        }

        #endregion

        private void M_Downloader_DownloadProgressChanged(object sender, FileDownloader.DownloadProgress progress)
        {
            // Update progress bar with the percentage.
            this.Dispatcher.Invoke(() =>
            {
                DownloadStatus.Value = progress.ProgressPercentage;
            });
        }

        /// <summary>
        /// Checks if the user clicked the button to start the installation.
        /// </summary>
        private void CheckForInstallationStart()
        {
            while (true)
            {
                if (m_StartInstallation)
                {
                    if(FileDownloader.CheckForInternetConnection())
                        DownloadFiles("PD.zip");
                    else
                    {
                        lock (m_StartInstallationLocker)
                            m_StartInstallation = false;

                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Problem", "There was a problem with your internet connection", 
                                ButtonType.Ok);
                        });
                    }

                    lock (m_StartInstallationLocker)
                    {
                        m_StartInstallation = false;
                        break;
                    }
                }

                //Task was cancelled, probably because the user pressed the exit button.
                if (m_InstallationTaskToken.IsCancellationRequested)
                    break;
            }
        }

        private void CheckForPDDownloadEnd()
        {
            bool Continue = false;

            while (true)
            {
                lock (m_PDInstallationEndedLocker)
                {
                    if (m_PDInstallationEnded)
                    {
                        Debug.WriteLine("PD installation ended!");
                        Continue = true;
                    }
                }

                if(Continue)
                {
                    if (!Directory.Exists(m_PDInstallPath))
                        Directory.CreateDirectory(m_PDInstallPath);

                    if (File.Exists(Path.GetTempPath() + "\\PD.zip"))
                    {
                        InstallPD();
                    }

                    if (!WindowsInstaller.CheckRegistry())
                    {
                        var Result = false;
                        this.Dispatcher.Invoke(() =>
                        {
                            Result = (bool)MessageBox.Show("The Sims Online",
                            "The Sims Online was not installed on your system. " +
                            "\r\nIt is required to run Project Dollhouse. " +
                            "\r\nDo you wish to install it now?");
                        });

                        if (Result == true)
                        {
                            if (!File.Exists(Path.GetTempPath() + "\\The Sims Online Setup Files.zip"))
                            {
                                lock (m_TSODownloadStartedLocker)
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        LblCurrentlyDownloading.Content = "Downloading The Sims Online...";
                                    });

                                    m_Downloader.DownloadFileAsync("https://drive.google.com/file/d/1OmA9ms3PlJRubc9nIchW5mz8hdGrxzyK/view?usp=sharing",
                                        Path.GetTempPath() + "\\The Sims Online Setup Files.zip");
                                    m_TSODownloadStarted = true;
                                }
                            }
                            else
                            {
                                lock (m_TSODownloadStartedLocker)
                                {
                                    if(!m_TSODownloadStarted)
                                        InstallTSO();
                                }
                            }
                        }
                        else
                        {
                            lock (m_PDInstallationEndedLocker)
                            {
                                m_PDInstallationEnded = false;
                                Continue = false;
                            }

                            this.Dispatcher.Invoke(() =>
                            {
                                var Result = MessageBox.Show("Finished", "Installation completed successfully!\r\n " +
                                    "Do you want a desktop icon to Project Dollhouse?",
                                    ButtonType.YesNo);

                                if (Result == true)
                                {
                                    WindowsInstaller.CreateDesktopIcon("Project Dollhouse", 
                                        m_PDInstallPath + m_PDEXECUTABLE, m_PDInstallPath);
                                }

                                UpdateUI(UIState.InstalledTSO);
                            });
                        }

                        lock (m_PDInstallationEndedLocker)
                        {
                            m_PDInstallationEnded = false;
                            Continue = false;
                        }
                    }
                    else //No need to install TSO, so we're #finished!
                    {
                        lock (m_PDInstallationEndedLocker)
                        {
                            m_PDInstallationEnded = false;
                            Continue = false;
                        }

                        this.Dispatcher.Invoke(() =>
                        {
                            var Result = MessageBox.Show("Finished", "Installation completed successfully!\r\n " +
                                "Do you want a desktop icon to Project Dollhouse?",
                                ButtonType.YesNo);

                            if (Result == true)
                            {
                                WindowsInstaller.CreateDesktopIcon("Project Dollhouse", 
                                    m_PDInstallPath + m_PDEXECUTABLE, m_PDInstallPath);
                            }

                            UpdateUI(UIState.InstalledTSO);
                        });
                    }
                }

                //Task was cancelled, probably because the user pressed the exit button.
                if (m_PDDownloadEndTaskToken.IsCancellationRequested)
                    break;
            }
        }

        /// <summary>
        /// The user clicked the exit button.
        /// </summary>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            m_InstallationTaskController.Cancel();
            m_PDDownloadEndTaskController.Cancel();

            m_InstallationTaskController.Dispose();
            m_PDDownloadEndTaskController.Dispose();

            if (m_Downloader != null)
                m_Downloader.Dispose();

            if (m_DBClient != null)
                m_DBClient.Dispose();

            DeleteTempFiles();

            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// The user clicked the install button.
        /// </summary>
        private void BtnInstall_Click(object sender, RoutedEventArgs e)
        {
            var Result = MessageBox.Show("Installation Path", "The default installation path is: " +
                m_PDInstallPath + "\r\nDo you wish to change it?", ButtonType.YesNo);

            if(Result != false)
            {
                FolderBrowserDialog FBrowserDiag = new FolderBrowserDialog();
                if(FBrowserDiag.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                    m_PDInstallPath = FBrowserDiag.SelectedPath;
            }

            UpdateUI(UIState.Downloading);

            lock (m_StartInstallationLocker)
            {
                m_StartInstallation = true;
            }
        }

        /// <summary>
        /// Downloads a file from Dropbox.
        /// </summary>
        /// <param name="FileToDownload">The file to download.</param>
        private async void DownloadFiles(string FileToDownload)
        {
            var Response = await m_DBClient.Files.DownloadAsync("/" + FileToDownload);
            ulong FileSize = Response.Response.Size;
            const int BufferSize = 1024 * 1024;

            byte[] Buffer = new byte[BufferSize];

            using (var ContentStream = await Response.GetContentAsStreamAsync())
            {
                using (var file = new FileStream(Path.GetTempPath() + "\\" + FileToDownload, 
                    FileMode.OpenOrCreate))
                {
                    int Length = ContentStream.Read(Buffer, 0, BufferSize);

                    while (Length > 0)
                    {
                        file.Write(Buffer, 0, Length);
                        var Percentage = 100 * (ulong)file.Length / FileSize;
                        // Update progress bar with the percentage.
                        this.Dispatcher.Invoke(() =>
                        {
                            DownloadStatus.Value = (int)Percentage;
                        });

                        Length = ContentStream.Read(Buffer, 0, BufferSize);
                    }
                }

                if (FileToDownload == "PD.zip")
                {
                    lock (m_PDInstallationEndedLocker)
                    {
                        m_PDInstallationEnded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Clean up temporary files.
        /// </summary>
        private void DeleteTempFiles()
        {
            if (File.Exists(Path.GetTempPath() + "\\PD.zip"))
                File.Delete(Path.GetTempPath() + "\\PD.zip");
            if (File.Exists(Path.GetTempPath() + "\\The Sims Online Setup Files.zip"))
                File.Delete(Path.GetTempPath() + "\\The Sims Online Setup Files.zip");
        }

        /// <summary>
        /// Changes the state of the UI.
        /// </summary>
        /// <param name="State">What is the UI state supposed to be?</param>
        /// <param name="InstallationProgress">The current installation progress, in percent.</param>
        public void UpdateUI(UIState State, int InstallationProgress = 0)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (State)
                {
                    case UIState.Downloading:
                        DownloadStatus.Visibility = Visibility.Visible;
                        TxtDownloadStatus.Visibility = Visibility.Visible;
                        LblDownloadProgress.Visibility = Visibility.Visible;
                        LblCurrentlyDownloading.Visibility = Visibility.Visible;
                        break;
                    case UIState.DownloadedTSO:
                        InstallationStatus.Visibility = Visibility.Visible;
                        LblCurrentlyInstalling.Visibility = Visibility.Visible;

                        LblDownloadProgress.Visibility = Visibility.Hidden;
                        TxtDownloadStatus.Visibility = Visibility.Hidden;
                        DownloadStatus.Visibility = Visibility.Hidden;
                        LblCurrentlyDownloading.Visibility = Visibility.Hidden;
                        break;
                    case UIState.InstallingTSO:
                        InstallationStatus.Value = InstallationProgress;
                        break;
                    case UIState.InstalledTSO:
                        BtnInstall.IsEnabled = false;
                        InstallationStatus.Visibility = Visibility.Hidden;
                        LblCurrentlyInstalling.Visibility = Visibility.Hidden;
                        break;
                }
            });
        }
    }
}
