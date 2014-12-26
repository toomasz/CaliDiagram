using System.Linq;
using System.Reflection;
using DiagramDesigner.Views;
using DiagramLib;
using DiagramLib.ViewModels;
using DiagramLib.Views;

namespace DiagramDesigner
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using System.Windows;

    public class AppBootstrapper : BootstrapperBase
    {
        private SimpleContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            Assembly diagramAssembly = typeof (DiagramViewModel).Assembly;
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            return new Assembly[]
            {
               thisAssembly,
               diagramAssembly
            };
        }


        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.PerRequest<IShell, AppViewModel>();

            var diagramHelpers = new DiagramLibHelpers();
            diagramHelpers.InitViewLocator();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}
