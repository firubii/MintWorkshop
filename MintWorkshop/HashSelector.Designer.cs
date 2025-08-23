
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
            button = new System.Windows.Forms.Button();
            hashList = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // button
            // 
            button.Location = new System.Drawing.Point(14, 45);
            button.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button.Name = "button";
            button.Size = new System.Drawing.Size(439, 27);
            button.TabIndex = 0;
            button.Text = "Select Hash";
            button.UseVisualStyleBackColor = true;
            button.Click += button_Click;
            // 
            // hashList
            // 
            hashList.FormattingEnabled = true;
            hashList.Location = new System.Drawing.Point(14, 14);
            hashList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            hashList.Name = "hashList";
            hashList.Size = new System.Drawing.Size(438, 23);
            hashList.TabIndex = 1;
            // 
            // HashSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(467, 87);
            Controls.Add(hashList);
            Controls.Add(button);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HashSelector";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Hash Selector";
            FormClosing += HashSelector_FormClosing;
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button;
        private System.Windows.Forms.ComboBox hashList;
    }
}