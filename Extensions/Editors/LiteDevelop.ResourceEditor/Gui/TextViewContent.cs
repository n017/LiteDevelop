using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteDevelop.ResourceEditor.Gui
{

    public class TextViewContent : TextBox , IResourceViewContent
    {
        public static readonly IResourceViewContentProvider Provider = new TextViewContentProvider();

        private sealed class TextViewContentProvider : IResourceViewContentProvider
        {
            #region IResourceViewContentProvider Members

            public bool SupportsFile(Framework.FileSystem.FilePath file)
            {
                return false;
            }

            public object ReadFile(Framework.FileSystem.FilePath file)
            {
                return null;
            }

            public IResourceViewContent CreateViewContent()
            {
                return new TextViewContent();
            }

            #endregion

        }

        private ResourceEntry _activeEntry;

        public TextViewContent()
        {
            Multiline = true;
            ScrollBars = System.Windows.Forms.ScrollBars.Both;
            WordWrap = false;
            MaxLength = int.MaxValue;
        }

        #region IResourceViewContent Members

        public Control Control
        {
            get { return this; }
        }

        public void AssignResourceEntry(ResourceEntry resourceEntry)
        {
            Text = (_activeEntry = resourceEntry).Value.ToString();
        }

        #endregion

        protected override void OnTextChanged(EventArgs e)
        {
            _activeEntry.Value = Text;
        }
    }
}
