using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.Debugging
{
    public delegate void PauseEventHandler(object sender, PauseEventArgs e);

    public class PauseEventArgs : EventArgs
    {
        public PauseEventArgs(PauseReason reason)
        {
            Reason = reason;
        }

        public PauseReason Reason
        {
            get;
            private set;
        }
    }
}
