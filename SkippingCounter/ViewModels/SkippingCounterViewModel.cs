using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using Microcharts;
using Serilog;
using SkiaSharp;
using SkippingCounter.Models;
using SkippingCounter.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkippingCounter.ViewModels
{
    public class SkippingCounterViewModel : BaseViewModel
    {
        const SensorSpeed Speed = SensorSpeed.Fastest;

        readonly IDataStore<SkippingSession> _skippingStore;

        float _threshold = 2;
        bool _registered;
        DateTimeOffset _start;

        List<TimeSpan> _jumps = new();
        List<Vector3> _measures = new();

        public SkippingCounterViewModel(
            ILogger logger,
            IDataStore<SkippingSession> skippingStore)
            : base(logger)
        {
            _skippingStore = skippingStore;

            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;

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

        public int JumpCount => _jumps.Count;

        public bool IsCounting => Accelerometer.IsMonitoring;

        public Chart? AccelerationChart { get; private set; }

        void StartCounting()
        {
            if (IsCounting) return;

            _start = DateTimeOffset.Now;
            Accelerometer.Start(Speed);
            RaisePropertyChanged(nameof(IsCounting));
        }

        void StopCounting()
        {
            if (!IsCounting) return;

            Accelerometer.Stop();
            RaisePropertyChanged(nameof(IsCounting));

            _skippingStore.AddItemAsync(new SkippingSession(_start, DateTimeOffset.Now, _jumps));
        }

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            _measures.Add(e.Reading.Acceleration);
            var length = e.Reading.Acceleration.Length();
            if (length > _threshold && !_registered)
            {
                _registered = true;

                _jumps.Add(DateTimeOffset.Now.Subtract(_start));
                RaisePropertyChanged(nameof(JumpCount));
            }
            else if (length < 1)
            {
                _registered = false;
            }
        }

        void DrawChart()
        {
            var entries = _measures.Select(x => new ChartEntry(x.Length())
            {
                Label = string.Empty,
                Color = SKColor.Parse("#00FF00"),
            });
            AccelerationChart = new LineChart { Entries = entries };
            RaisePropertyChanged(nameof(AccelerationChart));
        }
    }
}
