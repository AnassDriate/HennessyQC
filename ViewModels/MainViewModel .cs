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


namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _goodProductCount;
        private int _badProductCount;

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
            string rev_uid = null;
            string p_uid = null;
            while (_isRunning)
            {
                try
                {
                    // Call the ReadUrl function to get the URL
                    string url = Utilities.ReadUrl(reader); // Ensure 'reader' is properly initialized
                    Log.Information("--------- "+url);
                    if (string.IsNullOrEmpty(url))
                    {
                        //Log.Information("No URL read or invalid URL, Dead tag");
                        continue; // Skip to the next iteration if the URL is invalid
                    }
                    /* ntag.nxp.com/223?m=040C5322C81490x000003xCI73401CC0xCEB95EF769811E33 */

                    // Find the starting index of "m="
                    int startIndex = url.IndexOf("m=") + 2; // Start after 'm='
                    if (startIndex > 1) // Ensure 'm=' was found
                    {
                        // Find the position of the next 'x' after 'm='
                        int endIndex = url.IndexOf('x', startIndex);
                        if (endIndex > startIndex)
                        {
                            // Extract the substring between 'm=' and 'x'
                            rev_uid = url.Substring(startIndex, endIndex - startIndex);
                            Log.Information($"Extracted UID: {rev_uid}");
                        }
                        else
                        {
                            Log.Warning("Ending 'x' not found after 'm=', skipping URL.");
                            continue; // Skip if 'x' is not found
                        }
                    }
                    else
                    {
                        Log.Warning("'m=' not found in URL, skipping.");
                        continue; // Skip if 'm=' is not found
                    }

                    // Check if rev_uid equals p_uid
                    if (rev_uid == p_uid)
                    {
                        Log.Information("rev_uid matches p_uid, skipping...");
                        continue; // Skip this iteration if they match
                    }

                    // Check if the product is closed
                    if (Utilities.CheckIfClosed(url))
                    {
                        GoodProductCount++;
                        reader.Mifare.beep();
                        //Application.Current.Dispatcher.Invoke(() =>
                        //{
                        //    var goodFlash = (Storyboard)Application.Current.MainWindow.FindResource("GoodFlashAnimation");
                        //    Storyboard.SetTarget(goodFlash, Application.Current.MainWindow.FindName("GoodEllipse") as FrameworkElement);
                        //    goodFlash.Begin();
                        //});
                    }
                    else
                    {
                        BadProductCount++;
                        for (int i = 0; i < 4; i++)
                        {
                            reader.Mifare.beep();
                        }
                    }

                    p_uid = rev_uid; // Example: assign `rev_uid` to `p_uid`

                    // Simulate processing delay (optional)
                    Thread.Sleep(500); // Adjust the delay as needed
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred in QualityCheck: {ex.Message}");
                }
            }
            Log.Information("Thread stopped.");
        }

        // Reset counters for products
        private void ResetCounters(object obj)
        {
            Log.Information("Counters reseted...");
            BadProductCount = 0;
            GoodProductCount = 0;
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
