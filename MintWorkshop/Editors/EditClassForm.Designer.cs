namespace MintWorkshop.Editors
{
    partial class EditClassForm
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
            flagsLabel = new System.Windows.Forms.Label();
            flags = new System.Windows.Forms.NumericUpDown();
            okButton = new System.Windows.Forms.Button();
            implements = new System.Windows.Forms.TextBox();
            implementsLabel = new System.Windows.Forms.Label();
            extendsLabel = new System.Windows.Forms.Label();
            extends = new System.Windows.Forms.TextBox();
            typeLabel = new System.Windows.Forms.Label();
            type = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)flags).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 0;
            label1.Text = "Name";
            // 
            // name
            // 
            name.Location = new System.Drawing.Point(90, 7);
            name.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            name.Name = "name";
            name.Size = new System.Drawing.Size(313, 23);
            name.TabIndex = 1;
            // 
            // flagsLabel
            // 
            flagsLabel.AutoSize = true;
            flagsLabel.Location = new System.Drawing.Point(10, 67);
            flagsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            flagsLabel.Name = "flagsLabel";
            flagsLabel.Size = new System.Drawing.Size(34, 15);
            flagsLabel.TabIndex = 2;
            flagsLabel.Text = "Flags";
            // 
            // flags
            // 
            flags.Location = new System.Drawing.Point(90, 65);
            flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flags.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            flags.Name = "flags";
            flags.Size = new System.Drawing.Size(313, 23);
            flags.TabIndex = 3;
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(14, 155);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(389, 27);
            okButton.TabIndex = 4;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // implements
            // 
            implements.Location = new System.Drawing.Point(90, 95);
            implements.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            implements.Name = "implements";
            implements.Size = new System.Drawing.Size(313, 23);
            implements.TabIndex = 5;
            // 
            // implementsLabel
            // 
            implementsLabel.AutoSize = true;
            implementsLabel.Location = new System.Drawing.Point(10, 98);
            implementsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            implementsLabel.Name = "implementsLabel";
            implementsLabel.Size = new System.Drawing.Size(70, 15);
            implementsLabel.TabIndex = 6;
            implementsLabel.Text = "Implements";
            // 
            // extendsLabel
            // 
            extendsLabel.AutoSize = true;
            extendsLabel.Location = new System.Drawing.Point(10, 128);
            extendsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            extendsLabel.Name = "extendsLabel";
            extendsLabel.Size = new System.Drawing.Size(48, 15);
            extendsLabel.TabIndex = 8;
            extendsLabel.Text = "Extends";
            // 
            // extends
            // 
            extends.Location = new System.Drawing.Point(90, 125);
            extends.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            extends.Name = "extends";
            extends.Size = new System.Drawing.Size(313, 23);
            extends.TabIndex = 7;
            // 
            // typeLabel
            // 
            typeLabel.AutoSize = true;
            typeLabel.Location = new System.Drawing.Point(10, 39);
            typeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            typeLabel.Name = "typeLabel";
            typeLabel.Size = new System.Drawing.Size(31, 15);
            typeLabel.TabIndex = 9;
            typeLabel.Text = "Type";
            // 
            // type
            // 
            type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            type.FormattingEnabled = true;
            type.Items.AddRange(new object[] { "Invalid", "Class", "Enum", "Interface", "Pod", "Rawptr", "Struct", "Unknown_7", "Utility" });
            type.Location = new System.Drawing.Point(90, 36);
            type.Name = "type";
            type.Size = new System.Drawing.Size(313, 23);
            type.TabIndex = 10;
            // 
            // EditClassForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(416, 193);
            Controls.Add(type);
            Controls.Add(typeLabel);
            Controls.Add(extendsLabel);
            Controls.Add(extends);
            Controls.Add(implementsLabel);
            Controls.Add(implements);
            Controls.Add(okButton);
            Controls.Add(flags);
            Controls.Add(flagsLabel);
            Controls.Add(name);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "EditClassForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit Object";
            ((System.ComponentModel.ISupportInitialize)flags).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label flagsLabel;
        private System.Windows.Forms.NumericUpDown flags;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox implements;
        private System.Windows.Forms.Label implementsLabel;
        private System.Windows.Forms.Label extendsLabel;
        private System.Windows.Forms.TextBox extends;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.ComboBox type;
    }
}