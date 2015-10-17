using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LiteDevelop.Debugger.Gui;

namespace LiteDevelop.Debugger.Net.Disassembler.Gui
{
    public class X86InstructionsContent : DebuggerToolWindow
    {
        private readonly X86InstructionsControl _x86Control;

        public X86InstructionsContent()
        {
            Text = "Disassembly (x86)";
            Icon = System.Drawing.Icon.FromHandle(Properties.Resources.icon_read.GetHicon());
            Control = _x86Control = new X86InstructionsControl()
            {
                Dock = DockStyle.Fill
            };
        }

        protected override void OnDebuggerResumed(EventArgs e)
        {
            _x86Control.SetCurrentFrame(null);
            base.OnDebuggerResumed(e);
        }

        protected override void OnCurrentFrameChanged(EventArgs e)
        {
            if (DebuggerBase.Instance.CurrentFrame != null)
                _x86Control.SetCurrentFrame(DebuggerBase.Instance.CurrentFrame);
            base.OnCurrentFrameChanged(e);

        }
    }
}
