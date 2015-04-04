using CaliDiagram;
using CaliDiagram.ViewModels;
using System.Reflection;

namespace RaftDemo
{
    using Caliburn.Metro;
    using Caliburn.Micro;
    using RaftDemo.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;

    class TextListener : TraceListener
    {

        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
    public class AppBootstrapper : CaliburnMetroCompositionBootstrapper<AppViewModel>
    {
        private SimpleContainer container;

        public AppBootstrapper()
        {
            EnableTrace();
            Initialize();
        }
        void EnableTrace()
        {
            Trace.Listeners.Add(new TextListener());
            Trace.AutoFlush = true;
            Trace.Indent();
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

            var diagramHelpers = new CaliDiagramHelpers();
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
        protected override void OnExit(object sender, EventArgs e)
        {
            var shell = container.GetAllInstances<IShell>();
            foreach (var s in shell)
                s.Close();
        }
    }
}
