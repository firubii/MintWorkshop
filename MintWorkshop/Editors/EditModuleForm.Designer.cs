namespace MintWorkshop.Editors
{
    partial class EditModuleForm
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
            label1 = new System.Windows.Forms.Label();
            name = new System.Windows.Forms.TextBox();
            unk = new System.Windows.Forms.TextBox();
            unkLabel = new System.Windows.Forms.Label();
            save = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 0;
            label1.Text = "Name";
            // 
            // name
            // 
            name.Location = new System.Drawing.Point(86, 12);
            name.Name = "name";
            name.Size = new System.Drawing.Size(400, 23);
            name.TabIndex = 1;
            // 
            // unk
            // 
            unk.Location = new System.Drawing.Point(86, 41);
            unk.Name = "unk";
            unk.Size = new System.Drawing.Size(400, 23);
            unk.TabIndex = 3;
            // 
            // unkLabel
            // 
            unkLabel.AutoSize = true;
            unkLabel.Location = new System.Drawing.Point(12, 44);
            unkLabel.Name = "unkLabel";
            unkLabel.Size = new System.Drawing.Size(58, 15);
            unkLabel.TabIndex = 2;
            unkLabel.Text = "Unknown";
            // 
            // save
            // 
            save.Location = new System.Drawing.Point(12, 70);
            save.Name = "save";
            save.Size = new System.Drawing.Size(474, 23);
            save.TabIndex = 4;
            save.Text = "Save";
            save.UseVisualStyleBackColor = true;
            save.Click += save_Click;
            // 
            // EditModuleForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(498, 104);
            Controls.Add(save);
            Controls.Add(unk);
            Controls.Add(unkLabel);
            Controls.Add(name);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "EditModuleForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit Module";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.TextBox unk;
        private System.Windows.Forms.Label unkLabel;
        private System.Windows.Forms.Button save;
    }
}