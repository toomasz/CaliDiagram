using RaftAlgorithm;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is RaftNodeState))
                return null;

            RaftNodeState state = (RaftNodeState)value;

            if (state == RaftNodeState.Follower)
                return FollowerStyle;
            if (state == RaftNodeState.Leader)
                return LeaderStyle;
            if (state == RaftNodeState.Candidate)
                return CandidateStyle;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
