using Serilog;
using Xamarin.Essentials;

namespace SkippingCounter.ViewModels
{
    public class PreferenceViewModel : BaseViewModel
    {
        readonly IAccelerometer _accelerometer;

        float _jumpThreshold = Preferences.Get(Constants.PreferenceKeys.JumpThreshold, Constants.Defaults.JumpThreshold);

        public PreferenceViewModel(
            ILogger logger,
            IAccelerometer accelerometer)
            : base(logger)
        {
            _accelerometer = accelerometer;
        }

        public float JumpThreshold
        {
            get => _jumpThreshold;
            set
            {
                _jumpThreshold = value;
                _accelerometer.JumpThreshold = value;
                Preferences.Set(Constants.PreferenceKeys.JumpThreshold, value);
            }
        }
    }
}
