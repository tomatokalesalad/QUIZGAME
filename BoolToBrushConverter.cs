using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizGame1WPF
{
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCorrect)
            {
                return isCorrect ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}