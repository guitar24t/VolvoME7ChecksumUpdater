using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VolvoChecksumMonitor
{
    static class ME7ChecksumUpdater
    {

        /*******************************************************************************
         * With modifications, based on:
         *  Routine: Checksum_Volvo_ME7
         *  Input: file_buffer = bin file buffer for checksum calculation 
         *  Output: file_buffer is directly modified
         *  
         *  Author: Salvatore Faro
         *  E-Mail: info@mtx-electronics.com
         *  Website: www.mtx-electronics.com
         * 
         *  License: Dilemma use the same open source license that you are using for your prog        
         ******************************************************************************/

        public static bool Checksum_Volvo_ME7(string filename, bool checkOnly)
        {
            UInt32 buffer_index = 0x1F810;
            UInt32 start_addr;
            UInt32 end_addr;
            UInt32 checksum;
            UInt32 currChecksum;
            UInt32 ComplimentcurrChecksum;
            byte[] file_buffer;
            try
            {
                long binSize = new FileInfo(filename).Length / 1024;
                if (binSize == 512 || binSize == 1024)
                    file_buffer = File.ReadAllBytes(filename);
                else
                    throw new Exception();
            }
            catch (Exception e)
            {
                e.ToString();
                return false;
            }

            bool valid = true;
            do
            {
                // Get the checksum zone start address
                start_addr = ((UInt32)file_buffer[buffer_index + 3] << 24)
                           + ((UInt32)file_buffer[buffer_index + 2] << 16)
                           + ((UInt32)file_buffer[buffer_index + 1] << 8)
                           + (UInt32)file_buffer[buffer_index];

                // Get the checksum zone end address
                end_addr = ((UInt32)file_buffer[buffer_index + 7] << 24)
                         + ((UInt32)file_buffer[buffer_index + 6] << 16)
                         + ((UInt32)file_buffer[buffer_index + 5] << 8)
                         + (UInt32)file_buffer[buffer_index + 4];
                // Calculate the checksum by 32bit sum from star_addr to end_addr
                checksum = 0;
                for (UInt32 addr = start_addr; addr < end_addr; addr += 2)
                    checksum += ((UInt32)file_buffer[addr + 1] << 8) + (UInt32)file_buffer[addr];


                currChecksum = ((UInt32)file_buffer[buffer_index + 11] << 24)
                           + ((UInt32)file_buffer[buffer_index + 10] << 16)
                           + ((UInt32)file_buffer[buffer_index + 9] << 8)
                           + (UInt32)file_buffer[buffer_index + 8];
                ComplimentcurrChecksum = ((UInt32)file_buffer[buffer_index + 15] << 24)
                           + ((UInt32)file_buffer[buffer_index + 14] << 16)
                           + ((UInt32)file_buffer[buffer_index + 13] << 8)
                           + (UInt32)file_buffer[buffer_index + 12];

                if (checksum != currChecksum)
                {
                    valid = false;
                }
                UInt32 complchecksum = ~checksum;
                if (ComplimentcurrChecksum != complchecksum)
                {
                    valid = false;
                }
                if (!checkOnly)
                {
                    try
                    {
                        // Save the new checksum
                        savebytetobinary((int)(buffer_index + 8), (byte)(checksum & 0x000000FF), filename);
                        savebytetobinary((int)(buffer_index + 9), (byte)((checksum & 0x0000FF00) >> 8), filename);
                        savebytetobinary((int)(buffer_index + 10), (byte)((checksum & 0x00FF0000) >> 16), filename);
                        savebytetobinary((int)(buffer_index + 11), (byte)((checksum & 0xFF000000) >> 24), filename);
                        // Save the complement of the new checksum
                        checksum = ~checksum;
                        savebytetobinary((int)(buffer_index + 12), (byte)(checksum & 0x000000FF), filename);
                        savebytetobinary((int)(buffer_index + 13), (byte)((checksum & 0x0000FF00) >> 8), filename);
                        savebytetobinary((int)(buffer_index + 14), (byte)((checksum & 0x00FF0000) >> 16), filename);
                        savebytetobinary((int)(buffer_index + 15), (byte)((checksum & 0xFF000000) >> 24), filename);
                        valid = true;
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                        valid = false;
                        //MessageBox.Show(e.Message.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
                buffer_index += 0x10;
            }
            while (buffer_index < 0x1FA00);
            return valid;
        }

        //Code by Dilemma
        //http://trionic.mobixs.eu/
        //
        private static void savebytetobinary(int address, byte data, string filename)
        {
            FileStream fsi1 = File.OpenWrite(filename);
            BinaryWriter bw1 = new BinaryWriter(fsi1);
            fsi1.Position = address;
            bw1.Write((byte)data);
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();
        }

        public static void updateChecksums(String filename)
        {
            if (Checksum_Volvo_ME7(filename, true))
            {
                return;
            }
            else
            {
                if (!Checksum_Volvo_ME7(filename, false))
                {
                    //MessageBox.Show("Checksums failed to update!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }
    }
}
