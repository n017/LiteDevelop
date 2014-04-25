using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Debugger.Gui
{
    public class CallStackContent : DebuggerToolWindow
    {
        private CallStackControl _callStackControl;

        public CallStackContent()
        {
            Text = "Call stack";
            this.Control = _callStackControl = new CallStackControl()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
            };
            this.DockState = LiteToolWindowDockState.BottomAutoHide;
            this.Icon = System.Drawing.Icon.FromHandle(Properties.Resources.stack.GetHicon());

            DebuggerBase.Instance.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);
        }

        protected override void OnDebuggerPaused(ControllerPauseEventArgs e)
        {
            _callStackControl.SetCurrentChain(e.Thread.CurrentChain);
        }

        protected override void OnDebuggerResumed(EventArgs e)
        {
            _callStackControl.SetCurrentChain(null);
        }

        protected override void OnCurrentFrameChanged(EventArgs e)
        {
            _callStackControl.UpdateCurrentFrames();
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            DebuggerBase.Instance.MuiProcessor.TryApplyLanguageOnComponent(this, "CallStack.Title");
        }
    }
}
