using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.Mui;

namespace LiteDevelop.Debugger.Net.Gui
{
    public partial class GeneralSettingsControl : SettingsControl
    {
        private readonly Dictionary<object, string> _componentMuiIdentifiers;
        private ILiteExtensionHost _host;
        private MuiProcessor _muiProcessor;
        private DebuggerBaseSettings _settings;

        public GeneralSettingsControl(ILiteExtensionHost host, MuiProcessor muiProcessor, DebuggerBaseSettings settings)
        {
            InitializeComponent();
            _host = host;
            _muiProcessor = muiProcessor;
            _settings = settings;

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {breakOnHandledExceptionCheckBox, "GeneralSettingsControl.BreakOnHandledException"},
            };

            host.UILanguageChanged += host_UILanguageChanged;
            host_UILanguageChanged(null, null);
        }

        public override void LoadUserDefinedPresets()
        {
            breakOnHandledExceptionCheckBox.Checked = _settings.GetValue<bool>("Exceptions.BreakOnHandledException");
        }

        public override void ApplySettings()
        {
            _settings.SetValue<bool>("Exceptions.BreakOnHandledException", breakOnHandledExceptionCheckBox.Checked);
        }

        private void host_UILanguageChanged(object sender, EventArgs e)
        {
            _muiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }
    }
}
