using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Input;
using Serilog;
using SkippingCounter.Models;
using SkippingCounter.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkippingCounter.ViewModels
{
    public class SkippingCounterViewModel : BaseViewModel
    {
        const SensorSpeed Speed = SensorSpeed.Fastest;

        readonly IAccelerometer _accelerometer;
        readonly IDataStore<SkippingSession> _skippingStore;

        DateTimeOffset _start;
        List<(TimeSpan, Vector3)> _jumps = new();
        int _goal = 10;

        public SkippingCounterViewModel(
            ILogger logger,
            IAccelerometer accelerometer,
            IDataStore<SkippingSession> skippingStore)
            : base(logger)
        {
            _accelerometer = accelerometer;
            _skippingStore = skippingStore;

            _accelerometer.OnJump().Subscribe(WhenJumped);

            StartCountingCmd = new Command(StartCounting);
            StopCountingCmd = new Command(StopCounting);
            ResetCountCmd = new Command(() =>
            {
                _jumps.Clear();
                RaisePropertyChanged(nameof(JumpCount));
            });
        }

        public ICommand StartCountingCmd { get; }

        public ICommand StopCountingCmd { get; }

        public ICommand ResetCountCmd { get; }

        public string Goal
        {
            get => _goal.ToString();
            set
            {
                if (int.TryParse(value, out int g)) SetProperty(ref _goal, g);
            }
        }

        public int JumpCount => _jumps.Count;

        public bool IsCounting => _accelerometer.IsMonitoring;

        void StartCounting()
        {
            _start = DateTimeOffset.Now;
            _accelerometer.Start(Speed);
            RaisePropertyChanged(nameof(IsCounting));
        }

        void StopCounting()
        {
            _accelerometer.Stop();
            RaisePropertyChanged(nameof(IsCounting));

            _skippingStore.AddItemAsync(new SkippingSession(_start, DateTimeOffset.Now, _jumps));
        }

        void WhenJumped(Vector3 vector)
        {
            _jumps.Add((DateTimeOffset.Now.Subtract(_start), vector));
            RaisePropertyChanged(nameof(JumpCount));
            Logger.Debug($"Detected jump #{JumpCount}. Force {vector.Length()}");

            if (JumpCount >= _goal) FireAlarm();
        }

        async void FireAlarm()
        {
            var duration = TimeSpan.FromSeconds(0.5);
            var i = 0;
            while (i <= 5)
            {
                Vibration.Vibrate(duration);
                await Task.Delay(duration);
                i++;
            }
        }
    }
}
