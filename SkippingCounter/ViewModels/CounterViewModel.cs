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
    public class CounterViewModel : BaseViewModel
    {
        const SensorSpeed Speed = SensorSpeed.Fastest;

        readonly IAccelerometer _accelerometer;
        readonly IDataStore<SkippingSession> _skippingStore;

        readonly List<(TimeSpan, Vector3)> _jumps = new();

        DateTimeOffset? _start;
        int _goal = Preferences.Get(Constants.PreferenceKeys.JumpGoal, 100);

        public CounterViewModel(
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
            ResetCountCmd = new Command(Reset);
        }

        public ICommand StartCountingCmd { get; }

        public ICommand StopCountingCmd { get; }

        public ICommand ResetCountCmd { get; }

        public string Goal
        {
            get => _goal.ToString();
            set
            {
                if (int.TryParse(value, out var g))
                {
                    SetProperty(ref _goal, g);
                    Preferences.Set(Constants.PreferenceKeys.JumpGoal, g);
                }
            }
        }

        public int JumpCount => _jumps.Count;

        public bool IsCounting => _accelerometer.IsMonitoring;

        void StartCounting()
        {
            if (_start is null) _start = DateTimeOffset.Now;
            _accelerometer.Start(Speed);
            RaisePropertyChanged(nameof(IsCounting));
        }

        void StopCounting()
        {
            _accelerometer.Stop();
            RaisePropertyChanged(nameof(IsCounting));

            if (_start is not null)
                _skippingStore.AddItemAsync(new SkippingSession(_start.Value, DateTimeOffset.Now, _jumps));
        }

        void Reset()
        {
            _jumps.Clear();
            _start = null;
            RaisePropertyChanged(nameof(JumpCount));
        }

        void WhenJumped(Vector3 vector)
        {
            if (_start is null) return;

            _jumps.Add((DateTimeOffset.Now.Subtract(_start.Value), vector));
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
