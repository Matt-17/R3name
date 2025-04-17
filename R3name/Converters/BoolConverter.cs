using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace R3name.Converters;

public class BoolConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var b = (bool)value;

        if (targetType == typeof(bool))
        {

        }
        else if (targetType == typeof(double))
        {
            return b ? 1 : 0.4;
        }

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}