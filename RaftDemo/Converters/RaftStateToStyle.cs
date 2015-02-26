using RaftAlgorithm.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RaftDemo.Converters
{
    public class RaftStateToStyle: IValueConverter
    {
        public Style FollowerStyle
        {
            get;
            set;
        }
        public Style LeaderStyle
        {
            get;
            set;
        }
        public Style CandidateStyle
        {
            get;
            set;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Follower)
                return FollowerStyle;
            if (value is Leader)
                return LeaderStyle;
            if (value is Candicate)
                return CandidateStyle;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
