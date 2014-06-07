using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.ResourceEditor.Gui
{
    public class ResourceEditorContent : LiteDocumentContent
    {
    	private readonly ResourceEditorControl _editorControl;
    	
        public ResourceEditorContent(LiteExtension parent, OpenedFile file)
            : base(parent)
        {
            AssociatedFile = file;
            this.Text = file.FilePath.FileName + file.FilePath.Extension;

            Control = _editorControl = new ResourceEditorControl(file)
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
            };
        }

        public override void Save(Stream stream)
        {
        	_editorControl.WriteToStream(stream);
        }
    }
}
