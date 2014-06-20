using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Resources;
using System.ComponentModel.Design;
using System.Collections.Generic;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.ResourceEditor.Gui
{
    public partial class ResourceEditorControl : UserControl
    {
        private class EditorListViewItem : ListViewItem
        {
            public EditorListViewItem(ResourceEntry entry)
            {
                ResourceEntry = entry;
                UpdateItem();
            }

            public ResourceEntry ResourceEntry
            {
                get;
                private set;
            }

            public void UpdateItem()
            {
                this.SubItems.Clear();
                this.Text = ResourceEntry.Name;
                this.SubItems.AddRange(new string[]
                    {
                        ResourceEntry.Value.GetType().ToString(),
                        ResourceEntry.ToString(),
                    });
            }
        }

        private static readonly string[] _imageExtensions = new string[]
        {
            ".bmp", ".jpg", ".png", ".gif",
        };

        private readonly Framework.FileSystem.OpenedFile _file;
        private readonly ResourceEditorExtension _extension;
        private readonly Dictionary<Type, IResourceViewContent> _cachedEditors = new Dictionary<Type, IResourceViewContent>();
        private readonly Dictionary<object, string> _componentMuiIdentifiers;
        private IResourceViewContent _activeViewContent;

        public ResourceEditorControl(OpenedFile file)
        {
            InitializeComponent();

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {columnHeader1, "ResourceEditorControl.ColumnHeaders.Name"},
                {columnHeader2, "ResourceEditorControl.ColumnHeaders.Type"},
                {columnHeader3, "ResourceEditorControl.ColumnHeaders.Contents"},
                {addStringToolStripButton, "ResourceEditorControl.ToolBar.AddString"},
                {addFileToolStripButton, "ResourceEditorControl.ToolBar.AddFile"},
                {removeToolStripButton, "ResourceEditorControl.ToolBar.RemoveSelected"},
                {renameToolStripMenuItem, "ResourceEditorControl.ContextMenu.Rename"},
                {removeToolStripMenuItem, "ResourceEditorControl.ContextMenu.Remove"},
                {label1, "ResourceEditorControl.Messages.EditorNotInstalled"},
            };

            _extension = ResourceEditorExtension.Instance;
            _extension.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);

            _file = file;
            splitContainer1.Panel2.Controls.Remove(label1);
            ReadResourceFile();
        }

        private void ReadResourceFile()
        {
            if (_file.FilePath.Extension == ".resx")
            {
                ReadResXResourceFile();
            } 
            else if (_file.FilePath.Extension == ".resources")
            {
                ReadNormalResourceFile();
            }
        }

        private void ReadResXResourceFile()
        {
            using (var stream = _file.OpenRead())
            {
                using (var reader = new ResXResourceReader(stream))
                {
                    reader.BasePath = _file.FilePath.ParentDirectory.FullPath;
                    reader.UseResXDataNodes = true;
                    var enumerator = reader.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var node = (ResXDataNode)enumerator.Value;
                        resourcesListView.Items.Add(new EditorListViewItem(CreateResourceEntry(node.Name, node.GetValue(default(ITypeResolutionService)))));
                    }
                }
            }
        }

        private void ReadNormalResourceFile()
        {
            using (var stream = _file.OpenRead())
            {
                using (var reader = new ResourceReader(stream))
                {
                	var enumerator = reader.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        resourcesListView.Items.Add(new EditorListViewItem(CreateResourceEntry(enumerator.Key.ToString(), enumerator.Value)));
                    }
                }
            }
        }

        private ResourceEntry CreateResourceEntry(string name, object value)
        {
            var entry = new ResourceEntry(name, value);
            entry.NameChanged += entry_Changed;
            entry.ValueChanged += entry_Changed;
            return entry;
        }

        private void entry_Changed(object sender, EventArgs e)
        {
            _file.GiveUnsavedData();
            UpdateItem((ResourceEntry)sender);
        }

        private void UpdateItem(ResourceEntry entry)
        {
            foreach (var item in resourcesListView.Items.Cast<EditorListViewItem>())
            {
                if (item.ResourceEntry == entry)
                {
                    item.UpdateItem();
                    break;
                }
            }
        }

        public void WriteToStream(Stream stream)
        {
	        IResourceWriter writer = null;
        
	        if (_file.FilePath.Extension == ".resx")
	        {
	        	writer = new ResXResourceWriter(stream);
	        }
	        else if (_file.FilePath.Extension == ".resources")
	        {
	            writer = new ResourceWriter(stream);
	        }
	        else
	        {
	            throw new NotSupportedException();
	        }
	            
	        foreach (var item in resourcesListView.Items.Cast<EditorListViewItem>())
	        {
	            item.ResourceEntry.AddToWriter(writer);
	        }
	            
	        writer.Generate();
	        
            // no closing/disposing needed. caller will close stream.
        }

        private IResourceViewContent GetEditor(Type type)
        {
            IResourceViewContent content;
            if (!_cachedEditors.TryGetValue(type, out content))
            {
                IResourceViewContentProvider provider;
                if (_extension.RegisteredEditors.TryGetValue(type, out provider))
                    _cachedEditors.Add(type, content = provider.CreateViewContent());
            }
            return content;
        }

        private EditorListViewItem GetCurrentItem()
        {
			if (resourcesListView.SelectedItems.Count > 0)
				return resourcesListView.SelectedItems[0] as EditorListViewItem;
            return null;
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            _extension.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }
        
		private void resourcesListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var currentItem = GetCurrentItem();
			if (currentItem != null)
			{
                var lastActiveContent = _activeViewContent;

                _activeViewContent = GetEditor(currentItem.ResourceEntry.Value.GetType());


                if (lastActiveContent != _activeViewContent)
                    splitContainer1.Panel2.Controls.Clear();

                if (_activeViewContent == null)
                {
                    splitContainer1.Panel2.Controls.Add(label1);
                }
                else
                {
                    _activeViewContent.AssignResourceEntry(currentItem.ResourceEntry);
                    _activeViewContent.Control.Dock = DockStyle.Fill;
                    splitContainer1.Panel2.Controls.Add(_activeViewContent.Control);
                }
			}
		}
        
        private void resourcesListView_SizeChanged(object sender, EventArgs e)
        {
            columnHeader3.Width = resourcesListView.ClientSize.Width - columnHeader1.Width - columnHeader2.Width;
        }

        private void addStringToolStripButton_Click(object sender, EventArgs e)
        {
            var newItem = new EditorListViewItem(CreateResourceEntry("MyStringEntry", string.Empty));
            resourcesListView.Items.Add(newItem);
            newItem.BeginEdit();
            _file.GiveUnsavedData();
        }

        private void resourcesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            var editorItem = (EditorListViewItem)resourcesListView.Items[e.Item];
            if (string.IsNullOrEmpty(e.Label))
                e.CancelEdit = true;
            else
                editorItem.ResourceEntry.Name = e.Label;
        }

        private void addFileToolStripButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = new FilePath(dialog.FileName);
                    object value = null;

                    foreach (var editor in _extension.RegisteredEditors.Values)
                    {
                        if (editor.SupportsFile(filePath))
                        {
                            value = editor.ReadFile(filePath);
                            break;
                        }
                    }

                    if (value == null)
                        value = File.ReadAllBytes(filePath.FullPath);

                    resourcesListView.Items.Add(new EditorListViewItem(CreateResourceEntry(filePath.FileName, value)));
                    _file.GiveUnsavedData();
                }
            }
        }

        private void removeToolStripButton_Click(object sender, EventArgs e)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                if (MessageBox.Show(
                    _extension.MuiProcessor.GetString("ResourceEditorControl.Messages.RemoveEntryConfirmation", "file=" + currentItem.Name), 
                    "LiteDevelop", 
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    currentItem.Remove();
                    _file.GiveUnsavedData();
                }
            }
            
        }
        
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                currentItem.BeginEdit();
            }
        }
        
    }
}
