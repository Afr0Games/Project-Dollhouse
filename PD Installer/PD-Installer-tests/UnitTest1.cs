using System;
using System.IO;
using System.Diagnostics;
using NUnit.Framework;
using PD_Installer;

namespace PD_Installer_tests
{
    public class Tests
    {
        //TODO: Change this for release of PD!
        private static string m_PDEXECUTABLE = "GonzoTest.exe";
        private CrossThreadTestRunner m_CrossThreadRunner = new CrossThreadTestRunner();

        [SetUp]
        public void Setup()
        {
            //Only enable for debugging!
            //Debugger.Launch();
        }

        [Test]
        public void TestShortcut()
        {
            WindowsInstaller.CreateDesktopIcon("Project Dollhouse",
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Project Dollhouse\\" +
                m_PDEXECUTABLE, 
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Project Dollhouse\\");

            Assert.IsTrue(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + 
                "\\Project Dollhouse.lnk"));
        }

        [Test]
        public void TestRegistry()
        {
            /*m_CrossThreadRunner.RunInSTA(
                delegate
                {
                    MainWindow Window = new MainWindow();

                    if (!Window.CheckRegistry())
                    {
                        Window.CreateRegistry();

                        Assert.IsTrue(Window.CheckRegistry());
                    }
                });*/

            if (!WindowsInstaller.CheckRegistry())
            {
                WindowsInstaller.CreateRegistry(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "Maxis\\The Sims Online\\");

                Assert.IsTrue(WindowsInstaller.CheckRegistry());
            }

            Assert.Pass();
        }
    }
}