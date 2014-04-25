using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Framework.Mui;

namespace LiteDevelop.Extensions
{
    internal sealed class DefaultSourceNavigator : ISourceNavigator
    {
        private LiteExtensionHost _host;
        private MuiProcessor _muiProcessor;

        public DefaultSourceNavigator()
        {
            LiteDevelopApplication.Current.InitializedApplication += Current_InitializedApplication;
        }

        #region ISourceNavigator Members

        public void NavigateToLocation(SourceLocation location)
        {
            var file = _host.FileService.OpenFile(location.FilePath);

            var navigator = file.CurrentDocumentContent as ISourceNavigator ??
                file.RegisteredDocumentContents.FirstOrDefault(x => x is ISourceNavigator) as ISourceNavigator;
            
            if (navigator == null)
            {
                var fileHandlers = _host.ExtensionManager.GetFileHandlers(location.FilePath).Where(x => x is ISourceNavigator).ToArray();

                if (fileHandlers.Length == 0)
                    throw new ArgumentException(_muiProcessor.GetString("MainForm.Messages.NoEditorAvailable", "file=" + location.FilePath.FullPath));

                var handler = _host.FileService.SelectFileHandler(fileHandlers, location.FilePath);
                handler.OpenFile(file);
            }
            else
            {
                _host.ControlManager.ShowAndActivate(navigator as LiteDocumentContent);
            }

            if (navigator != null)
                navigator.NavigateToLocation(location);
        }

        #endregion

        private void Current_InitializedApplication(object sender, EventArgs e)
        {
            _host = LiteDevelopApplication.Current.ExtensionHost;
            _muiProcessor = LiteDevelopApplication.Current.MuiProcessor;
        }
    }
}
