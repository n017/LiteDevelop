using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger.Gui
{
    public partial class ExceptionDialog : Form
    {
        static Icon _icon;

        static ExceptionDialog()
        {
            var provider = IconProvider.GetProvider<ErrorIconProvider>();
            var index = provider.GetImageIndex(MessageSeverity.Error);
            _icon = Icon.FromHandle(new Bitmap(provider.ImageList.Images[index]).GetHicon());
        }

        private IValue _exceptionValue;
        private SourceLocation _sourceLocation;

        public ExceptionDialog(IValue exceptionValue, SourceLocation sourceLocation)
        {
            InitializeComponent();
            this.Icon = _icon;

            _exceptionValue = exceptionValue;
            _sourceLocation = sourceLocation;

            // no need to hook to UILanguageChanged as this dialog will be closed 
            // before the user gets any chance to update the UI language.
            var _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {this, "ExceptionDialog.Title"},
                {headerLabel, "ExceptionDialog.ExceptionOccured"},
                {fileLabel, "ExceptionDialog.File"},
                {locationLabel, "ExceptionDialog.Location"},
                {messageLabel, "ExceptionDialog.Message"},
                {closeButton, "ExceptionDialog.Close"},
                {goToFileButton, "ExceptionDialog.GoToFile"},
                {detailsButton, "ExceptionDialog.Details"},
            };

            DebuggerBase.Instance.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);

            fileTextBox.Text = sourceLocation.FilePath.FullPath;
            locationTextBox.Text = DebuggerBase.Instance.MuiProcessor.GetString("ExceptionDialog.LocationFormat",
                "line=" + sourceLocation.Line,
                "column=" + sourceLocation.Column);
            messageTextBox.Text = exceptionValue.ValueAsString();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void detailsButton_Click(object sender, EventArgs e)
        {
            // TODO:
        }

        private void goToFileButton_Click(object sender, EventArgs e)
        {
            DebuggerBase.Instance.ExtensionHost.SourceNavigator.NavigateToLocation(_sourceLocation);
            this.Close();
        }

    }
}
