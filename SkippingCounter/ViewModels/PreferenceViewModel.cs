using Serilog;
using Xamarin.Essentials;

namespace SkippingCounter.ViewModels
{
    public class PreferenceViewModel : BaseViewModel
    {
        float _jumpThreshold = Preferences.Get(Constants.PreferenceKeys.JumpThreshold, Constants.Defaults.JumpThreshold);

        public PreferenceViewModel(ILogger logger)
            : base(logger)
        {
        }

        public float JumpThreshold
        {
            get => _jumpThreshold;
            set
            {
                _jumpThreshold = value;
                Preferences.Set(Constants.PreferenceKeys.JumpThreshold, value);
            }
        }
    }
}
