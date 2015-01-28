using DiagramDesigner.Raft;
using DiagramDesigner.ViewModels;
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

namespace DiagramDesigner.Views
{
    /// <summary>
    /// Interaction logic for DiagramNodeBigView.xaml
    /// </summary>
    public partial class DiagramNodeBigView : UserControl
    {
        public DiagramNodeBigView()
        {
            InitializeComponent();
            DataContextChanged += DiagramNodeBigView_DataContextChanged;
            Storyboard sb = this.FindResource("timerStoryboard") as Storyboard;
            DoubleAnimation anim = (DoubleAnimation)sb.Children.FirstOrDefault(c => c.Name == "doubleAnimation");
         
        }

      

        void DiagramNodeBigView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as DiagramNodeBigViewModel ;
            if (vm != null)
            {
                RaftNode raftNode = vm.NodeSoftware as RaftNode;
                raftNode.RaftTimer.TimerSet += RaftTimer_TimerSet;
            }
        }

        void RaftTimer_TimerSet(object sender, int e)
        {
            Storyboard sb = this.FindResource("timerStoryboard") as Storyboard;
            DoubleAnimation anim = (DoubleAnimation)sb.Children.FirstOrDefault(c => c.Name == "doubleAnimation");
            anim.Duration = TimeSpan.FromMilliseconds(e);  
            sb.Begin();
        }

    }
}
