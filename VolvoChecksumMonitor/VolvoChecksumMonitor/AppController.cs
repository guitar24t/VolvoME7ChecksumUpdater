using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Utility.ModifyRegistry;
using Microsoft.Win32;

namespace VolvoChecksumMonitor
{
    public partial class AppController : ApplicationContext 
    {
        public static readonly String HKCU_RUN = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static readonly String APP_KEY = "Software\\" + Application.ProductName;

        bool update;
        NotifyIcon notifyIcon;
        ModifyRegistry regedit;
        String updateDirectory;
        System.Timers.Timer timer;
        public AppController()
        {
            MenuItem settingsMenuItem = new MenuItem("Settings", new EventHandler(ShowSettings));
            MenuItem updaterMenuItem = new MenuItem("Single File Updater", new EventHandler(ShowUpdater));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = VolvoChecksumMonitor.Properties.Resources.ME7Flasher;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { settingsMenuItem, updaterMenuItem, exitMenuItem });
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.Visible = true;

            regedit = new ModifyRegistry();
            parseRegistryVals();

            if (updateDirectory == "" || !update)
            {
                frmShowSettings();
            }
            else
            {
                initializeUpdate();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowUpdater(sender, e);
        }

        private void ShowUpdater(object sender, EventArgs e)
        {
            frmMain updater = new frmMain();
            updater.Show();
        }

        private void parseRegistryVals()
        {
            regedit.BaseRegistryKey = Registry.CurrentUser;
            regedit.SubKey = APP_KEY;
            updateDirectory = regedit.Read("UpdateDirectory");
            String tmpUpdate = regedit.Read("Update");
            if (tmpUpdate == "1")
                update = true;
            else
                update = false;
        }

        private void checkRegistryVals()
        {
            parseRegistryVals();
            if (updateDirectory == "" || !update)
            {
                stopUpdate();
            }
            else
            {
                if (timer != null)
                {
                    timer.Start();
                }
                else
                {
                    initializeUpdate();
                }
            }
        }

        private void initializeUpdate()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(timerTick);
            timer.Start();
        }

        private void startUpdate()
        {
            if (timer != null)
                timer.Start();
        }

        private void stopUpdate()
        {
            if (timer != null)
                timer.Stop();
        }

        private void timerTick(object sender, ElapsedEventArgs e)
        {
            stopUpdate();
            updateChecksums();
            checkRegistryVals();
        }

        
        private bool updateChecksums()
        {
            if (Directory.Exists(updateDirectory))
            {
                foreach (string file in Directory.GetFiles(updateDirectory))
                {
                    try
                    {
                        ME7ChecksumUpdater.updateChecksums(file);
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                }
                return true;
            }
            else
                return false;
        }
        /*
         // Test method for only updating newly added files
        private bool updateChecksums()
        {
            if (Directory.Exists(updateDirectory))
            {
                string[] files = Directory.GetFiles(updateDirectory);
                if (oldFiles == null)
                {
                    foreach (string file in files)
                    {
                        try
                        {
                            ME7ChecksumUpdater.Checksum_Volvo_ME7(file, false);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                else
                {
                    foreach (string file in files)
                    {
                        if (Array.IndexOf(oldFiles, file) == -1)
                        {
                            try
                            {
                                ME7ChecksumUpdater.Checksum_Volvo_ME7(file, false);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
                oldFiles = files;
                return true;
            }
            else
                return false;
        }
        */
        void ShowSettings(object sender, EventArgs e)
        {
            frmShowSettings();
        }

        void frmShowSettings()
        {
            frmSettings settings = new frmSettings();
            stopUpdate();
            settings.Show();
            checkRegistryVals();
        }

        void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

    }
}
