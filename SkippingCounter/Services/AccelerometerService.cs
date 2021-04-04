using System;
using System.Numerics;
using System.Reactive.Linq;
using Xamarin.Essentials;

namespace SkippingCounter.Services
{
    public class AccelerometerService : IAccelerometer
    {
        public float JumpThreshold { get; set; } = Preferences.Get(Constants.PreferenceKeys.JumpThreshold, Constants.Defaults.JumpThreshold);

        public bool IsMonitoring => Accelerometer.IsMonitoring;

        public void Start(SensorSpeed speed)
        {
            if (Accelerometer.IsMonitoring) return;

            Accelerometer.Start(speed);
        }

        public void Stop() => Accelerometer.Stop();

        public IObservable<Vector3> OnJump() =>
            Observable.Create<Vector3>(obs =>
            {
                Vector3? peak = null;
                float? peakLength = null;

                var receiver = OnReadingChanged().Subscribe(acc =>
                {
                    var length = acc.LengthSquared();
                    if (length > JumpThreshold && length > (peakLength ?? float.MinValue))
                    {
                        peak = acc;
                        peakLength = length;
                    }
                    else if (length < 1 && peak is not null)
                    {
                        obs.OnNext(peak.Value);
                        peak = null;
                        peakLength = null;
                    }
                });

                return receiver.Dispose;
            });

        public IObservable<Vector3> OnReadingChanged() =>
            Observable.FromEventPattern<AccelerometerChangedEventArgs>(
                    h => Accelerometer.ReadingChanged += h,
                    h => Accelerometer.ReadingChanged -= h)
                .Select(x => x.EventArgs.Reading.Acceleration);
    }
}
