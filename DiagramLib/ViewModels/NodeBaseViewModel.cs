using System.Windows;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib.ViewModels
{

    public abstract class NodeBaseViewModel : PropertyChangedBase
    {
        public NodeBaseViewModel()
        {
            AttachPlacement = new AttachDescriptorPlacement(this);
        }
        public event EventHandler LocationChanged;
        public event EventHandler SizeChanged;
        public event EventHandler BindingComplete;

        protected virtual void OnLocationChanged()
        {
        }

        protected virtual void OnSizeChanged()
        {
        }
        protected virtual void OnBindingCompleted()
        {
        }

        public Rect Rect
        {
            get
            {
                return new Rect(Location, Size);
            }
        }

        void RaiseLocationChanged()
        {
            if (!isInitialized)
                return;
            // AttachPlacement.UpdatePoints();
          //  Console.WriteLine("{0} NewLocation: {1}", Name, Location);
            OnLocationChanged();
            if (LocationChanged != null)
                LocationChanged(this, null);
        }

        //public void RaiseBindingComplete()
        //{
        //    OnBindingCompleted();
        //    Console.WriteLine(Name + " BC");
        //    // AttachPlacement.UpdatePoints();

        //    if (BindingComplete != null)
        //        BindingComplete(this, null);
            
        //}
        public DiagramViewModel ParentDiagram
        {
            get;
            set;
        }

        public void RaiseSizeChanged()
        {
            if (!isInitialized)
                return;
            //Console.WriteLine(Name + " SC");
            AttachPlacement.UpdateAttachPoints();
            OnSizeChanged();
            if (SizeChanged != null)
                SizeChanged(this, null);
        }
        bool isInitialized = false;
        public void RaiseInitialize()
        {
            if(Size.Width == 0)
            {
                throw new Exception("Raise initialize called on viewmodel not bound to view");
            }
            //Console.WriteLine(Name + " initialized");
            isInitialized = true;
            AttachPlacement.UpdateAttachPoints();
        }

        public void UpdateAttachPoints()
        {
            AttachPlacement.UpdateAttachPoints();
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyOfPropertyChange(() => Name);
                }
            }
        }
        


        private Point _Location;
        public Point Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    RaiseLocationChanged();
                    NotifyOfPropertyChange(() => Location);
                }
            }
        }

        private Size _Size;
        public Size Size
        {
            get { return _Size; }
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    NotifyOfPropertyChange(() => Size);
                    RaiseSizeChanged();
                }
            }
        }
        

        public AttachDescriptorPlacement AttachPlacement
        {
            get; set; 
        }

        public AttachPoint Attach(AttachDirection direction, ConnectionViewModel connection, NodeBaseViewModel node)
        {
            return AttachPlacement.Attach(direction, connection, node);
        }

        public void Detach(AttachPoint ap)
        {
            AttachPlacement.Detach(ap);
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;
    }
}
