using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.Languages;
using LiteDevelop.Extensions;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Templates;

namespace LiteDevelop.Gui.Forms
{
    internal partial class CreateProjectDialog : Form
    {
        public static void UserCreateProject(LiteExtensionHost extensionHost)
        {
           var solution = extensionHost.CurrentSolution;
           
           using (var dlg = new CreateProjectDialog(solution))
           {
               if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
               {
                   var projectDirectory = new FilePath(dlg.Directory, dlg.FileName);
                 
                   if (dlg.CreateSolutionDirectory)
                       projectDirectory = projectDirectory.Combine(projectDirectory.FileName);
           
                   if (System.IO.Directory.Exists(projectDirectory.FullPath))
                   {
                       if (MessageBox.Show(LiteDevelopApplication.Current.MuiProcessor.GetString("CreateProjectDialog.FolderAlreadyExists", "folder=" + projectDirectory), "LiteDevelop", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                           return;
                   }
                   else
                   {
                       System.IO.Directory.CreateDirectory(projectDirectory.FullPath);
                   }
                   
                   bool newSolution = solution == null;
                   if (newSolution)
                   {
                       var solutionDirectory = dlg.CreateSolutionDirectory ? projectDirectory.ParentDirectory : projectDirectory;

                       solution = Solution.CreateSolution(projectDirectory.FileName);
                       solution.FilePath = solutionDirectory.Combine(projectDirectory.FileName + ".sln");
                   }

                   var projects = dlg.Template.Create(new FileCreationContext()
                       {
                           CurrentProject = null,
                           CurrentSolution = solution,
                           FilePath = projectDirectory,
                           FileService = extensionHost.FileService,
                           ProgressReporter = extensionHost.CreateOrGetReporter("Build"),
                       }).Cast<Project>();

                   if (newSolution)
                       extensionHost.DispatchSolutionCreated(new SolutionEventArgs(solution));

                   foreach (var project in projects)
                       solution.Nodes.Add(new ProjectEntry(project));

                   if (newSolution)
                       extensionHost.DispatchSolutionLoad(new SolutionEventArgs(solution));
               }
           }
        }

        private CreateProjectDialog()
        {
            InitializeComponent();
        }

        private IEnumerable<Template> _templates;
        private SolutionFolder _parentFolder;

        public CreateProjectDialog(SolutionFolder parentFolder)
        {
            _parentFolder = parentFolder;
            _templates = LiteDevelopApplication.Current.ExtensionHost.TemplateService.GetProjectTemplates();

            InitializeComponent();
            templatesListView.TemplateService = LiteDevelopApplication.Current.ExtensionHost.TemplateService;
            SetupMuiComponents();

            checkBox1.Checked = checkBox1.Visible = parentFolder == null;
                        
            if (parentFolder == null)
                directoryTextBox.Text = LiteDevelopSettings.Instance.GetValue("Projects.DefaultProjectsPath");
            else
                directoryTextBox.Text = parentFolder.FilePath.HasExtension ? parentFolder.FilePath.ParentDirectory.FullPath : parentFolder.FilePath.FullPath;

            languagesTreeView.Populate(_templates);
        }

        public string FileName
        {
            get { return fileNameTextBox.Text; }
        }

        public string Directory
        {
            get { return directoryTextBox.Text; }
        }

        public bool CreateSolutionDirectory
        {
            get { return _parentFolder == null && checkBox1.Checked; }
        }

        public Template Template
        {
            get { return templatesListView.SelectedTemplate; }
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

        private void UpdateOkButton()
        {
            okButton.Enabled = templatesListView.SelectedItems.Count != 0 && !string.IsNullOrEmpty(fileNameTextBox.Text) && !string.IsNullOrEmpty(directoryTextBox.Text);
        }

        private void SetupMuiComponents()
        {
            var componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {this, "CreateProjectDialog.Title"},
                {nameHeaderLabel, "Common.Name"},
                {directoryHeaderLabel, "Common.Directory"},
                {browseButton, "Common.Browse"},
                {okButton, "Common.Ok"},
                {cancelButton, "Common.Cancel"},
            };

            LiteDevelopApplication.Current.MuiProcessor.ApplyLanguageOnComponents(componentMuiIdentifiers);
        }

        private void languagesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            templatesListView.Populate(from template in _templates
                                       where template.Category == e.Node.Text
                                       select template);
            UpdateOkButton();
        }

        private void control_Changed(object sender, EventArgs e)
        {
            UpdateOkButton();
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void CreateProjectDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && okButton.Enabled)
            {
                okButton.PerformClick();
            }
        }
    }
}
