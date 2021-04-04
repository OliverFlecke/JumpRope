using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SkippingCounter.ViewModels;
using Xamarin.Forms;

namespace SkippingCounter
{
    public static class ViewModelLocator
    {
        static IServiceProvider Container => App.Container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable) =>
            (bool)bindable.GetValue(AutoWireViewModelProperty);

        public static void SetAutoWireViewModel(BindableObject bindable, bool value) =>
            bindable.SetValue(AutoWireViewModelProperty, value);

        public static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<CounterViewModel>();
            services.AddTransient<SessionHistoryViewModel>();
            services.AddTransient<PreferenceViewModel>();
        }

        static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not Element view) return;

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName)
                .Replace("Page", "View");

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType is null)
            {
                Container.GetRequiredService<ILogger>().Warning($"Unable to find view model for '{viewName}'");
                return;
            }

            var viewModel = Container.GetRequiredService(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}
