namespace LiteDevelop.ResourceEditor.Gui
{
    partial class ResourceEditorControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceEditorControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.resourcesListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addStringToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.addFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.removeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.resourcesListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(666, 417);
            this.splitContainer1.SplitterDistance = 392;
            this.splitContainer1.TabIndex = 0;
            // 
            // resourcesListView
            // 
            this.resourcesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.resourcesListView.ContextMenuStrip = this.contextMenuStrip1;
            this.resourcesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourcesListView.FullRowSelect = true;
            this.resourcesListView.GridLines = true;
            this.resourcesListView.LabelEdit = true;
            this.resourcesListView.Location = new System.Drawing.Point(0, 0);
            this.resourcesListView.MultiSelect = false;
            this.resourcesListView.Name = "resourcesListView";
            this.resourcesListView.Size = new System.Drawing.Size(392, 417);
            this.resourcesListView.TabIndex = 0;
            this.resourcesListView.UseCompatibleStateImageBehavior = false;
            this.resourcesListView.View = System.Windows.Forms.View.Details;
            this.resourcesListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.resourcesListView_AfterLabelEdit);
            this.resourcesListView.SelectedIndexChanged += new System.EventHandler(this.resourcesListView_SelectedIndexChanged);
            this.resourcesListView.SizeChanged += new System.EventHandler(this.resourcesListView_SizeChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 132;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 104;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Contents";
            this.columnHeader3.Width = 152;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 417);
            this.label1.TabIndex = 0;
            this.label1.Text = "There are no editors for this type of resource entry installed.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addStringToolStripButton,
            this.addFileToolStripButton,
            this.removeToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(666, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addStringToolStripButton
            // 
            this.addStringToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("addStringToolStripButton.Image")));
            this.addStringToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addStringToolStripButton.Name = "addStringToolStripButton";
            this.addStringToolStripButton.Size = new System.Drawing.Size(82, 22);
            this.addStringToolStripButton.Text = "Add &string";
            this.addStringToolStripButton.Click += new System.EventHandler(this.addStringToolStripButton_Click);
            // 
            // addFileToolStripButton
            // 
            this.addFileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("addFileToolStripButton.Image")));
            this.addFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addFileToolStripButton.Name = "addFileToolStripButton";
            this.addFileToolStripButton.Size = new System.Drawing.Size(68, 22);
            this.addFileToolStripButton.Text = "Add &file";
            this.addFileToolStripButton.Click += new System.EventHandler(this.addFileToolStripButton_Click);
            // 
            // removeToolStripButton
            // 
            this.removeToolStripButton.Image = global::LiteDevelop.ResourceEditor.Properties.Resources.remove;
            this.removeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeToolStripButton.Name = "removeToolStripButton";
            this.removeToolStripButton.Size = new System.Drawing.Size(116, 22);
            this.removeToolStripButton.Text = "&Remove selected";
            this.removeToolStripButton.Click += new System.EventHandler(this.removeToolStripButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 48);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::LiteDevelop.ResourceEditor.Properties.Resources.remove;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripButton_Click);
            // 
            // ResourceEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ResourceEditorControl";
            this.Size = new System.Drawing.Size(666, 442);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView resourcesListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton removeToolStripButton;
        private System.Windows.Forms.ToolStripButton addStringToolStripButton;
        private System.Windows.Forms.ToolStripButton addFileToolStripButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
    }
}
