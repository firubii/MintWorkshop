namespace MintWorkshop.Editors
{
    partial class EditVariableForm
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
            label3 = new System.Windows.Forms.Label();
            type = new System.Windows.Forms.TextBox();
            okButton = new System.Windows.Forms.Button();
            flags = new System.Windows.Forms.NumericUpDown();
            flagsLabel = new System.Windows.Forms.Label();
            name = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)flags).BeginInit();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(14, 47);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(31, 15);
            label3.TabIndex = 13;
            label3.Text = "Type";
            // 
            // type
            // 
            type.Location = new System.Drawing.Point(93, 44);
            type.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            type.Name = "type";
            type.Size = new System.Drawing.Size(312, 23);
            type.TabIndex = 12;
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(18, 104);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(388, 27);
            okButton.TabIndex = 11;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // flags
            // 
            flags.Location = new System.Drawing.Point(93, 74);
            flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flags.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            flags.Name = "flags";
            flags.Size = new System.Drawing.Size(313, 23);
            flags.TabIndex = 10;
            // 
            // flagsLabel
            // 
            flagsLabel.AutoSize = true;
            flagsLabel.Location = new System.Drawing.Point(14, 76);
            flagsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            flagsLabel.Name = "flagsLabel";
            flagsLabel.Size = new System.Drawing.Size(34, 15);
            flagsLabel.TabIndex = 9;
            flagsLabel.Text = "Flags";
            // 
            // name
            // 
            name.Location = new System.Drawing.Point(93, 14);
            name.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            name.Name = "name";
            name.Size = new System.Drawing.Size(312, 23);
            name.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 17);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 7;
            label1.Text = "Name";
            // 
            // EditVariableForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(422, 148);
            Controls.Add(label3);
            Controls.Add(type);
            Controls.Add(okButton);
            Controls.Add(flags);
            Controls.Add(flagsLabel);
            Controls.Add(name);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "EditVariableForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit Variable";
            ((System.ComponentModel.ISupportInitialize)flags).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox type;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.NumericUpDown flags;
        private System.Windows.Forms.Label flagsLabel;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label1;
    }
}