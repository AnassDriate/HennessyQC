using labid.NFC;
using labid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace WpfApp1.Services
{
    static internal class Utilities
    {
        public static string doublethecurrant(BitArray cttt)
        {
            // extraction de current 
            string TTs = (cttt[0] ? "1" : "0") + (cttt[1] ? "1" : "0") + (cttt[2] ? "1" : "0") + (cttt[3] ? "1" : "0") + (cttt[4] ? "1" : "0");
            var strb = new StringBuilder(TTs);
            TTs = "1" + strb.ToString() + "11";
            string TT = Convert.ToInt32(TTs, 2).ToString("X");
            if (TT.Length == 1)
                TT = "0" + TT;
            return TT;
        }

        public static string halfthecurrant(BitArray cttt)
        {
            // extraction de current --> to int --> /2 --> to binary 
            string TTs = (cttt[0] ? "1" : "0") + (cttt[1] ? "1" : "0") + (cttt[2] ? "1" : "0") + (cttt[3] ? "1" : "0") + (cttt[4] ? "1" : "0");
            int int_currant = Convert.ToInt32(TTs, 2);
            int hafl_currant = Convert.ToInt32(int_currant / 2);
            byte[] half_curr_byt = BitConverter.GetBytes(hafl_currant);
            string halfcurrbinarystring = Convert.ToString(half_curr_byt[0], 2).PadLeft(5, '0');
            //Console.WriteLine("half currant in string binary : " + halfcurrbinarystring);
            TTs = halfcurrbinarystring;
            var strb = new StringBuilder(TTs);
            TTs = "0" + strb.ToString() + "11";
            string TT = Convert.ToInt32(TTs, 2).ToString("X");
            if (TT.Length == 1)
                TT = "0" + TT;
            return TT;
        }


        public static bool CheckIfClosed(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Log.Information("The URL is null, empty, or whitespace");
                return false; // Return false if the URL is null, empty, or whitespace
            }

            try
            {
                // Ensure the URL contains "?m="
                int index = url.IndexOf("?m=");
                if (index == -1 || index + 3 >= url.Length)
                {
                    Log.Information("\"?m=\" not found or invalid");
                    return false; // "?m=" not found or invalid
                }

                // Extract the part after "?m="
                string parameterString = url.Substring(index + 3);
                if (string.IsNullOrEmpty(parameterString))
                {
                    Log.Information("Nothing after \"?m=\"");
                    return false; // Nothing after "?m="
                }

                // Split by 'x'
                string[] parts = parameterString.Split('x');
                if (parts.Length < 2)
                {
                    Log.Information("Not enough parts for the second-to-last element");
                    return false; // Not enough parts for the second-to-last element
                }

                // Get the second-to-last part
                string extractedPart = parts[parts.Length - 2]; // Use ^2 to access the second-to-last part
                if (string.IsNullOrEmpty(extractedPart) || extractedPart.Length < 2)
                {
                    Log.Information("The extracted part is invalid or too short");
                    return false; // The extracted part is invalid or too short
                }

                // Check if the first letter is 'C' and the second letter is 'C'
                return extractedPart[0] == 'C' && extractedPart[1] == 'C';
            }
            catch (Exception ex)
            {
                Log.Error("Fail check if closed " + ex.Message);
                return false; // Return false on any exception
            }
        }



        public static string ReadUrl(BearsReader reader)
        {
            if (reader == null)
            {
                Log.Information("null , Reader is not initialized");
                return null; // Reader is not initialized
              
            }

            try
            {
                reader.Mifare.rfReset();
                byte[] data = reader.Mifare.ReadUltralightCUserData();
                if (data == null || data.Length == 0)
                {
                    Log.Information("null , data length 0");
                    return null;
                }

                // Parse TLV
                int tlvLen;
                TLV tlv = TLV.Parse(data, 0, data.Length, out tlvLen);
                if (tlv?.Value == null)
                {
                    return null;
                }

                byte[] tlvData = tlv.Value;

                // Parse NDEF message
                NDEFMessage readNdef = NDEFMessage.Parse(tlvData, 0, tlvData.Length);
                if (readNdef?.NdefRecords == null || readNdef.NdefRecords.Length == 0)
                {
                    Log.Information("null , NdefRecords Length 0");
                    return null;
                }

                // Parse URI
                string readUrl = NDEF_URI.ParseURI(readNdef.NdefRecords[0]);
                return string.IsNullOrWhiteSpace(readUrl) ? null : readUrl;
            }
            catch (Exception ex)
            {
                Log.Error("read url fail" + ex.Message);
                return null;
            }
        }


        public static string readchipmem(BearsReader reader)
        {
            string outp = "";
            int page = 0;
            byte[] readed = reader.Mifare.ReadUltralightC(0, 59, false, true);
            for (int i = 0; i < readed.Length; i = i + 4)
            {
                outp = outp + page.ToString("X") + "  " + ByteUtils.toHexString(readed[i]) + " " + ByteUtils.toHexString(readed[i + 1]) + " " +
                ByteUtils.toHexString(readed[i + 2]) + " " + ByteUtils.toHexString(readed[i + 3]) + Environment.NewLine;
                //Console.WriteLine(page.ToString("X") + "  " + ByteUtils.toHexString(readed[i]) + " " + ByteUtils.toHexString(readed[i + 1]) + " " +
                //    ByteUtils.toHexString(readed[i + 2]) + " " + ByteUtils.toHexString(readed[i + 3])); 
                //+"      " + 
                //    ByteUtils.toASCII(new byte[] { readed[i], readed[i+1], readed[i+2], readed[i+3] }));
                page++;
            }
            return outp;
        }

        public static string recursiveFunc(int diff_meas, BitArray cttt)
        {
            // extraction de current 
            string TTs = (cttt[0] ? "1" : "0") + (cttt[1] ? "1" : "0") + (cttt[2] ? "1" : "0") + (cttt[3] ? "1" : "0") + (cttt[4] ? "1" : "0");

            // this is for testing the recursive func
            // string TTs = (cttt[3] ? "1" : "0") + (cttt[4] ? "1" : "0") + (cttt[5] ? "1" : "0") + (cttt[6] ? "1" : "0") + (cttt[7] ? "1" : "0");

            // adjustment of the current trim
            if (diff_meas < 0)
            {
                //put the last one to 0
                var strb = new StringBuilder(TTs);
                for (int i = 4; i >= 0; i--)
                {
                    // if the last element is already 1 it meas we finished the iterations
                    if (strb[4] == '1')
                    {
                        strb.Replace('1', '0', 4, 1);
                        break;
                    }
                    if (strb[i] == '1')
                    {
                        strb.Replace('1', '0', i, 1);
                        strb.Replace('0', '1', i + 1, 1);
                        break;
                    }
                }
                TTs = "0" + strb.ToString() + "11";
                string TT = Convert.ToInt32(TTs, 2).ToString("X");
                if (TT.Length == 1)
                    TT = "0" + TT;
                return TT;
            }
            else
            {
                var strb = new StringBuilder(TTs);
                for (int i = 4; i >= 0; i--)
                {
                    // if the last element is already 1 it meas we finished the iterations
                    if (strb[4] == '1')
                        break;
                    if (strb[i] == '0' && strb[i - 1] == '1')
                    {
                        strb.Replace('0', '1', i, 1);
                        break;
                    }
                }
                TTs = "0" + strb.ToString() + "11";
                string TT = Convert.ToInt32(TTs, 2).ToString("X");
                if (TT.Length == 1)
                    TT = "0" + TT;
                return TT;
            }
        }
    }
}
