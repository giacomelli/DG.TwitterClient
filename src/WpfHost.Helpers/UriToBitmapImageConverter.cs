using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Globalization;

namespace DG.TwitterClient.WpfHost.Helpers
{
    public class UriToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             CultureInfo culture)
        {
            BitmapImage image = new BitmapImage();
            if (value != null)
            {
                try
                {
                    image.DownloadCompleted += new EventHandler(image_DownloadCompleted);
                    image.DownloadProgress += new EventHandler<DownloadProgressEventArgs>(image_DownloadProgress);
                    image.DownloadFailed += new EventHandler<System.Windows.Media.ExceptionEventArgs>(image_DownloadFailed);                    
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.UriSource = new Uri((string)value, UriKind.Absolute);
                    image.EndInit();                    
                }
                catch
                {
                    image = null;
                }
            }

            return image;
        }

        void image_DownloadFailed(object sender, System.Windows.Media.ExceptionEventArgs e)
        {
            
        }

        void image_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            
        }

        void image_DownloadCompleted(object sender, EventArgs e)
        {
            
        }

        public object ConvertBack(object value, Type targetType, object parameter,
               CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
