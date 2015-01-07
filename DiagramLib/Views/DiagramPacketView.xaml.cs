using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramLib.Views
{
    /// <summary>
    /// Interaction logic for DiagramPacketView.xaml
    /// </summary>
    public partial class DiagramPacketView : UserControl
    {
        ConnectionViewModel vm;

        public DiagramPacketView()
        {
            InitializeComponent();
           
            DataContextChanged += DiagramPacketView_DataContextChanged;

        }
        void CreateAnimation()
        {

        }
        void DiagramPacketView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            vm = DataContext as ConnectionViewModel;
            if (vm != null)
            {
                vm.AttachPointFrom.LocationChanged += AttachPointFrom_LocationChanged;
                vm.AttachPointTo.LocationChanged += AttachPointTo_LocationChanged;
            }
        }
        void UpdateAnimation()
        {
            sb.Stop();
            sb.Begin();
            
        }
        void AttachPointTo_LocationChanged(AttachPoint ap, Point location)
        {
            UpdateAnimation();
        }

        void AttachPointFrom_LocationChanged(AttachPoint ap, Point location)
        {
            UpdateAnimation();
        }




    }
}
