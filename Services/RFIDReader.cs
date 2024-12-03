using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace WpfApp1.ViewModels
{
    public class RFIDReader
    {
        private IPEndPoint remoteEP;
        private Socket socket;
        private byte[] bytes = new byte[1024];
        private CancellationTokenSource cancellationTokenSource;

        public event Action<bool> ConnectionStatusChanged;

        // Event to notify when a new UID is read
        public event Action<string> OnUIDRead;

        // Constructor
        public RFIDReader(string ipAddress, int port)
        {
            remoteEP = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            socket = new Socket(IPAddress.Parse(ipAddress).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            cancellationTokenSource = new CancellationTokenSource();
        }

        // Method to start the connection
        public void Connect()
        {
            try
            {
                socket.Connect(remoteEP);
                ConnectionStatusChanged?.Invoke(true); // Connection successful

                // Start listening for RFID data
                Task.Run(() => StartListening(), cancellationTokenSource.Token);
               // StartListening();
            }
            catch (Exception ex)
            {
                Log.Error("RFID Reader connection > False " + ex.Message);
            }
        }

        // Method to stop the connection
        public void Disconnect()
        {
            cancellationTokenSource.Cancel();
            socket.Close();
        }

        // Method to start listening for RFID data
        private void StartListening()
        {
            try
            {
                int bytesRec;
                byte[] rev_uid;
                byte[] p_uid = new byte[] { 0, 0, 0, 0, 0 };
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        bytesRec = socket.Receive(bytes);
                       // Console.WriteLine("***************Received bytes: " + BitConverter.ToString(bytes).Replace("-", " "));
                        //if (bytes[6] == 52 && bytes[7] == 48) // ntag
                        //{
                        //    rev_uid = bytes.Skip(8).Take(7).ToArray();
                        //    byte[] uid = { rev_uid[6], rev_uid[5], rev_uid[4], rev_uid[3], rev_uid[2], rev_uid[1], rev_uid[0] };
                        //    if (!uid.SequenceEqual(p_uid))
                        //    {
                        //        string uidString = Regex.Replace(BitConverter.ToString(uid), "-", "");
                        //        //p_uid = uid;

                        //        // Notify listeners with the new UID
                        //        OnUIDRead?.Invoke(uidString);
                        //    }
                        //}
                        //else
                        //{
                        //    Log.Error("RFID Reader unknown tag type!");
                        //}

                       // byte[] ctt = { bytes[1], bytes[2] };
                        string cttString = BitConverter.ToString(bytes).Replace("-", "");
                        OnUIDRead?.Invoke(cttString);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("RFID listening Error: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Listening stopped: " + ex.Message);
            }
        }
    }
}
