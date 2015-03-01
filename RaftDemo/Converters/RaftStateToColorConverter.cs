using RaftAlgorithm;
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
            if (!(value is RaftNodeState))
                return null;

            RaftNodeState state = (RaftNodeState)value;
            if (parameter == null)
            {
                if (state == RaftNodeState.Follower)
                    return Brushes.LightBlue;
                if (state == RaftNodeState.Leader)
                    return Brushes.DarkRed;
                if (state == RaftNodeState.Candidate)
                    return Brushes.Orange;
            }
            else
            {
                if (state == RaftNodeState.Follower)
                    return Brushes.Black;
                if (state == RaftNodeState.Leader)
                    return Brushes.White;
                if (state == RaftNodeState.Candidate)
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
