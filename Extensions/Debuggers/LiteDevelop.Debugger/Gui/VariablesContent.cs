using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDevelop.Debugger.Net;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Debugger.Gui
{
    public class VariablesContent : DebuggerToolWindow
    {
        private VariablesControl _variablesControl;

        public VariablesContent()
        {
            this.Text = "Variables";
            this.Control = _variablesControl = new VariablesControl()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
            };
            this.DockState = LiteToolWindowDockState.BottomAutoHide;

            var iconProvider = IconProvider.GetProvider<AssemblyIconProvider>();
            var bmp = new Bitmap(iconProvider.ImageList.Images[iconProvider.GetImageIndex(typeof(System.Reflection.FieldInfo))]);
            this.Icon = Icon.FromHandle(bmp.GetHicon());

            DebuggerBase.Instance.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);
        }

        protected override void OnCurrentFrameChanged(EventArgs e)
        {
            if (DebuggerBase.Instance.CurrentFrame != null)
                _variablesControl.SetCurrentFrame(DebuggerBase.Instance.CurrentFrame);
        }

        protected override void OnDebuggerResumed(EventArgs e)
        {
            _variablesControl.SetCurrentFrame(null);
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            DebuggerBase.Instance.MuiProcessor.TryApplyLanguageOnComponent(this, "VariablesContent.Title");
        }
    }
}
