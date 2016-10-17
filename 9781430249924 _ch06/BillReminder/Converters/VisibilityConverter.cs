using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BillReminder.Converters
{

    //http://stackoverflow.com/questions/2926451/silverlight-bind-to-inverse-of-boolean-property-value
    public class VisibilityConverter :  BoolToValueConverter<Visibility>
    {
        public object Convert(object value, Type type, object parameter, string language)
        {
            bool visibility = (bool)value;
            return visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type type, object parameter, string language)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }

    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type type, object parameter, string language)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type type, object parameter, string language)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
}
