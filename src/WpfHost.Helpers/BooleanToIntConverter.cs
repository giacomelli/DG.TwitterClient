using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace DG.TwitterClient.WpfHost.Helpers
{
    public class BooleanToIntConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            var trueIntValue = 0;
            var falseIntValue = 0;

            if (parameter != null)
            {
                var parts = parameter.ToString().Split(',');

                if(parts.Length > 1)
                {
                    trueIntValue = System.Convert.ToInt32(parts[0]);
                    falseIntValue = System.Convert.ToInt32(parts[1]);
                }
            }

            return boolValue ? trueIntValue : falseIntValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
