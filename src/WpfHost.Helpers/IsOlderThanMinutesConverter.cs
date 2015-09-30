using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace DG.TwitterClient.WpfHost.Helpers
{
    public class IsOlderThanMinutesConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            var olderThanMinutes = System.Convert.ToInt32(parameter, CultureInfo.InvariantCulture);

            TimeSpan diff = (DateTime.Now - dateTime);

            return diff.TotalMinutes > olderThanMinutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
