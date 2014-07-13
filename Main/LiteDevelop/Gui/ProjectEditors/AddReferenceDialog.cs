using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Gui.ProjectEditors
{
    internal partial class AddReferenceDialog : Form
    {
        private class AssemblyListViewItem : ListViewItem
        {
            private readonly FilePath _filePath;
            private static readonly FilePath _microsoftNetFolder = new FilePath(typeof(object).Assembly.Location).ParentDirectory.ParentDirectory;

            public AssemblyListViewItem(FileVersionInfo info)
            {
                AssemblyInfo = info;
                _filePath = new FilePath(info.FileName);

                this.Text = _filePath.FileName;
                SubItems.AddRange(new string[]
                {
                    info.FileVersion,
                    _filePath.FullPath,
                });
            }

            public AssemblyReference GetAssemblyReference(Project container)
            {
                var reference = new AssemblyReference(_filePath.FileName);
                if (!_filePath.ParentDirectory.FullPath.StartsWith(_microsoftNetFolder.FullPath))
                {
                    reference.HintPath = _filePath.GetRelativePath(container);
                }

                return reference;
            }

            public FileVersionInfo AssemblyInfo
            {
                get;
                private set;
            }
        }

        private static List<FileVersionInfo> _assemblyCache = new List<FileVersionInfo>();

        static AddReferenceDialog()
        {
            foreach (var searchPath in LiteDevelopSettings.Instance.GetArray("AddReferenceDialog.SearchPaths"))
            {
                if (Directory.Exists(searchPath))
                {
                    foreach (var file in Directory.GetFiles(searchPath, "*.dll"))
                    {                        
                        _assemblyCache.Add(FileVersionInfo.GetVersionInfo(file));
                    }
                }
            }
        }

        public AddReferenceDialog()
        {
            InitializeComponent();

            foreach (var assembly in _assemblyCache)
                listView1.Items.Add(new AssemblyListViewItem(assembly));
        }

        public IEnumerable<AssemblyReference> GetSelectedAssemblies(Project container)
        {
            foreach (AssemblyListViewItem item in listView1.CheckedItems)
                if (item.Checked)
                    yield return item.GetAssemblyReference(container);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "Assemblies|*.exe;*.dll"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var item = new AssemblyListViewItem(FileVersionInfo.GetVersionInfo(ofd.FileName));
                listView1.Items.Add(item);
                item.Checked = true;
                item.Selected = true;
                item.EnsureVisible();
            }
        }
    }
}
