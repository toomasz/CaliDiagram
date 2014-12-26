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
    /// Interaction logic for DiagramBaseView.xaml
    /// </summary>
    public partial class DiagramBaseView : UserControl
    {
        public DiagramBaseView()
        {
            InitializeComponent();
            Loaded += DiagramBaseView_Loaded;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(Control_MouseMove);
            DataContextChanged += TreeEntityView_DataContextChanged;
            
        }

        protected bool isDragging;
        private Point clickPosition;
        private DiagramBaseViewModel vm;
        void DiagramBaseView_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as DiagramBaseViewModel;
            if (vm != null)
            {
                vm.Width = ActualWidth;
                vm.Height = ActualHeight;

                vm.RaiseBindingComplete();
               

                vm.LocationChanged += vm_LocationChanged;
                UpdateGuiLocation(vm.X, vm.Y);
            }
        }

        void vm_LocationChanged(object sender, EventArgs e)
        {
            UpdateGuiLocation(vm.X, vm.Y);
        }
        /// <summary>
        /// Gets or sets additional content for the UserControl
        /// </summary>
        public object AdditionalContent
        {
            get { return (object)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }
        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(DiagramBaseView),
              new PropertyMetadata(null));


        void TreeEntityView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as DiagramBaseViewModel;
            if (vm != null)
                UpdateGuiLocation(vm.X, vm.Y);
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var draggableControl = sender as UserControl;
            clickPosition = e.GetPosition(this);
            draggableControl.CaptureMouse();
            //e.Handled = true;
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggable = sender as UserControl;
            draggable.ReleaseMouseCapture();
        }


        void UpdateGuiLocation(double x, double y)
        {
            RenderTransform = new TranslateTransform(x, y);
        }



        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as UserControl;

            if (isDragging && draggableControl != null)
            {
                var parent = VisualTreeHelper.GetParent(this);
                Mouse.Capture(this, CaptureMode.SubTree);
                Point currentPosition = e.GetPosition(parent as UIElement);

                var transform = draggableControl.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableControl.RenderTransform = transform;
                }

                double x = currentPosition.X - clickPosition.X;
                double y = currentPosition.Y - clickPosition.Y;

                //   transform.X = x;
                //    transform.Y = y;

                var vm = DataContext as DiagramBaseViewModel;

                // UpdateGuiLocation(x, y);
                if (vm != null)
                    vm.SetLocation(x, y);
                //Console.Beep(3000, 40);
            }
        }
    }
}
