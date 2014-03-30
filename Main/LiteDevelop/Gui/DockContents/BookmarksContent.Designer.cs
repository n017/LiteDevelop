namespace LiteDevelop.Gui.DockContents
{
    partial class BookmarksContent
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView1 = new System.Windows.Forms.ListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addBookmarkToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.removeBookmarkToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.previousToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.deleteAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Line";
            this.columnHeader1.Width = 38;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File";
            this.columnHeader3.Width = 155;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 25);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(367, 237);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            this.listView1.SizeChanged += new System.EventHandler(this.listView1_SizeChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBookmarkToolStripButton,
            this.removeBookmarkToolStripButton,
            this.toolStripSeparator1,
            this.previousToolStripButton,
            this.nextToolStripButton,
            this.toolStripSeparator2,
            this.deleteAllToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(367, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // addBookmarkToolStripButton
            // 
            this.addBookmarkToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addBookmarkToolStripButton.Image = global::LiteDevelop.Properties.Resources.bookmark_add;
            this.addBookmarkToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addBookmarkToolStripButton.Name = "addBookmarkToolStripButton";
            this.addBookmarkToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.addBookmarkToolStripButton.Text = "&Add bookmark at current line";
            this.addBookmarkToolStripButton.Click += new System.EventHandler(this.addBookmarkToolStripButton_Click);
            // 
            // removeBookmarkToolStripButton
            // 
            this.removeBookmarkToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeBookmarkToolStripButton.Image = global::LiteDevelop.Properties.Resources.bookmark_remove;
            this.removeBookmarkToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeBookmarkToolStripButton.Name = "removeBookmarkToolStripButton";
            this.removeBookmarkToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.removeBookmarkToolStripButton.Text = "&Remove bookmark from current line";
            this.removeBookmarkToolStripButton.Click += new System.EventHandler(this.removeBookmarkToolStripButton_Click);
            // 
            // previousToolStripButton
            // 
            this.previousToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previousToolStripButton.Image = global::LiteDevelop.Properties.Resources.left;
            this.previousToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previousToolStripButton.Name = "previousToolStripButton";
            this.previousToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.previousToolStripButton.Text = "Go to &previous bookmark";
            this.previousToolStripButton.Click += new System.EventHandler(this.previousToolStripButton_Click);
            // 
            // nextToolStripButton
            // 
            this.nextToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextToolStripButton.Image = global::LiteDevelop.Properties.Resources.right;
            this.nextToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextToolStripButton.Name = "nextToolStripButton";
            this.nextToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextToolStripButton.Text = "Go to &next bookmark";
            this.nextToolStripButton.Click += new System.EventHandler(this.nextToolStripButton_Click);
            // 
            // deleteAllToolStripButton
            // 
            this.deleteAllToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteAllToolStripButton.Image = global::LiteDevelop.Properties.Resources.remove;
            this.deleteAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteAllToolStripButton.Name = "deleteAllToolStripButton";
            this.deleteAllToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.deleteAllToolStripButton.Text = "&Delete all";
            this.deleteAllToolStripButton.Click += new System.EventHandler(this.deleteAllToolStripButton_Click);
            // 
            // BookmarksContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 262);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "BookmarksContent";
            this.Text = "Bookmarks";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addBookmarkToolStripButton;
        private System.Windows.Forms.ToolStripButton removeBookmarkToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton nextToolStripButton;
        private System.Windows.Forms.ToolStripButton previousToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton deleteAllToolStripButton;

    }
}