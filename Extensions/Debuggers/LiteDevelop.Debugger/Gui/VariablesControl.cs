using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDevelop.Debugger.Net.Interop.Wrappers;
using System.Threading;

namespace LiteDevelop.Debugger.Gui
{
    public partial class VariablesControl : UserControl
    {

        private readonly Dictionary<object, string> _componentMuiIdentifiers = new Dictionary<object, string>();
        private IFrame _lastFrame;
        private VirtualVariable _exceptionVariable;
        private ListViewItem _exceptionVariableItem;

        public VariablesControl()
        {
            InitializeComponent();

            _exceptionVariable = new VirtualVariable("$exception", NullValue.Instance);
            _exceptionVariableItem = CreateVariableItem(_exceptionVariable, null);

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {columnHeader1, "VariablesContent.ListHeaders.Name"},
                {columnHeader2, "VariablesContent.ListHeaders.Value"},
                {columnHeader3, "VariablesContent.ListHeaders.Type"},
            };

            contextMenuStrip1.Renderer = DebuggerBase.Instance.ExtensionHost.ControlManager.MenuRenderer;
            DebuggerBase.Instance.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);
        }

        public void SetCurrentFrame(IFrame frame)
        {
            if (frame == null)
            {
                _lastFrame = null;
                listView1.Items.Clear();
                return;
            }

            if (_lastFrame == null || _lastFrame.Function != frame.Function)
            {
                listView1.Items.Clear();

                foreach (var localVariable in frame.Function.Symbols.GetVariables())
                    listView1.Items.Add(CreateVariableItem(localVariable, frame));

            }
            else
            {
                foreach (ListViewItem item in listView1.Items)
                    if (item != _exceptionVariableItem)
                        UpdateVariableItem(item, frame);
            }

            _exceptionVariable.SetValue(frame.Thread.CurrentException);

            if (frame.Thread.CurrentException != null && !listView1.Items.Contains(_exceptionVariableItem))
            {
                UpdateVariableItem(_exceptionVariableItem, frame);
                listView1.Items.Insert(0, _exceptionVariableItem);
            }
            else if (frame.Thread.CurrentException == null && listView1.Items.Contains(_exceptionVariableItem))
            {
                _exceptionVariableItem.Remove();
            }

            _lastFrame = frame;
        }

        private ListViewItem CreateVariableItem(IVariable variable, IFrame frame)
        {
            var item = new ListViewItem(new string[4]);
            item.Tag = variable;
            UpdateVariableItem(item, frame);
            return item;
        }

        private void UpdateVariableItem(ListViewItem item, IFrame frame)
        {
            var localVariable = item.Tag as IVariable;
            if (localVariable != null)
            {
                item.SubItems[0].Text = localVariable.Name;

                var value = localVariable.GetValue(frame);
                item.SubItems[1].Text = value.ValueAsString();
                item.SubItems[2].Text = value.Type.ToString();
            }
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            DebuggerBase.Instance.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Clipboard.SetText(listView1.SelectedItems[0].SubItems.Join(", "));
            }
        }
    }
}
