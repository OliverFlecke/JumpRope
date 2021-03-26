using System.IO;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using UIKit;
using Xamarin.Essentials;

namespace SkippingCounter.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(RegisterServices));

            return base.FinishedLaunching(app, options);
        }

        void RegisterServices(IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.NSLog(LogEventLevel.Debug, Constants.SerialLogTemplate)
                .MinimumLevel.Is(LogEventLevel.Verbose)
                .WriteTo.File(
                    Path.Combine(FileSystem.AppDataDirectory, "logs", "app.log"),
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 5 * 1048576, // 5 MB,
                    retainedFileCountLimit: 5)
                .CreateLogger();
            services.AddSingleton<ILogger>(logger);
        }
    }
}
