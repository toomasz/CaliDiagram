using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using CaliDiagram.ViewModels;
using CaliDiagram.Views;

namespace CaliDiagram
{
    public class CaliDiagramHelpers
    {
        public void InitViewLocator()
        {
            OriginalLocateForModel = ViewLocator.LocateForModel;
            ViewLocator.LocateForModel = LocateForModel;
        }
        private Func<object, DependencyObject, object, UIElement> OriginalLocateForModel;

        private UIElement LocateForModel(object o, DependencyObject dependencyObject, object arg3)
        {
            if (o is NodeBaseViewModel)
            {
                Type type = o.GetType();

                NodeBaseView diagramBaseView = new NodeBaseView();
             //   diagramBaseView.con = "asd";
                if (type.IsSubclassOf(typeof(NodeBaseViewModel)))
                {
                    UIElement element = OriginalLocateForModel(o, dependencyObject, arg3);
                    FrameworkElement fe = element as FrameworkElement;

                    diagramBaseView.Content = fe;
                    ViewModelBinder.Bind(o, element, arg3);
                    return diagramBaseView;
                }
                return diagramBaseView;
            }
            return OriginalLocateForModel(o, dependencyObject, arg3);
        }
    }
}
