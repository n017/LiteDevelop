namespace LiteDevelop.Debugger.Net.Gui
{
    partial class GeneralSettingsControl
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
            this.breakOnHandledExceptionCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // breakOnHandledExceptionCheckBox
            // 
            this.breakOnHandledExceptionCheckBox.AutoSize = true;
            this.breakOnHandledExceptionCheckBox.Location = new System.Drawing.Point(13, 14);
            this.breakOnHandledExceptionCheckBox.Name = "breakOnHandledExceptionCheckBox";
            this.breakOnHandledExceptionCheckBox.Size = new System.Drawing.Size(159, 17);
            this.breakOnHandledExceptionCheckBox.TabIndex = 0;
            this.breakOnHandledExceptionCheckBox.Text = "Break on handled exception";
            this.breakOnHandledExceptionCheckBox.UseVisualStyleBackColor = true;
            // 
            // GeneralSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.breakOnHandledExceptionCheckBox);
            this.Name = "GeneralSettingsControl";
            this.Size = new System.Drawing.Size(456, 263);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox breakOnHandledExceptionCheckBox;
    }
}
