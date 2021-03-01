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
            this.label3 = new System.Windows.Forms.Label();
            this.varType = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.varFlags = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.varName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.varFlags)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Type";
            // 
            // varType
            // 
            this.varType.Location = new System.Drawing.Point(80, 38);
            this.varType.Name = "varType";
            this.varType.Size = new System.Drawing.Size(268, 20);
            this.varType.TabIndex = 12;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(15, 90);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(333, 23);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // varFlags
            // 
            this.varFlags.Location = new System.Drawing.Point(80, 64);
            this.varFlags.Name = "varFlags";
            this.varFlags.Size = new System.Drawing.Size(268, 20);
            this.varFlags.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Flags";
            // 
            // varName
            // 
            this.varName.Location = new System.Drawing.Point(80, 12);
            this.varName.Name = "varName";
            this.varName.Size = new System.Drawing.Size(268, 20);
            this.varName.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Name";
            // 
            // EditVariableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 128);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.varType);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.varFlags);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.varName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditVariableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Variable";
            ((System.ComponentModel.ISupportInitialize)(this.varFlags)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox varType;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.NumericUpDown varFlags;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox varName;
        private System.Windows.Forms.Label label1;
    }
}