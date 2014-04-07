using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Extensions;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Gui.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace LiteDevelop.Gui.DockContents
{
    public class ViewContentContainer : DockContent
    {
        private LiteExtensionHost _extensionHost;
        private OpenedFile _currentFile;
        private bool _canCloseSafely;

        public ViewContentContainer(LiteViewContent viewContent)
        {
            base.HideOnClose = true;
            _extensionHost = LiteDevelopApplication.Current.ExtensionHost;
            
            ViewContent = viewContent;
            ViewContent.ControlChanged += ViewContent_ControlChanged;
            ViewContent.TextChanged += ViewContent_TextChanged;
            ViewContent.IconChanged += ViewContent_IconChanged;
            ViewContent.Closing += ViewContent_Closing;
            ViewContent.Closed += ViewContent_Closed;

            if (DocumentContent != null)
            {
                _currentFile = DocumentContent.AssociatedFile;
                DocumentContent.AssociatedFileChanged += DocumentContent_AssociatedFileChanged;
                DocumentContent_AssociatedFileChanged(null, null);
            }
            else if (ToolWindow != null)
            {
                ToolWindow.DockStateChanged += ToolWindow_DockStateChanged;
            }

            UpdateText();
            UpdateControl();
            UpdateIcon();
        }

        public LiteViewContent ViewContent
        {
            get;
            private set;
        }

        public LiteDocumentContent DocumentContent
        {
            get { return ViewContent as LiteDocumentContent; }
        }

        public LiteToolWindow ToolWindow
        {
            get { return ViewContent as LiteToolWindow; }
        }

        private bool GetIsChanged()
        {
            return DocumentContent != null && 
                DocumentContent.AssociatedFile != null &&
                DocumentContent.AssociatedFile.HasUnsavedData;
        }

        private void UpdateText()
        {
            Text = ViewContent.Text + (GetIsChanged() ? "*" : "");
        }

        private void UpdateControl()
        {
            Controls.Clear();
            Controls.Add(ViewContent.Control);
        }

        private void UpdateIcon()
        {
            this.Icon = ViewContent.Icon;
        }

        protected override string GetPersistString()
        {
            if (ToolWindow != null)
                return ToolWindow.GetPersistName();
            return ViewContent.GetType().ToString();
        }

        private void ViewContent_TextChanged(object sender, EventArgs e)
        {
            UpdateText();
        }

        private void ViewContent_ControlChanged(object sender, EventArgs e)
        {
            UpdateControl();
        }

        private void ViewContent_IconChanged(object sender, EventArgs e)
        {
            UpdateIcon();
        }

        private void ViewContent_Closing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            var documentContent = DocumentContent;
            if (_extensionHost.ControlManager.NotifyUnsavedFilesWhenClosing && documentContent != null && documentContent.AssociatedFile != null)
            {
                if (documentContent.AssociatedFile.HasUnsavedData)
                {
                    var dialog = new UnsavedFilesDialog(new OpenedFile[1] { documentContent.AssociatedFile });
                    switch (dialog.ShowDialog())
                    {
                        case DialogResult.Yes:
                            dialog.GetItemsToSave()[0].Save(_extensionHost.CreateOrGetReporter("Build"));
                            break;
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
            }
        }

        private void ViewContent_Closed(object sender, FormClosedEventArgs e)
        {
            _canCloseSafely = true;
            Close();
        }

        private void DocumentContent_AssociatedFileChanged(object sender, EventArgs e)
        {
            if (_currentFile != null)
                _currentFile.HasUnsavedDataChanged -= AssociatedFile_HasUnsavedDataChanged;
  
            if ((_currentFile = DocumentContent.AssociatedFile) != null)
                _currentFile.HasUnsavedDataChanged += AssociatedFile_HasUnsavedDataChanged;
        }

        private void AssociatedFile_HasUnsavedDataChanged(object sender, EventArgs e)
        {
            UpdateText();
        }

        private void ToolWindow_DockStateChanged(object sender, EventArgs e)
        {
            this.DockState = ToolWindow.DockState.ToDockPanelSuite();
        }

        protected override void OnDockStateChanged(EventArgs e)
        {
            if (ToolWindow != null)
                ToolWindow.DockState = this.DockState.ToLiteDevelop();

            base.OnDockStateChanged(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel = !_canCloseSafely)
                ViewContent.Close();
            else 
                base.OnClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            ViewContent.ControlChanged -= ViewContent_ControlChanged;
            ViewContent.TextChanged -= ViewContent_TextChanged;
            ViewContent.Closing -= ViewContent_Closing;
            ViewContent.Closed -= ViewContent_Closed;

            if (DocumentContent != null)
            {
                DocumentContent.AssociatedFileChanged -= DocumentContent_AssociatedFileChanged;
            }
            else if (ToolWindow != null)
            {
                ToolWindow.DockStateChanged -= ToolWindow_DockStateChanged;
            }

            base.Dispose(disposing);
        }
    }
}
