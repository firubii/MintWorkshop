
namespace MintWorkshop
{
    partial class ConfigForm
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
            this.uppercase = new System.Windows.Forms.CheckBox();
            this.fontSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.restoreDefault = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // uppercase
            // 
            this.uppercase.AutoSize = true;
            this.uppercase.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uppercase.Location = new System.Drawing.Point(11, 12);
            this.uppercase.Name = "uppercase";
            this.uppercase.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.uppercase.Size = new System.Drawing.Size(135, 17);
            this.uppercase.TabIndex = 0;
            this.uppercase.Text = "Uppercase Mnemonics";
            this.uppercase.UseVisualStyleBackColor = true;
            // 
            // fontSize
            // 
            this.fontSize.Location = new System.Drawing.Point(132, 35);
            this.fontSize.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.fontSize.Name = "fontSize";
            this.fontSize.Size = new System.Drawing.Size(120, 20);
            this.fontSize.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Font Size";
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(15, 90);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(237, 23);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // restoreDefault
            // 
            this.restoreDefault.Location = new System.Drawing.Point(15, 61);
            this.restoreDefault.Name = "restoreDefault";
            this.restoreDefault.Size = new System.Drawing.Size(237, 23);
            this.restoreDefault.TabIndex = 4;
            this.restoreDefault.Text = "Restore Defaults";
            this.restoreDefault.UseVisualStyleBackColor = true;
            this.restoreDefault.Click += new System.EventHandler(this.restoreDefault_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 123);
            this.Controls.Add(this.restoreDefault);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fontSize);
            this.Controls.Add(this.uppercase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mint Workshop Settings";
            ((System.ComponentModel.ISupportInitialize)(this.fontSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox uppercase;
        private System.Windows.Forms.NumericUpDown fontSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button restoreDefault;
    }
}