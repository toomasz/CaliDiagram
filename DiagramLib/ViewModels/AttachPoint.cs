using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace DiagramLib.ViewModels
{
    public enum AttachDirection
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
        Center,
        Float
    };

    public delegate void AttachPointLocationChangedDelegate(AttachPoint ap, Point location);

    public delegate void AttachPointDirectionChangedDelegate(AttachPoint ap, AttachDirection direction);

    public class AttachPoint : PropertyChangedBase
    {
        public AttachPoint(AttachDirection direction, ConnectionViewModel connection)
        {
            Direction = direction;
            this.Connection = connection;
        }

        public double Width
        {
            get
            {
                if(Control == null)
                    return 0;
                return Control.Width;
            }
        }

        public double Height
        {
            get
            {
                if(Control == null)
                    return 0;
                return Control.Height;
            }
        }
        public ConnectionViewModel Connection
        {
            get;
            private set;
        }
        private AttachDirection _direction;
        public AttachDirection Direction
        {
            get { return _direction; }
            set
            {
                if (_direction != value)
                {
                    if (DirectionChanging != null)
                        DirectionChanging(this, value);

                    _direction = value;

                }
            }
        }
        public int Order { get; set; }

        public DiagramBaseViewModel Control { get; set; }

        public event AttachPointLocationChangedDelegate LocationChanged;
        public event AttachPointDirectionChangedDelegate DirectionChanging;

        private Point _Location;
        public Point Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    if (LocationChanged != null)
                        LocationChanged(this, Location);
                    NotifyOfPropertyChange(()=>Location);
                }
            }
        }

        public Point ControlLocation { get; set; }
    }
}
