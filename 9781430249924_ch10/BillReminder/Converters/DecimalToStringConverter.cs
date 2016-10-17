﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
namespace BillReminder.Converters
{
    public class DecimalToStringConverter: IValueConverter
    {

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            string result = "";

            if (value is Decimal)
            {
                Decimal theValue = (Decimal)value;
                result = string.Format("{0:N}", theValue);
            }
            else
            {
                Decimal theValue = 0;
                result = string.Format("{0:N}", theValue);
            }

            return result;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDecimal(value);
        }
    }
}
