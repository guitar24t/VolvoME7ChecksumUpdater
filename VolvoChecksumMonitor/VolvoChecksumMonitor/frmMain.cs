using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VolvoChecksumMonitor
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(frmMain_DragEnter);
            this.DragDrop += new DragEventHandler(frmMain_DragDrop);
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            long binSize = new FileInfo(files[0]).Length / 1024;
            if (binSize == 512 || binSize == 1024)
                txtFile.Text = files[0];
            else
                MessageBox.Show("Incorrect File Size!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            txtFile.Text = openFileDialog.FileName;
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            String filename = txtFile.Text;
            if (filename != "")
            {
                if (ME7ChecksumUpdater.Checksum_Volvo_ME7(filename, true))
                {
                    MessageBox.Show("Checksums Verified OK!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (MessageBox.Show("Checksums Incorrect! Update?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                    {
                        bool retval = ME7ChecksumUpdater.Checksum_Volvo_ME7(filename, false);
                        if (retval)
                        {
                            MessageBox.Show("Checksums updated successfully!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Checksums failed to update!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}
