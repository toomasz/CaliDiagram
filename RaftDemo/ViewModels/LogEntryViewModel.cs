using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using RaftAlgorithm;

namespace RaftDemo.ViewModels
{
    public class LogEntryViewModel : PropertyChangedBase
    {
        public LogEntryViewModel(LogEntry<string> entry)
        {
            this.Entry = entry;
        }
        public LogEntry<string> Entry
        {
            get;
            private set;
        }
        private Thickness _CellMargin;
        public Thickness CellMargin
        {
            get { return _CellMargin; }
            set
            {
                if (_CellMargin != value)
                {
                    _CellMargin = value;
                    NotifyOfPropertyChange(() => CellMargin);
                }
            }
        }

        private Brush _EntryColor;
        public Brush EntryColor
        {
            get { return _EntryColor; }
            set
            {
                if (_EntryColor != value)
                {
                    _EntryColor = value;
                    NotifyOfPropertyChange(() => EntryColor);
                }
            }
        }

        public int CommitIndex
        {
            get
            {
                return Entry.CommitIndex;
            }
        }
        public int Term
        {
            get
            {
                return Entry.Term;
            }
        }
        public string Data
        {
            get
            {
                if (Entry.Data == null)
                    return "[null]";
                return Entry.Data.ToString();
            }
        }
    }
}
