using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDevelop.Framework.Mui;

namespace LiteDevelop.ResourceEditor.Gui
{
    public partial class ImageViewContent : UserControl, IResourceViewContent
    {
        public static readonly IResourceViewContentProvider Provider = new ImageViewContentProvider();

        private sealed class ImageViewContentProvider : IResourceViewContentProvider
        {
            private static readonly string[] _supportedExtensions = new string[]
            {
                ".bmp", ".jpg", ".png", ".gif"
            };

            #region IResourceViewContentProvider Members

            public bool SupportsFile(Framework.FileSystem.FilePath file)
            {
                return _supportedExtensions.Contains(file.Extension, StringComparer.OrdinalIgnoreCase);
            }

            public object ReadFile(Framework.FileSystem.FilePath file)
            {
                return Bitmap.FromFile(file.FullPath);
            }

            public IResourceViewContent CreateViewContent()
            {
                return new ImageViewContent();
            }

            #endregion
        }

        private ResourceEntry _resourceEntry;
        private readonly ResourceEditorExtension _extension;
        private readonly Dictionary<object, string> _componentMuiIdentifiers;

        public ImageViewContent()
        {
            InitializeComponent();

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {updateImageButton, "ImageViewContent.UpdateImage"}
            };

            _extension = ResourceEditorExtension.Instance;
            _extension.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);
        }
        
        #region IResourceViewContent Members

        public Control Control
        {
            get { return this; }
        }

        public void AssignResourceEntry(ResourceEntry resourceEntry)
        {
            _resourceEntry = resourceEntry;
            pictureBox1.Image = (Image)resourceEntry.Value;
        }

        #endregion

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            _extension.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }

        private void updateImageButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Image files (*.png;*.bmp;*.jpg;*.gif)|*.png;*.bmp;*.jpg;*.gif";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        _resourceEntry.Value = pictureBox1.Image = Image.FromFile(dialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

    }
}
