using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace DG.TwitterClient.WpfHost.Helpers
{
    public class TweetByAgeOpacityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dateTime = (DateTime)value;

            TimeSpan diff = (DateTime.Now - dateTime);

            var opacity = (1 / (diff.TotalMinutes / 5));

            if (opacity >= 1)
            {
                opacity = 0.9;
            }
            else if (opacity < 0.05)
            {
                opacity = 0.05;
            }

            return opacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
