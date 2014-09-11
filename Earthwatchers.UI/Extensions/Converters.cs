using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Earthwatchers.UI.Extensions
{
    public class CommentDeleteVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int id;
            if (int.TryParse(value.ToString(), out id))
            {
                if (id == Current.Instance.Earthwatcher.Id)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CurrentRankingBoldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int id;
            if (int.TryParse(value.ToString(), out id))
            {
                if (id == 1)
                {
                    return FontWeights.Bold;
                }
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TranslateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Earthwatchers.UI.Resources.Labels));
            return rm.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LabelsConverter : System.Windows.Data.IValueConverter
    {
        public LabelsConverter()
        {
        }

        public object Convert(object value, Type targetType,
                              object parameter, System.Globalization.CultureInfo culture)
        {
            string formatString = value.ToString();

            // Escape special XML characters like <>&'
            formatString = new System.Xml.Linq.XText(formatString).ToString();

            string formatted = String.Format(formatString,
                  @"<TextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>",
                  "</TextBlock>", "<Run FontWeight='bold' Text='", @"'/>",
                      "<Run Text='", @"'/>");
            TextBlock block = (TextBlock)System.Windows.Markup.XamlReader.Load(formatted);
            return block;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("The FormatConverter.ConvertBack method is not supported.");
        }
    }

    public static class ControlFinder
    {
        public static T FindParent<T>(UIElement control) where T : UIElement
        {
            UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
            if (p != null)
            {
                if (p is T)
                    return p as T;
                else
                    return ControlFinder.FindParent<T>(p);
            }
            return null;
        }
    }
}
