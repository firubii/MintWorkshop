namespace MintWorkshop.Editors
{
    partial class EditEnumForm
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
            okButton = new System.Windows.Forms.Button();
            value = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            name = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            flags = new System.Windows.Forms.NumericUpDown();
            flagsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)value).BeginInit();
            ((System.ComponentModel.ISupportInitialize)flags).BeginInit();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(13, 102);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(396, 27);
            okButton.TabIndex = 23;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // value
            // 
            value.Hexadecimal = true;
            value.Location = new System.Drawing.Point(94, 44);
            value.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            value.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            value.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            value.Name = "value";
            value.Size = new System.Drawing.Size(313, 23);
            value.TabIndex = 22;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(15, 46);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(35, 15);
            label2.TabIndex = 21;
            label2.Text = "Value";
            // 
            // name
            // 
            name.Location = new System.Drawing.Point(94, 14);
            name.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            name.Name = "name";
            name.Size = new System.Drawing.Size(312, 23);
            name.TabIndex = 20;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 17);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 19;
            label1.Text = "Name";
            // 
            // flags
            // 
            flags.Hexadecimal = true;
            flags.Location = new System.Drawing.Point(93, 73);
            flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flags.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            flags.Name = "flags";
            flags.Size = new System.Drawing.Size(313, 23);
            flags.TabIndex = 25;
            // 
            // flagsLabel
            // 
            flagsLabel.AutoSize = true;
            flagsLabel.Location = new System.Drawing.Point(14, 75);
            flagsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            flagsLabel.Name = "flagsLabel";
            flagsLabel.Size = new System.Drawing.Size(34, 15);
            flagsLabel.TabIndex = 24;
            flagsLabel.Text = "Flags";
            // 
            // EditConstantForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(422, 139);
            Controls.Add(flags);
            Controls.Add(flagsLabel);
            Controls.Add(okButton);
            Controls.Add(value);
            Controls.Add(label2);
            Controls.Add(name);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "EditConstantForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit Constant";
            ((System.ComponentModel.ISupportInitialize)value).EndInit();
            ((System.ComponentModel.ISupportInitialize)flags).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.NumericUpDown value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown flags;
        private System.Windows.Forms.Label flagsLabel;
    }
}