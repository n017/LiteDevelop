using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LiteDevelop.Debugger.Gui;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Debugger.Net.Disassembler.Gui
{
    public class MsilInstructionsContent : DebuggerToolWindow
    {
        private MsilInstructionsControl _msilControl;
        private ManualResetEvent _switchFrames = new ManualResetEvent(false);

        public MsilInstructionsContent()
        {
            Text = "Disassembly (MSIL)";
            Icon = System.Drawing.Icon.FromHandle(Properties.Resources.icon_read.GetHicon());
            Control = _msilControl = new MsilInstructionsControl()
            {
                Dock = DockStyle.Fill
            };
        }

        protected override void OnDebuggerResumed(EventArgs e)
        {
            _msilControl.SetCurrentFrame(null);
            base.OnDebuggerResumed(e);
        }

        protected override void OnCurrentFrameChanged(EventArgs e)
        {
            if (DebuggerBase.Instance.CurrentFrame != null)
                _msilControl.SetCurrentFrame(DebuggerBase.Instance.CurrentFrame);
            base.OnCurrentFrameChanged(e);

        }
    }
}
