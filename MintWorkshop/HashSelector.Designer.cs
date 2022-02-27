
namespace MintWorkshop
{
    partial class HashSelector
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
            this.button = new System.Windows.Forms.Button();
            this.hashList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button
            // 
            this.button.Location = new System.Drawing.Point(12, 39);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(376, 23);
            this.button.TabIndex = 0;
            this.button.Text = "Select Hash";
            this.button.UseVisualStyleBackColor = true;
            this.button.Click += new System.EventHandler(this.button_Click);
            // 
            // hashList
            // 
            this.hashList.FormattingEnabled = true;
            this.hashList.Location = new System.Drawing.Point(12, 12);
            this.hashList.Name = "hashList";
            this.hashList.Size = new System.Drawing.Size(376, 21);
            this.hashList.TabIndex = 1;
            // 
            // HashSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 75);
            this.Controls.Add(this.hashList);
            this.Controls.Add(this.button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "HashSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hash Selector";
            this.Shown += new System.EventHandler(this.HashSearch_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button;
        private System.Windows.Forms.ComboBox hashList;
    }
}