using RaftDemo.Raft.State;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RaftDemo.Converters
{
    public class RaftStateToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                if (value is Follower)
                    return Brushes.LightBlue;
                if (value is Leader)
                    return Brushes.DarkRed;
                if (value is Candicate)
                    return Brushes.Orange;
            }
            else
            {
                if (value is Follower)
                    return Brushes.Black;
                if (value is Leader)
                    return Brushes.White;
                if (value is Candicate)
                    return Brushes.Black;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
