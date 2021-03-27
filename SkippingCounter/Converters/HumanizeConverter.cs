using System;
using System.Globalization;
using Humanizer;
using Humanizer.Localisation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkippingCounter
{
    public class HumanizeConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value switch
            {
                DateTime dateTime => dateTime.Humanize(),
                DateTimeOffset dateTimeOffset => dateTimeOffset.Humanize(),
                TimeSpan timeSpan => timeSpan.Humanize(precision: 3, minUnit: TimeUnit.Second),
                _ => value.ToString(),
            }).ApplyCase(LetterCasing.Sentence);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}