using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.Languages;
using LiteDevelop.Extensions;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem.Templates;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Gui.Forms
{
    internal partial class CreateFileDialog : Form
    {
        public static void UserCreateFile(LiteExtensionHost extensionHost)
        {
            UserCreateFile(extensionHost, string.Empty);
        }

        public static void UserCreateFile(LiteExtensionHost extensionHost, string directory)
        {
            UserCreateFile(extensionHost, extensionHost.GetCurrentSelectedProject(), directory);
        }

        public static void UserCreateFile(LiteExtensionHost extensionHost, Project currentProject, string directory)
        {
            using (var dlg = new CreateFileDialog(currentProject))
            {
                if (!string.IsNullOrEmpty(directory))
                {
                    dlg.Directory = directory;
                }
            
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var filePath = new FilePath(dlg.FileName);
                    var files = dlg.Template.Create(new FileCreationContext()
                        {
                            CurrentProject = currentProject,
                            CurrentSolution = extensionHost.CurrentSolution,
                            FilePath = filePath,
                            FileService = extensionHost.FileService,
                            ProgressReporter = extensionHost.CreateOrGetReporter("Build"),
                        });

                    foreach (var createdFile in files)
                    {
                        extensionHost.FileService.SelectFileHandler(
                            extensionHost.ExtensionManager.GetFileHandlers(createdFile.FilePath),
                            createdFile.FilePath).OpenFile((OpenedFile)createdFile);
                    }
                }
            }
        }

        private Project _parentProject;
        private IEnumerable<Template> _templates;
        
        // required for the designer.
        private CreateFileDialog()
        {
            InitializeComponent();
        }

        public CreateFileDialog(Project parentProject)
        {
            _parentProject = parentProject;
            _templates = LiteDevelopApplication.Current.ExtensionHost.TemplateService.GetFileTemplates();

            InitializeComponent();

            templatesListView.TemplateService = LiteDevelopApplication.Current.ExtensionHost.TemplateService;

            if (parentProject != null)
            {
                Text += " - " + parentProject.Name;
            }
            
            SetupMuiComponents();
            
            if (parentProject != null)
                directoryTextBox.Text = parentProject.ProjectDirectory;

            languagesTreeView.Populate(_templates);
        }

        public string FileName
        {
            get { return Path.Combine(directoryTextBox.Text, fileNameTextBox.Text); }
        }

        public string Directory
        {
            get { return directoryTextBox.Text; }
            set { directoryTextBox.Text = value; }
        }

        public Template Template
        {
            get { return templatesListView.SelectedTemplate; }
        }

        private void SetupMuiComponents()
        {
            var componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {this, "CreateFileDialog.Title"},
                {nameHeaderLabel, "Common.Name"},
                {directoryHeaderLabel, "Common.Directory"},
                {browseButton, "Common.Browse"},
                {okButton, "Common.Ok"},
                {cancelButton, "Common.Cancel"},
            };

            LiteDevelopApplication.Current.MuiProcessor.ApplyLanguageOnComponents(componentMuiIdentifiers);
        }

        private void UpdateOkButton()
        {
            okButton.Enabled = templatesListView.SelectedItems.Count != 0
                               && !string.IsNullOrEmpty(fileNameTextBox.Text)
                               && !string.IsNullOrEmpty(directoryTextBox.Text)
                               && !Path.GetInvalidPathChars().Any(directoryTextBox.Text.Contains)
                               && Path.IsPathRooted(directoryTextBox.Text);
        }

        private TreeNode GetLanguageOrderNode(List<TreeNode> nodes, string languageOrder)
        {
            foreach (TreeNode node in nodes)
                if (node.Text == languageOrder)
                    return node;
            var newNode = new TreeNode(languageOrder);
            nodes.Add(newNode);
            return newNode;
        }

        private void languagesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            templatesListView.Populate(from template in _templates
                                       where template.Category == e.Node.Text
                                       select template);
            UpdateOkButton();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!System.IO.Directory.Exists(directoryTextBox.Text))
            {
                MessageBox.Show(
                    LiteDevelopApplication.Current.MuiProcessor.GetString("Common.Messages.DirectoryDoesNotExist", new Dictionary<string, string>
                    {
                        {"directory", directoryTextBox.Text}
                    }),
                    "", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CreateFileDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && okButton.Enabled)
            {
                okButton.PerformClick();
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = directoryTextBox.Text;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    directoryTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void templatesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOkButton();
        }

        private void textBoxes_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButton();
        }


    }
}
