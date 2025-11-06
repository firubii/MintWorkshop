namespace MintWorkshop.Editors
{
    partial class NewArchiveForm
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
            name = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            version = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            isLittleEndian = new System.Windows.Forms.CheckBox();
            save = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // name
            // 
            name.Location = new System.Drawing.Point(91, 12);
            name.Name = "name";
            name.Size = new System.Drawing.Size(247, 23);
            name.TabIndex = 0;
            name.Text = "New Archive";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 1;
            label1.Text = "Name";
            // 
            // version
            // 
            version.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            version.FormattingEnabled = true;
            version.Location = new System.Drawing.Point(91, 41);
            version.Name = "version";
            version.Size = new System.Drawing.Size(247, 23);
            version.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 44);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(45, 15);
            label2.TabIndex = 3;
            label2.Text = "Version";
            // 
            // isLittleEndian
            // 
            isLittleEndian.AutoSize = true;
            isLittleEndian.Checked = true;
            isLittleEndian.CheckState = System.Windows.Forms.CheckState.Checked;
            isLittleEndian.Location = new System.Drawing.Point(91, 70);
            isLittleEndian.Name = "isLittleEndian";
            isLittleEndian.Size = new System.Drawing.Size(91, 19);
            isLittleEndian.TabIndex = 4;
            isLittleEndian.Text = "Little Endian";
            isLittleEndian.UseVisualStyleBackColor = true;
            // 
            // save
            // 
            save.Location = new System.Drawing.Point(127, 100);
            save.Name = "save";
            save.Size = new System.Drawing.Size(92, 23);
            save.TabIndex = 5;
            save.Text = "Create Archive";
            save.UseVisualStyleBackColor = true;
            save.Click += save_Click;
            // 
            // NewArchiveForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(350, 135);
            Controls.Add(save);
            Controls.Add(isLittleEndian);
            Controls.Add(label2);
            Controls.Add(version);
            Controls.Add(label1);
            Controls.Add(name);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewArchiveForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "New Archive";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox version;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox isLittleEndian;
        private System.Windows.Forms.Button save;
    }
}