using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiteDevelop.Debugger.Net;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Debugger.Gui
{
    public abstract class DebuggerToolWindow : LiteToolWindow
    {
        private ManualResetEvent _updatedEvent = new ManualResetEvent(false);

        public DebuggerToolWindow()
        {
            if (!DebuggerBase.HasInstance)
                throw new InvalidOperationException("Debugger is not initialized yet.");

            UpdateTimeout = 2000;
            DebuggerBase.Instance.Paused += LiteDebugger_Paused;
            DebuggerBase.Instance.Resumed += LiteDebugger_Resumed;
            DebuggerBase.Instance.CurrentFrameChanged +=LiteDebugger_CurrentFrameChanged;
        }

        public int UpdateTimeout
        {
            get;
            set;
        }

        private void LiteDebugger_Paused(object sender, ControllerPauseEventArgs e)
        {
            _updatedEvent.Set();
            OnDebuggerPaused(e);
        }
        
        private void LiteDebugger_Resumed(object sender, EventArgs e)
        {
            _updatedEvent.Reset();
            new Thread(() =>
            {
                if (!_updatedEvent.WaitOne(UpdateTimeout))
                {
                    if (this.Control.IsHandleCreated)
                        this.Control.Invoke(new Action(() => { OnDebuggerResumed(e); }));
                }
            }) { IsBackground = true }.Start();
        }

        private void LiteDebugger_CurrentFrameChanged(object sender, EventArgs e)
        {
            OnCurrentFrameChanged(e);
        }
        
        protected virtual void OnDebuggerPaused(ControllerPauseEventArgs e)
        {
        }

        protected virtual void OnDebuggerResumed(EventArgs e)
        {
        }

        protected virtual void OnCurrentFrameChanged(EventArgs e)
        {
        }

    }
}
