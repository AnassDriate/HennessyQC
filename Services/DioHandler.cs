using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;
using Serilog;

namespace WpfApp1.Services
{
    public class DioHandler : IDisposable
    {
        private static readonly InstantDiCtrl _instantDiCtrl = new InstantDiCtrl();
        private readonly byte[] _buffer = new byte[64];
        private bool _sentRise1 = false;
        private bool _sentDown1 = false;
        private bool _run = false;

        public bool Connected { get; private set; }

        public event Action RisingEdgeSignal1;
        public event Action FallingEdgeSignal1;

        // Add a delegate and event for connection status
        public delegate void ConnectionStatusHandler(bool isConnected);
        public event ConnectionStatusHandler ConnectionStatusChanged;



        public DioHandler()
        {
           
        }


        private bool InitializeDevice(int deviceInfo)
        {
            try
            {
                _instantDiCtrl.SelectedDevice = new DeviceInformation(deviceInfo);
                Connected = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to initialize device: {ex.Message}");
                Connected = false;
                return false;
            }
        }

        // Method to start the connection
        public void Connect(int deviceNumber)
        {
            try
            {
                Connected = InitializeDevice(deviceNumber);
                ConnectionStatusChanged?.Invoke(true);

                if (Connected)
                {
                    Start();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"DIO Module connection error: {ex.Message}");
            }
        }

        public void Start()
        {
            if (!_run)
            {
                _run = true;
                Task.Run(() => Loop());
            }
        }

        public void Stop()
        {
            _run = false;
        }

        private void Loop()
        {
            while (_run)
            {
                try
                {
                    _instantDiCtrl.Read(0, 1, _buffer);

                    if (_buffer[0] == 1 && !_sentRise1)
                    {
                        RisingEdgeSignal1?.Invoke();
                        _sentRise1 = true;
                        _sentDown1 = false;
                    }
                    else if (_buffer[0] == 0 && !_sentDown1)
                    {
                        FallingEdgeSignal1?.Invoke();
                        _sentDown1 = true;
                        _sentRise1 = false;
                    }

                   // Task.Delay(5).Wait(); // Small delay to reduce CPU load
                }
                catch (Exception ex)
                {
                    Log.Error($"Usb-5830 failed to read: {ex.Message}");
                    Stop();
                }
            }

            //Dispose();
            Log.Error("Usb5830 Stopped");
        }

        public void Dispose()
        {
            if (Connected)
            {
                _instantDiCtrl.Dispose();
                Connected = false;
            }
        }

    }
}
