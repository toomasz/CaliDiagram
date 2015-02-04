using DiagramDesigner.Raft.State;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DiagramDesigner.Converters
{
    public class RaftStateToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Follower)
                return Brushes.Green;
            if (value is Leader)
                return Brushes.DarkRed;
            if(value is Candicate)
                return Brushes.Orange;
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
