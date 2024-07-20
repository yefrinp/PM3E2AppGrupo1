using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace PM2E2Grupo1.Controllers
{
    public class Base64ToImage : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            ImageSource imageSource = null;

            if (value != null)
            {
                string base64Image = (string)value;
                byte[] imageBytes = System.Convert.FromBase64String(base64Image);
                var stream = new MemoryStream(imageBytes);
                imageSource = ImageSource.FromStream(() => stream);
            }

            return imageSource;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
