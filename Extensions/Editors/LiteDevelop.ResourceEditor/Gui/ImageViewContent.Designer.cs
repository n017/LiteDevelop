namespace LiteDevelop.ResourceEditor.Gui
{
    partial class ImageViewContent
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.updateImageButton = new System.Windows.Forms.Button();
            this.imageWrapperPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.imageWrapperPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(115, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // updateImageButton
            // 
            this.updateImageButton.Location = new System.Drawing.Point(3, 3);
            this.updateImageButton.Name = "updateImageButton";
            this.updateImageButton.Size = new System.Drawing.Size(142, 23);
            this.updateImageButton.TabIndex = 1;
            this.updateImageButton.Text = "Update image";
            this.updateImageButton.UseVisualStyleBackColor = true;
            this.updateImageButton.Click += new System.EventHandler(this.updateImageButton_Click);
            // 
            // imageWrapperPanel
            // 
            this.imageWrapperPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageWrapperPanel.AutoScroll = true;
            this.imageWrapperPanel.Controls.Add(this.pictureBox1);
            this.imageWrapperPanel.Location = new System.Drawing.Point(3, 32);
            this.imageWrapperPanel.Name = "imageWrapperPanel";
            this.imageWrapperPanel.Size = new System.Drawing.Size(328, 242);
            this.imageWrapperPanel.TabIndex = 2;
            // 
            // ImageViewContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.imageWrapperPanel);
            this.Controls.Add(this.updateImageButton);
            this.Name = "ImageViewContent";
            this.Size = new System.Drawing.Size(334, 277);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.imageWrapperPanel.ResumeLayout(false);
            this.imageWrapperPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button updateImageButton;
        private System.Windows.Forms.Panel imageWrapperPanel;
    }
}
