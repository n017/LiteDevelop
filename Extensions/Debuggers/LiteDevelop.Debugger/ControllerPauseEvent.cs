using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger
{
    public delegate void ControllerPauseEventHandler(object sender, ControllerPauseEventArgs e);

    public class ControllerPauseEventArgs : PauseEventArgs
    {
        public ControllerPauseEventArgs(IDebuggerController controller, IThread thread, PauseReason reason)
            : base(reason)
        {
            Controller = controller;
            Thread = thread;
        }

        public IDebuggerController Controller
        {
            get;
            private set;
        }

        public IThread Thread
        {
            get;
            private set;
        }
    }
}
