using Xamarin.Forms;
using Serilog;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SkippingCounter
{
    public partial class App : Application
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static IServiceProvider Container;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        readonly ILogger _logger;

        public App(Action<IServiceCollection>? register = null)
        {
            InitializeComponent();

            var services = new ServiceCollection();

            if (register is not null) register(services);

            ViewModelLocator.RegisterViewModels(services);

            Container = services.BuildServiceProvider();
            _logger = Container.GetRequiredService<ILogger>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            _logger.Information("*** App started ***");
        }

        protected override void OnSleep()
        {
            _logger.Information("*** App sleeping ***");
        }

        protected override void OnResume()
        {
            _logger.Information("*** App resuming ***");
        }
    }
}
