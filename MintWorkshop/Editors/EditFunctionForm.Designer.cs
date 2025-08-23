namespace MintWorkshop.Editors
{
    partial class EditFunctionForm
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
            flags = new System.Windows.Forms.NumericUpDown();
            flagsLabel = new System.Windows.Forms.Label();
            name = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            arguments = new System.Windows.Forms.NumericUpDown();
            argumentsLabel = new System.Windows.Forms.Label();
            registers = new System.Windows.Forms.NumericUpDown();
            registersLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)flags).BeginInit();
            ((System.ComponentModel.ISupportInitialize)arguments).BeginInit();
            ((System.ComponentModel.ISupportInitialize)registers).BeginInit();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(21, 134);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(388, 27);
            okButton.TabIndex = 18;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // flags
            // 
            flags.Location = new System.Drawing.Point(97, 44);
            flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flags.Name = "flags";
            flags.Size = new System.Drawing.Size(313, 23);
            flags.TabIndex = 17;
            // 
            // flagsLabel
            // 
            flagsLabel.AutoSize = true;
            flagsLabel.Location = new System.Drawing.Point(18, 46);
            flagsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            flagsLabel.Name = "flagsLabel";
            flagsLabel.Size = new System.Drawing.Size(34, 15);
            flagsLabel.TabIndex = 16;
            flagsLabel.Text = "Flags";
            // 
            // name
            // 
            name.Location = new System.Drawing.Point(97, 14);
            name.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            name.Name = "name";
            name.Size = new System.Drawing.Size(312, 23);
            name.TabIndex = 15;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(18, 17);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 14;
            label1.Text = "Name";
            // 
            // arguments
            // 
            arguments.Location = new System.Drawing.Point(97, 74);
            arguments.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            arguments.Name = "arguments";
            arguments.Size = new System.Drawing.Size(313, 23);
            arguments.TabIndex = 20;
            // 
            // argumentsLabel
            // 
            argumentsLabel.AutoSize = true;
            argumentsLabel.Location = new System.Drawing.Point(18, 76);
            argumentsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            argumentsLabel.Name = "argumentsLabel";
            argumentsLabel.Size = new System.Drawing.Size(66, 15);
            argumentsLabel.TabIndex = 19;
            argumentsLabel.Text = "Arguments";
            // 
            // registers
            // 
            registers.Location = new System.Drawing.Point(97, 104);
            registers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            registers.Name = "registers";
            registers.Size = new System.Drawing.Size(313, 23);
            registers.TabIndex = 22;
            // 
            // registersLabel
            // 
            registersLabel.AutoSize = true;
            registersLabel.Location = new System.Drawing.Point(18, 106);
            registersLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            registersLabel.Name = "registersLabel";
            registersLabel.Size = new System.Drawing.Size(54, 15);
            registersLabel.TabIndex = 21;
            registersLabel.Text = "Registers";
            // 
            // EditFunctionForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(426, 179);
            Controls.Add(registers);
            Controls.Add(registersLabel);
            Controls.Add(arguments);
            Controls.Add(argumentsLabel);
            Controls.Add(okButton);
            Controls.Add(flags);
            Controls.Add(flagsLabel);
            Controls.Add(name);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "EditFunctionForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit Function";
            ((System.ComponentModel.ISupportInitialize)flags).EndInit();
            ((System.ComponentModel.ISupportInitialize)arguments).EndInit();
            ((System.ComponentModel.ISupportInitialize)registers).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.NumericUpDown flags;
        private System.Windows.Forms.Label flagsLabel;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown arguments;
        private System.Windows.Forms.Label argumentsLabel;
        private System.Windows.Forms.NumericUpDown registers;
        private System.Windows.Forms.Label registersLabel;
    }
}