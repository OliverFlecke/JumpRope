using System;
using Serilog;
using SkippingCounter.Resources.Themes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkippingCounter.Services
{
    public class ThemeHandler
    {
        readonly ILogger _logger;

        public ThemeHandler(ILogger logger)
        {
            _logger = logger;

            ChangeTheme(Application.Current.RequestedTheme, true);
            Application.Current.RequestedThemeChanged += ThemeChanged;
        }

        /// <summary>
        /// Gets or sets the current theme of the application. If default,
        /// the app will follow the OS requested theme.
        /// </summary>
        public static OSAppTheme CurrentTheme { get; set; } =
            Enum.Parse<OSAppTheme>(
                Preferences.Get(Constants.PreferenceKeys.Theme, OSAppTheme.Unspecified.ToString()));

        void ThemeChanged(object sender, AppThemeChangedEventArgs e) => ChangeTheme(e.RequestedTheme);

        /// <summary>
        /// Change the theme of the application.
        /// </summary>
        /// <param name="theme">Requested theme.</param>
        /// <param name="forceTheme">Force a theme change.</param>
        public void ChangeTheme(OSAppTheme theme, bool forceTheme = false)
        {
            _logger.Information($"Changing theme to {theme}");

            if (theme == CurrentTheme && !forceTheme) return;

            var applicationResourceDictionary = Application.Current.Resources;
            CurrentTheme = theme;

            ResourceDictionary newTheme = Application.Current.RequestedTheme switch
            {
                OSAppTheme.Dark => new DarkTheme(),
                _ => new LightTheme(), // Defaults to light theme.
            };

            ManuallyCopyThemes(newTheme, applicationResourceDictionary);
        }

        void ManuallyCopyThemes(ResourceDictionary fromResource, ResourceDictionary toResource)
        {
            foreach (var item in fromResource.Keys)
            {
                toResource[item] = fromResource[item];
            }
        }
    }
}
