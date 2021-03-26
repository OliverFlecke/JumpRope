using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkippingCounter.ViewModels
{
    public class SkippingCounterViewModel : BaseViewModel
    {
        const SensorSpeed Speed = SensorSpeed.Fastest;

        int _jumpCount;
        bool _registered;

        public SkippingCounterViewModel()
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;

            StartCountingCmd = new Command(StartCounting);
            StopCountingCmd = new Command(StopCounting);
        }

        public int JumpCount { get => _jumpCount; set => SetProperty(ref _jumpCount, value); }

        public bool IsCounting => Accelerometer.IsMonitoring;

        public ICommand StartCountingCmd { get; }

        public ICommand StopCountingCmd { get; }

        void StartCounting()
        {
            if (IsCounting) return;

            Accelerometer.Start(Speed);
            RaisePropertyChanged(nameof(IsCounting));
        }

        void StopCounting()
        {
            if (!IsCounting) return;

            Accelerometer.Stop();
            RaisePropertyChanged(nameof(IsCounting));
        }

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var length = e.Reading.Acceleration.Length();
            if (length > 2 && !_registered)
            {
                _registered = true;
                JumpCount++;
                Console.WriteLine($"Jump #{_jumpCount} {length}");
            }
            else if (length < 1)
            {
                _registered = false;
            }
        }
    }
}
