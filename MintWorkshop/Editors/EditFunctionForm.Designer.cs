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
            this.okButton = new System.Windows.Forms.Button();
            this.funcFlags = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.funcName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.funcUnk1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.funcUnk2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.funcFlags)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.funcUnk1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.funcUnk2)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(18, 116);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(333, 23);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // funcFlags
            // 
            this.funcFlags.Location = new System.Drawing.Point(83, 38);
            this.funcFlags.Name = "funcFlags";
            this.funcFlags.Size = new System.Drawing.Size(268, 20);
            this.funcFlags.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Flags";
            // 
            // funcName
            // 
            this.funcName.Location = new System.Drawing.Point(83, 12);
            this.funcName.Name = "funcName";
            this.funcName.Size = new System.Drawing.Size(268, 20);
            this.funcName.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Name";
            // 
            // funcUnk1
            // 
            this.funcUnk1.Location = new System.Drawing.Point(83, 64);
            this.funcUnk1.Name = "funcUnk1";
            this.funcUnk1.Size = new System.Drawing.Size(268, 20);
            this.funcUnk1.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Arguments";
            // 
            // funcUnk2
            // 
            this.funcUnk2.Location = new System.Drawing.Point(83, 90);
            this.funcUnk2.Name = "funcUnk2";
            this.funcUnk2.Size = new System.Drawing.Size(268, 20);
            this.funcUnk2.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Registers";
            // 
            // EditFunctionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 155);
            this.Controls.Add(this.funcUnk2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.funcUnk1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.funcFlags);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.funcName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EditFunctionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Function";
            ((System.ComponentModel.ISupportInitialize)(this.funcFlags)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.funcUnk1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.funcUnk2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.NumericUpDown funcFlags;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox funcName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown funcUnk1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown funcUnk2;
        private System.Windows.Forms.Label label4;
    }
}