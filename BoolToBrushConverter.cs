using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizGame1WPF
{
    /// <summary>
    /// Converts a boolean value to a Brush for UI display.
    /// </summary>
    public class BoolToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Brush (Green for true, Red for false).
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A Brush based on the boolean value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isCorrect = value is bool b && b;
            return isCorrect
                ? new SolidColorBrush(Color.FromRgb(34, 223, 34))   // #22DF22
                : new SolidColorBrush(Color.FromRgb(223, 34, 34));  // #DF2222
        }

        /// <summary>
        /// Not implemented. Converts a Brush back to a boolean value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Throws NotImplementedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}