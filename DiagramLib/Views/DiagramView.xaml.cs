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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DiagramLib.ViewModels;

namespace DiagramLib.Views
{
    /// <summary>
    /// Interaction logic for DiagramView.xaml
    /// </summary>
    public partial class DiagramView : UserControl
    {
        public DiagramView()
        {
            InitializeComponent();
            diagram.PreviewMouseLeftButtonDown += diagram_PreviewMouseLeftButtonDown;
            diagram.MouseDown += diagram_MouseDown;
        }

        void diagram_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            DiagramViewModel vm = DataContext as DiagramViewModel;
          
        }

        void diagram_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 0)
            {
                //e.Handled = true;
            }
        }
    }
}
