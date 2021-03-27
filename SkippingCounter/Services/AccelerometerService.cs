using System;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Essentials;

namespace SkippingCounter.Services
{
    public class AccelerometerService : IAccelerometer
    {
        float _jumpThreshold = 2; // TODO: Should be configurable

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
                    var length = acc.Length();
                    if (length > _jumpThreshold && length > (peakLength ?? float.MinValue))
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

                return Disposable.Create(receiver.Dispose);
            });

        public IObservable<Vector3> OnReadingChanged() =>
            Observable.FromEventPattern<AccelerometerChangedEventArgs>(
                    h => Accelerometer.ReadingChanged += h,
                    h => Accelerometer.ReadingChanged -= h)
                .Select(x => x.EventArgs.Reading.Acceleration);
    }
}
