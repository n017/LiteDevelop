using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LiteDevelop.Framework.Gui
{
    public abstract class LiteToolWindow : LiteViewContent
    {
        public event EventHandler DockStateChanged;

        private LiteToolWindowDockState _dockState;

        public LiteToolWindowDockState DockState
        {
            get { return _dockState; }
            set
            {
                if (_dockState != value)
                {
                    _dockState = value;
                    OnDockStateChanged(EventArgs.Empty);
                }
            }
        }

        public virtual string GetPersistName()
        {
            return GetType().FullName;
        }

        protected virtual void OnDockStateChanged(EventArgs e)
        {
            if (DockStateChanged != null)
                DockStateChanged(this, e);
        }

       
    }
}
