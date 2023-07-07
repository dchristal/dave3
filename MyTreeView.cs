using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dave3
{
    public class MyTreeView : TreeView
    {
        public MyTreeView()
        {
            this.HideSelection = false;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
        }

        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
    public class CustomTextBox : TextBox
    {
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                // Place a breakpoint here to break when the Text property changes
                base.Text = value;
            }
        }
    }
    
           
    
    public class FilterClass : INotifyPropertyChanged
    {
        private bool _filterStat;

        public bool FilterStat
        {
            get { return _filterStat; }
            set
            {
                if (_filterStat != value)
                {
                    _filterStat = value;
                    OnPropertyChanged("FilterStat");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
