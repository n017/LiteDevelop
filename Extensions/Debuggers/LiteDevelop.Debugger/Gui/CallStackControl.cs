using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem;
using System.IO;

namespace LiteDevelop.Debugger.Gui
{
    public partial class CallStackControl : UserControl
    {
        private readonly Dictionary<object, string> _componentMuiIdentifiers;
        private IChain _currentChain;
        private readonly ImageList _imageList;

        public CallStackControl()
        {
            InitializeComponent();

            listView1.SmallImageList = _imageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
            };
            _imageList.Images.AddRange(new Image[]
            {
                new Bitmap(1,1),
                Properties.Resources.arrow_yellow,
                Properties.Resources.arrow_blue,
            });

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {columnHeader1, "CallStack.ListHeaders.Function"},
                {columnHeader2, "CallStack.ListHeaders.Offset"},
            };

            DebuggerBase.Instance.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);

            contextMenuStrip1.Renderer = DebuggerBase.Instance.ExtensionHost.ControlManager.MenuRenderer;
        }

        public void SetCurrentChain(IChain chain)
        {
            if (_currentChain != chain)
            {
                listView1.Items.Clear();
                if ((_currentChain = chain) != null)
                {
                    foreach (var frame in chain.GetFrames())
                        listView1.Items.Add(new CallStackListViewItem(frame));
                }
            }
            else
            {
                UpdateCurrentFrames();
            }
        }

        internal void UpdateCurrentFrames()
        {
            if (DebuggerBase.Instance.CurrentFrame != null)
            {
                foreach (CallStackListViewItem item in listView1.Items)
                    item.UpdateItem();
            }
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            DebuggerBase.Instance.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }

        private void listView1_ClientSizeChanged(object sender, EventArgs e)
        {
            columnHeader3.Width = listView1.ClientRectangle.Width - columnHeader2.Width - columnHeader1.Width;
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            var item = listView1.SelectedItems[0] as CallStackListViewItem;
            if (item.Frame.IsUserCode)
            {
                DebuggerBase.Instance.CurrentFrame = item.Frame;
                DebuggerBase.Instance.ExtensionHost.SourceNavigator.NavigateToLocation(item.SourceRange);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Clipboard.SetText(listView1.SelectedItems[0].SubItems.Join(", "));
            }
        }

        private void switchToFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                listView1_ItemActivate(null, null);
        }

        private class CallStackListViewItem : ListViewItem
        {
            public CallStackListViewItem(IFrame frame)
                : base(new string[3])
            {
                Frame = frame;
                UpdateItem();
            }

            public IFrame Frame
            {
                get;
                private set;
            }

            public SourceRange SourceRange
            {
                get;
                private set;
            }

            public void UpdateItem()
            {
                this.ImageIndex = Index == 0 ? 1 : (Frame == DebuggerBase.Instance.CurrentFrame ? 2 : 0);

                if (Frame.IsExternal)
                {
                    SubItems[0].Text = DebuggerBase.Instance.MuiProcessor.GetString("CallStack.Messages.ExternalCode");
                    SubItems[1].Text = "-";
                    SubItems[2].Text = "-";
                    ForeColor = Color.Gray;
                }
                else
                {
                    SubItems[0].Text = string.Format("{0} ({1} {2:X8})", Frame.Function.Name, Path.GetFileName(Frame.Function.Module.Name), Frame.Function.Token.GetToken());
                    SubItems[1].Text = Frame.GetOffset().ToString("X");
                    if (Frame.IsUserCode)
                    {
                        ForeColor = Color.Black;
                        SourceRange = Frame.Function.Symbols.GetSourceRange(Frame.GetOffset());
                        SubItems[2].Text = string.Format("{0}:{1},{2}",
                            SourceRange.FilePath,
                            SourceRange.Line,
                            SourceRange.Column);
                    }
                    else
                    {
                        ForeColor = Color.Gray;
                        SourceRange = null;
                        SubItems[2].Text = DebuggerBase.Instance.MuiProcessor.GetString("CallStack.Messages.NonUserCode");
                    }
                }
            }
        }
    }
}
