using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Services;
using Serilog;
using System.Timers;


namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _productCount;
        private int _badProductCount;
        private Brush _rfReaderColor;
        private Brush _ioModuleColor;
        private Brush _plcColor;

        private RFIDReader _rfidReader;
        private DioHandler _dioHandler;
        private bool _uidDetected; // Flag to track UID detection

        private int _productsPerMinute; // New property for speed
        private Timer _timer; // Timer for counting speed
        private int _productCountLastMinute; // Count of products in the last minute

        private DateTime? _lastUpEdgeTime;
        private List<double> _partTimes = new List<double>();

        public MainViewModel()
        {
            // Initialize default values
            ProductCount =26000;
            BadProductCount = 0;
            RFReaderColor = Brushes.Red; // Default color
            IOModuleColor = Brushes.Red; // Default color
            PLCColor = Brushes.Red;      // Default color
            ProductsPerMinute = 0; // Initialize products per minute

            // Initialize Timer
            _timer = new Timer(60000); // Set the timer for 1 minute (60000 ms)
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start(); // Start the timer

            //Initialize RFID reader and start listening
            _rfidReader = new RFIDReader("192.168.72.223", 10001);
            _rfidReader.OnUIDRead += UIDReadHandler;
            _rfidReader.ConnectionStatusChanged += OnRFIDConnectionStatusChanged;
            _rfidReader.Connect();

            //// Initialize DioHandler
            _dioHandler = new DioHandler();
            _dioHandler.RisingEdgeSignal1 += OnDioUPedge;
            _dioHandler.FallingEdgeSignal1 += OnDioDOWNedge;
            _dioHandler.ConnectionStatusChanged += OnDioConnectionStatusChanged;
            _dioHandler.Connect(1);

            // Commands
            ResetCommand = new RelayCommand(ResetCounters);
            SettingsCommand = new RelayCommand(OpenSettings);
            DataReportsCommand = new RelayCommand(OpenDataReports);
            AboutCommand = new RelayCommand(OpenAbout);
        }

        // Handler for when a new UID is read
        private void UIDReadHandler(string uid)
        {
            Log.Information(" ");
            Log.Information("RFID UID Detected > " + uid);

            _uidDetected = true; // Set flag when UID is read**  // <-- Modified Line
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Update products per minute every minute
            ProductsPerMinute = _productCountLastMinute; // Set the current count as products per minute
            _productCountLastMinute = 0; // Reset the count for the next minute
        }



        private void OnRFIDConnectionStatusChanged(bool isConnected)
        {
            Log.Information("RFID reader connected > True");
            RFReaderColor = isConnected ? Brushes.Green : Brushes.Red;
        }

        private void OnDioUPedge()
        {
            ////Log.Information("----------> befor up ");
            //// Check if a UID was detected during the period
            //if (_uidDetected)   // <-- Modified Line
            //{
            //    ProductCount++;
            //    Log.Information("Good product detected. Product Count: " + ProductCount);
            //}
            //else   // <-- Modified Line
            //{
            //    BadProductCount++;
            //    Log.Information("Bad product detected. Bad Product Count: " + BadProductCount);
            //}
            //// Start monitoring for a UID on rising edge
            //_uidDetected = false; // Reset the flag at the start**  // <-- Modified Line
            //_productCountLastMinute++; // Increment the minute product count
            //Log.Information("----------> up  Object detected, monitoring for UID...");

            _productCountLastMinute++; // Increment the minute product count
        }


        private void OnDioDOWNedge()
        {
            Log.Information("----------> down ");
           // Check if a UID was detected during the period
            if (_uidDetected)   // <-- Modified Line
            {
                ProductCount++;
                Log.Information("Good product detected. Product Count: " + ProductCount);
            }
            else   // <-- Modified Line
            {
                BadProductCount++;
                Log.Information("Bad product detected. Bad Product Count: " + BadProductCount);
            }
            _uidDetected = false; // Reset the flag at the start**  // <-- Modified Line
          //  Log.Information("----------> up  Object detected, monitoring for UID...");
        }
        private void OnDioConnectionStatusChanged(bool isConnected)
        {
            if(isConnected) {
            Log.Information("IO Module connected > True");
            IOModuleColor = isConnected ? Brushes.Green : Brushes.Red;
            }
        }

        // New property for products per minute
        public int ProductsPerMinute
        {
            get { return _productsPerMinute; }
            private set
            {
                _productsPerMinute = value;
                OnPropertyChanged(nameof(ProductsPerMinute));
            }
        }

        // Product Counter for good products
        public int ProductCount
        {
            get { return _productCount; }
            set
            {
                _productCount = value;
                OnPropertyChanged(nameof(ProductCount));
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

        // RF Reader Color
        public Brush RFReaderColor
        {
            get { return _rfReaderColor; }
            set
            {
                _rfReaderColor = value;
                OnPropertyChanged(nameof(RFReaderColor));
            }
        }

        // IO Module Color
        public Brush IOModuleColor
        {
            get { return _ioModuleColor; }
            set
            {
                _ioModuleColor = value;
                OnPropertyChanged(nameof(IOModuleColor));
            }
        }

        // PLC Color
        public Brush PLCColor
        {
            get { return _plcColor; }
            set
            {
                _plcColor = value;
                OnPropertyChanged(nameof(PLCColor));
            }
        }

        // Command for Reset button
        public ICommand ResetCommand { get; }

        // Command for Settings button
        public ICommand SettingsCommand { get; }

        // Command for Data Reports button
        public ICommand DataReportsCommand { get; }

        // Command for About button
        public ICommand AboutCommand { get; }

        // Reset counters for products
        private void ResetCounters(object obj)
        {
            Log.Information("Counters reset");
            ProductCount = 0;
            BadProductCount = 0;
            ProductsPerMinute = 0;
        }

        // Open Settings (implement the functionality as needed)
        private void OpenSettings(object obj)
        {
            // Open settings window or panel
        }

        // Open Data Reports (implement the functionality as needed)
        private void OpenDataReports(object obj)
        {
            // Open data reports window or panel
        }

        // Open About (implement the functionality as needed)
        private void OpenAbout(object obj)
        {
            // Open about window or panel
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
