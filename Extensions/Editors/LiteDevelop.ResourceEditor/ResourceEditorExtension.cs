using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.Mui;
using LiteDevelop.ResourceEditor.Gui;

namespace LiteDevelop.ResourceEditor
{
    public class ResourceEditorExtension : LiteExtension, IFileHandler
    {
        public static ResourceEditorExtension Instance
        {
            get;
            private set;
        }

        public ResourceEditorExtension()
        {
            if (Instance != null)
                throw new InvalidOperationException("Cannot create a second instance of ResourceEditorExtension.");

            Instance = this;
        }

        public override string Name
        {
            get { return "Resource Editor"; }
        }

        public override string Description
        {
            get { return "Default resource files editor."; }
        }

        public override string Author
        {
            get { return "Jerre S."; }
        }

        public override Version Version
        {
            get { return typeof(ResourceEditorExtension).Assembly.GetName().Version; }
        }

        public override string Copyright
        {
            get { return "Copyright © Jerre S. 2014"; }
        }

        public override void Initialize(InitializationContext context)
        {
            ExtensionHost = context.Host;
            RegisteredEditors = new Dictionary<Type, IResourceViewContentProvider>()
            {
                {typeof(Bitmap), ImageViewContent.Provider},
                {typeof(string), TextViewContent.Provider}
            };

            MuiProcessor = new MuiProcessor(ExtensionHost, Path.Combine(Path.GetDirectoryName(typeof(ResourceEditorExtension).Assembly.Location), "Mui"));

        }

        public ILiteExtensionHost ExtensionHost
        {
            get;
            private set;
        }

        public MuiProcessor MuiProcessor
        {
            get;
            private set;
        }

        public Dictionary<Type, IResourceViewContentProvider> RegisteredEditors
        {
            get;
            private set;
        }

        public override void Dispose()
        {
            base.Dispose();
            Instance = null;
        }

        #region IFileHandler Members

        public bool CanOpenFile(Framework.FileSystem.FilePath filePath)
        {
            return new string[] { ".resx", ".resources" }.Contains(filePath.Extension);
        }

        public void OpenFile(Framework.FileSystem.OpenedFile file)
        {
            ExtensionHost.ControlManager.OpenDocumentContents.Add(new ResourceEditorContent(this, file));
        }

        #endregion

    }
}
