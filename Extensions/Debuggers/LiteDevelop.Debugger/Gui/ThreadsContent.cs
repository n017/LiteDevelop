using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDevelop.Debugger.Net;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Debugger.Gui
{
    public class ThreadsContent : DebuggerToolWindow
    {
        private ThreadsControl _threadsControl;

        public ThreadsContent()
        {
            this.Text = "Threads";
            this.Control = _threadsControl = new ThreadsControl()
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            this.DockState = LiteToolWindowDockState.BottomAutoHide;
            this.Icon = Icon.FromHandle(Properties.Resources.threads.GetHicon());

            DebuggerBase.Instance.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);
        }

        protected override void OnDebuggerPaused(ControllerPauseEventArgs e)
        {
            _threadsControl.UpdateControl(e.Thread);
        }

        protected override void OnDebuggerResumed(EventArgs e)
        {
            _threadsControl.UpdateControl(null);
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            DebuggerBase.Instance.MuiProcessor.TryApplyLanguageOnComponent(this, "ThreadsContent.Title");
        }
    }
}
