using RaftAlgorithm;
using RaftDemo.Model;
using RaftDemo.Raft;
using RaftDemo.ViewModels;
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

namespace RaftDemo.Views
{
    /// <summary>
    /// Interaction logic for DiagramNodeBigView.xaml
    /// </summary>
    public partial class DiagramNodeServerView : UserControl
    {
        public DiagramNodeServerView()
        {
            InitializeComponent();
            DataContextChanged += DiagramNodeBigView_DataContextChanged;
           // Storyboard sb = this.FindResource("timerStoryboard") as Storyboard;
         //   DoubleAnimation anim = (DoubleAnimation)sb.Children.FirstOrDefault(c => c.Name == "doubleAnimation");
         
        }

      

        void DiagramNodeBigView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as DiagramNodeServerViewModel ;
            if (vm != null)
            {
                RaftHost raftNode = vm.NodeSoftware as RaftHost;
                raftNode.OnRaftEvent += raftNode_OnRaftEvent;
            }
        }

        void raftNode_OnRaftEvent(object sender, RaftEventResult e)
        {
            if(e.TimerSet)
                Dispatcher.BeginInvoke(new Action(()=>StartTimerAnimation(e.TimerValue)));
        }

        void StartTimerAnimation(int ms)
        {
            Storyboard sb = this.FindResource("timerStoryboard") as Storyboard;
            DoubleAnimation anim = (DoubleAnimation)sb.Children.FirstOrDefault(c => c.Name == "doubleAnimation");
            anim.Duration = TimeSpan.FromMilliseconds(ms);
            sb.Begin();
        }

    }
}
