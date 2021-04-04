using System;
using System.Numerics;
using Xamarin.Essentials;

namespace SkippingCounter
{
    public interface IAccelerometer
    {
        float JumpThreshold { get; set; }

        bool IsMonitoring { get; }

        void Start(SensorSpeed speed);

        void Stop();

        IObservable<Vector3> OnReadingChanged();

        IObservable<Vector3> OnJump();
    }
}
