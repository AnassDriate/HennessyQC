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


namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _goodProductCount;
        private int _badProductCount;


        public MainViewModel()
        {
            // Initialize default values
            GoodProductCount = 0;
            BadProductCount = 0;

            // Commands
            ResetCommand = new RelayCommand(ResetCounters);
            StartStop = new RelayCommand(StartStopb);
            AboutCommand = new RelayCommand(OpenAbout);
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


        // Command for Reset button
        public ICommand ResetCommand { get; }

        // Command for Settings button
        public ICommand StartStop { get; }

        public ICommand AboutCommand { get; }

        private void StartStopb(object obj)
        {

        }

        // Reset counters for products
        private void ResetCounters(object obj)
        {
            Log.Information("Counters reset");
            BadProductCount = 0;
            GoodProductCount = 0;
        }
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
