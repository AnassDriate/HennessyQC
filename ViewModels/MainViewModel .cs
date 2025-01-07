using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Serilog;
using System.Timers;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using labid;
using System.Threading;
using System.Xml.Linq;
using WpfApp1.Services;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Contexts;


namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _goodProductCount;
        private int _badProductCount;
        private string _leftPanelText; // New property for LeftPanelText
        private string _logText;

        private Thread _workerThread; // Thread reference
        private bool _isRunning;      // Status flag
        BearsReader reader = new BearsReader();



        // Command for Reset button
        public ICommand ResetCommand { get; }

        // Command for Settings button
        public ICommand StartStopCommand { get; }

        public ICommand AboutCommand { get; }

        public MainViewModel()
        {
            // Initialize default values
            GoodProductCount = 0;
            BadProductCount = 0;
            LeftPanelText = "Welcome to the Quality Control App";
            LogText = "...";

            // Commands
            ResetCommand = new RelayCommand(ResetCounters);
            StartStopCommand = new RelayCommand(StartStopb);
            AboutCommand = new RelayCommand(OpenAbout);


            try
            {
                reader.Connect();
                Log.Information("Reader connected , application started ....");
            }
            catch (Exception ex)
            {
                Log.Error("RFID Reader connection > False " + ex.Message);
                MessageBox.Show("RFID Reader connection > False", "Exit Message", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
        }

        // Product Counter for bad products
        public int GoodProductCount
        {
            get { return _goodProductCount; }
            set
            {
                _goodProductCount = value;
                OnPropertyChanged(nameof(GoodProductCount));

            }
        }
        // Property for LeftPanelText
        public string LeftPanelText
        {
            get { return _leftPanelText; }
            set
            {
                _leftPanelText = value;
                OnPropertyChanged(nameof(LeftPanelText)); // Notify UI when this changes
            }
        }

        // New property for LogText bound to the TextBox
        public string LogText
        {
            get { return _logText; }
            set
            {
                _logText = value;
                OnPropertyChanged(nameof(LogText)); // Notify UI when this changes
            }
        }

        // Product Counter for bad products
        public int BadProductCount
        {
            get { return _badProductCount; }
            set
            {
                _badProductCount = value;
                OnPropertyChanged(nameof(BadProductCount));
            }
        }

        private string _startStopButtonText = "Start";
        public string StartStopButtonText
        {
            get => _startStopButtonText;
            set
            {
                _startStopButtonText = value;
                OnPropertyChanged(nameof(StartStopButtonText));
            }
        }


        private void StartStopb(object obj)
        {
            if (!_isRunning)
            {
                // Start the thread
                _isRunning = true;
                StartStopButtonText = "Stop"; // Update button text

                _workerThread = new Thread(QualityCheck)
                {
                    IsBackground = true // Ensure the thread stops when the app closes
                };
                _workerThread.Start();
            }
            else
            {
                // Stop the thread
                _isRunning = false;
                StartStopButtonText = "Start"; // Update button text

                if (_workerThread != null && _workerThread.IsAlive)
                {
                    _workerThread.Join(); // Wait for the thread to finish gracefully
                }
            }
        }

        private void QualityCheck()
        {

            while (_isRunning)
            {
                try
                {
                    // Call the ReadUrl function to get the URL
                    string url = Utilities.ReadUrl(reader); // Ensure 'reader' is properly initialized
                    //Log.Information("--------- "+url);
                    if (string.IsNullOrEmpty(url))
                    {
                        //Log.Information("No URL read or invalid URL, Dead tag");
                        continue; // Skip to the next iteration if the URL is invalid
                    }
                    LogText = url + "          Diff Meas " + calcdiffmeas(url);
                    // Check if the product is closed
                    if (Utilities.CheckIfClosed(url))
                    {
                        GoodProductCount++;
                        reader.Mifare.beep();
                    }
                    else
                    {
                        BadProductCount++;
                        for (int i = 0; i < 4; i++)
                        {
                            reader.Mifare.beep();
                        }
                    }

                    LeftPanelText = Utilities.readchipmem(reader);
                    Thread.Sleep(500); // Adjust the delay as needed
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred in QualityCheck: {ex.Message}");
                }
            }
            Log.Information("Thread stopped.");
        }

        private int calcdiffmeas(string url)
        {

            string CTT_hex = "";
            string[] dat = url.Split('x');
            CTT_hex = "" + dat[dat.Length - 2][8] + dat[dat.Length - 2][9] + dat[dat.Length - 2][6] + dat[dat.Length - 2][7] + dat[dat.Length - 2][4] + dat[dat.Length - 2][5] + dat[dat.Length - 2][2] + dat[dat.Length - 2][3] + dat[dat.Length - 2][0] + dat[dat.Length - 2][1];
            StringBuilder cttb = new StringBuilder(CTT_hex);
            if (cttb[8] == 'C')
                cttb[8] = '3';
            if (cttb[9] == 'C')
                cttb[9] = '3';
            if (cttb[8] == 'O')
                cttb[8] = 'F';
            if (cttb[9] == 'O')
                cttb[9] = 'F';
            // ERROR can happens if ctt contains 'I'
            // if ctt contains 'I' replace it with 3 closed
            if (cttb[9] == 'I')
                cttb[9] = '9';
            CTT_hex = cttb.ToString();
            // ----------------------------------------------------------




            //converting the ctt to binary and calculating signed diff_meas
            // ----------------------------------------------------------
            string diff_meas = "";
            BitArray CTT_Binary;
            byte[] ctt = Enumerable.Range(0, CTT_hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(CTT_hex.Substring(x, 2), 16)).ToArray();
            CTT_Binary = new BitArray(ctt, 40);
            //for (int i = 0; i < 40; i++)
            //    Console.WriteLine(CTT_Binary[i] ? "1" : "0");
            diff_meas = (CTT_Binary[19] ? "1" : "0") + (CTT_Binary[20] ? "1" : "0") + (CTT_Binary[21] ? "1" : "0") + (CTT_Binary[22] ? "1" : "0") + (CTT_Binary[23] ? "1" : "0")
                                  + (CTT_Binary[8] ? "1" : "0") + (CTT_Binary[9] ? "1" : "0") + (CTT_Binary[10] ? "1" : "0") + (CTT_Binary[11] ? "1" : "0") + (CTT_Binary[12] ? "1" : "0") + (CTT_Binary[13] ? "1" : "0");
            int sig_diff_mea = (CTT_Binary[18] ? "1" : "0") == "1" ? -Math.Abs(Convert.ToInt32(diff_meas, 2)) : Convert.ToInt32(diff_meas, 2);
            Log.Debug("Diff_Meas : " + sig_diff_mea);
            return sig_diff_mea;
            // ----------------------------------------------------------
        }

        // Reset counters for products
        private void ResetCounters(object obj)
        {
            Log.Information("Counters reseted...");
            BadProductCount = 0;
            GoodProductCount = 0;
            LeftPanelText = string.Empty;
            LogText = string.Empty;

        }
        private void OpenAbout(object obj)
        {
            string aboutMessage = "Hennessy CTT Closures Quality Control Application\n\n" +
                                  "Version: 1.0.0\n" +
                                  "Developer: Anass Driate\n" +
                                  "Contact: driateanass@gmail.com\n\n" +
                                  "This application is designed for quality control of Hennessy CTT closures. " +
                                  "It monitors and controls the permanent and current closed status (CC) of closures, ensuring high standards.";
            MessageBox.Show(aboutMessage, "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
