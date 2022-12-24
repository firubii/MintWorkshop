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
            this.label1 = new System.Windows.Forms.Label();
            this.className = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.classFlags = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            this.classImpl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.classExt = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.classFlags)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // className
            // 
            this.className.Location = new System.Drawing.Point(77, 6);
            this.className.Name = "className";
            this.className.Size = new System.Drawing.Size(268, 20);
            this.className.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Flags";
            // 
            // classFlags
            // 
            this.classFlags.Location = new System.Drawing.Point(77, 32);
            this.classFlags.Name = "classFlags";
            this.classFlags.Size = new System.Drawing.Size(268, 20);
            this.classFlags.TabIndex = 3;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(12, 110);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(333, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // classImpl
            // 
            this.classImpl.Location = new System.Drawing.Point(77, 58);
            this.classImpl.Name = "classImpl";
            this.classImpl.Size = new System.Drawing.Size(268, 20);
            this.classImpl.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Implements";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Extends";
            // 
            // classExt
            // 
            this.classExt.Location = new System.Drawing.Point(77, 84);
            this.classExt.Name = "classExt";
            this.classExt.Size = new System.Drawing.Size(268, 20);
            this.classExt.TabIndex = 7;
            // 
            // EditClassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 142);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.classExt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.classImpl);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.classFlags);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.className);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditClassForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Class";
            ((System.ComponentModel.ISupportInitialize)(this.classFlags)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox className;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown classFlags;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox classImpl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox classExt;
    }
}