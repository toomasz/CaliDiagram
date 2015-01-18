using System.Windows;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DiagramLib.ViewModels
{

    public abstract class NodeBaseViewModel : PropertyChangedBase, IDisposable
    {
        public NodeBaseViewModel()
        {
            AttachPlacement = new AttachDescriptorPlacement(this);
            Connections = new ObservableCollection<ConnectionViewModel>();
        }
        public event EventHandler LocationChanged;
        public event EventHandler SizeChanged;
        
        protected virtual void OnLocationChanged()
        {
        }

        protected virtual void OnSizeChanged()
        {
        }

        protected virtual void OnNodeCreated()
        {

        }
        protected virtual bool OnNodeDeleting()
        {
            return true;
        }

        protected virtual void OnConnectionAdded(ConnectionViewModel connection)
        {

        }
        protected virtual void OnConnectionRemoved(ConnectionViewModel connection)
        {

        }
        internal void RaiseConnectionAdded(ConnectionViewModel connection)
        {
            Connections.Add(connection);
            if(isInitialized)
                OnConnectionAdded(connection);
        }
        internal virtual void RaiseConnectionRemoved(ConnectionViewModel connection)
        {
            Connections.Remove(connection);
            if(isInitialized)
                OnConnectionRemoved(connection);
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
            OnLocationChanged();
            if (LocationChanged != null)
                LocationChanged(this, null);
        }

        public DiagramViewModel ParentDiagram
        {
            get;
            set;
        }

        public void RaiseSizeChanged()
        {
            if (!isInitialized)
                return;
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
            isInitialized = true;
            AttachPlacement.UpdateAttachPoints();
            OnNodeCreated();
        }

        public void UpdateAttachPoints()
        {
            AttachPlacement.UpdateAttachPoints();
        }

        private bool _CanEditName;
        public bool CanEditName
        {
            get { return _CanEditName; }
            set
            {
                if (_CanEditName != value)
                {
                    _CanEditName = value;
                    NotifyOfPropertyChange(() => CanEditName);
                }
            }
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

        public ObservableCollection<ConnectionViewModel> Connections
        {
            get;
            set;
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
        public bool IsDisposed
        {
            get;
            private set;
        }
        public void Dispose()
        {
            IsDisposed = true;
            OnNodeDeleting();
            
        }
    }
}
