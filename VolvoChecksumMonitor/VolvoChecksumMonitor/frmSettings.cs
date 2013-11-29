using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utility.ModifyRegistry;
using Microsoft.Win32;

namespace VolvoChecksumMonitor
{
    public partial class frmSettings : Form
    {
        ModifyRegistry regedit;
        String updateDirectory;
        bool update;
        bool startup;

        public frmSettings()
        {
            InitializeComponent();
            regedit = new ModifyRegistry();
            parseRegistryVals();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            txtFile.Text = updateDirectory;
            chkUpdate.Checked = update;
            chkStartup.Checked = startup;
        }

        private void parseRegistryVals()
        {
            regedit.BaseRegistryKey = Registry.CurrentUser;
            regedit.SubKey = AppController.APP_KEY;
            updateDirectory = regedit.Read("UpdateDirectory");

            String tmpUpdate = regedit.Read("Update");
            if (tmpUpdate == "1")
                update = true;
            else
                update = false;

            regedit.SubKey = AppController.HKCU_RUN;

            if (regedit.Read(Application.ProductName) != null)
                startup = true;
            else
                startup = false;

        }

        private void cmdChooseFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFile.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            String dir = txtFile.Text;
            String update;
            if (chkUpdate.Checked)
                update = "1";
            else
                update = "0";

            regedit.SubKey = AppController.APP_KEY;
            
            regedit.Write("UpdateDirectory", dir);
            regedit.Write("Update", update);

            regedit.SubKey = AppController.HKCU_RUN;

            if (chkStartup.Checked)
            {
                regedit.Write(Application.ProductName, Application.ExecutablePath);
            }
            else
            {
                regedit.DeleteKey(Application.ProductName);
            }
            this.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
