using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using LiteDevelop.Essentials.CodeEditor;
using LiteDevelop.Essentials.FormsDesigner.Gui;
using LiteDevelop.Essentials.FormsDesigner.Services;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.FileSystem.Projects.Net;
using LiteDevelop.Framework.Gui;
using LiteDevelop.Framework.Languages;
using LiteDevelop.Framework.Languages.Net;

namespace LiteDevelop.Essentials.FormsDesigner
{
    public class FormsDesignerExtension : LiteExtension, IFileHandler
    {
        private Dictionary<OpenedFile, FormsDesignerContent> _formEditors;

        public FormsDesignerExtension()
        {
            _formEditors = new Dictionary<OpenedFile, FormsDesignerContent>();
            ToolBoxBuilder = new FormsToolBoxBuilder();
            DesignerSurfaceManager = new DesignSurfaceManager();
        }

        #region LiteExtension Members

        public override string Name
        {
            get { return "Windows Forms Designer"; }
        }

        public override string Description
        {
            get { return "Default graphical user interface designer for windows applications."; }
        }

        public override string Author
        {
            get { return "Jerre S."; }
        }

        public override Version Version
        {
            get { return new Version(0, 9, 0, 0); }
        }

        public override string Copyright
        {
            get { return "Copyright © Jerre S. 2014"; }
        }

        public override void Initialize(InitializationContext context)
        {
            ExtensionHost = context.Host;
        }

        public override void Dispose()
        {
            foreach (var keyValuePair in _formEditors)
                keyValuePair.Value.Close(true);

            DesignerSurfaceManager.Dispose();
            base.Dispose();
        }

        #endregion
        
        #region IFileHandler Members

        public bool CanOpenFile(FilePath filePath)
        {
            if (ExtensionHost.CurrentSolution != null)
            {
                var projectFile = ExtensionHost.CurrentSolution.FindProjectFile(filePath);
                if (projectFile != null)
                {
                    return projectFile.Dependencies.Count != 0;
                }
            }
            return false;
        }

        public void OpenFile(OpenedFile file)
        {
            FormsDesignerContent tab;
            if (!_formEditors.TryGetValue(file, out tab))
            {
                tab = new FormsDesignerContent(this, file);
                tab.Closed += tab_Closed;
                _formEditors.Add(file, tab);
                ExtensionHost.ControlManager.OpenDocumentContents.Add(tab);
            }

            ExtensionHost.ControlManager.SelectedDocumentContent = tab;
           
        }

        #endregion

        public ILiteExtensionHost ExtensionHost
        {
            get;
            private set;
        }
        public DesignSurfaceManager DesignerSurfaceManager
        {
            get;
            private set;
        }

        public FormsToolBoxBuilder ToolBoxBuilder
        {
            get;
            private set;
        }
        
        private void tab_Closed(object sender, FormClosedEventArgs e)
        {
            _formEditors.Remove((sender as LiteDocumentContent).AssociatedFile);
        }

    }
}
