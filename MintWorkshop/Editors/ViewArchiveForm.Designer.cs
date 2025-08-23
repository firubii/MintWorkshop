namespace MintWorkshop.Editors
{
    partial class ViewArchiveForm
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
            splitContainer = new System.Windows.Forms.SplitContainer();
            groupBox1 = new System.Windows.Forms.GroupBox();
            endianness = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            xdataVerMin = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            xdataVerMaj = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            groupBox2 = new System.Windows.Forms.GroupBox();
            isCompressed = new System.Windows.Forms.CheckBox();
            mintVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)xdataVerMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xdataVerMaj).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.IsSplitterFixed = true;
            splitContainer.Location = new System.Drawing.Point(0, 0);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(groupBox2);
            splitContainer.Size = new System.Drawing.Size(419, 167);
            splitContainer.SplitterDistance = 89;
            splitContainer.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(endianness);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(xdataVerMin);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(xdataVerMaj);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(419, 89);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "XData Properties";
            // 
            // endianness
            // 
            endianness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            endianness.FormattingEnabled = true;
            endianness.Items.AddRange(new object[] { "Big", "Little" });
            endianness.Location = new System.Drawing.Point(85, 51);
            endianness.Name = "endianness";
            endianness.Size = new System.Drawing.Size(121, 23);
            endianness.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 54);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(69, 15);
            label3.TabIndex = 4;
            label3.Text = "Endianness:";
            // 
            // xdataVerMin
            // 
            xdataVerMin.Location = new System.Drawing.Point(141, 22);
            xdataVerMin.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            xdataVerMin.Name = "xdataVerMin";
            xdataVerMin.Size = new System.Drawing.Size(37, 23);
            xdataVerMin.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(125, 28);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(10, 15);
            label2.TabIndex = 2;
            label2.Text = ".";
            // 
            // xdataVerMaj
            // 
            xdataVerMaj.Location = new System.Drawing.Point(85, 22);
            xdataVerMaj.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            xdataVerMaj.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            xdataVerMaj.Name = "xdataVerMaj";
            xdataVerMaj.Size = new System.Drawing.Size(39, 23);
            xdataVerMaj.TabIndex = 1;
            xdataVerMaj.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(11, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 15);
            label1.TabIndex = 0;
            label1.Text = "Version:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(isCompressed);
            groupBox2.Controls.Add(mintVersion);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(0, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(419, 74);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "Archive Properties";
            // 
            // isCompressed
            // 
            isCompressed.AutoSize = true;
            isCompressed.Location = new System.Drawing.Point(12, 46);
            isCompressed.Name = "isCompressed";
            isCompressed.Size = new System.Drawing.Size(120, 19);
            isCompressed.TabIndex = 1;
            isCompressed.Text = "LZ77 Compressed";
            isCompressed.UseVisualStyleBackColor = true;
            // 
            // mintVersion
            // 
            mintVersion.AutoSize = true;
            mintVersion.Location = new System.Drawing.Point(10, 25);
            mintVersion.Name = "mintVersion";
            mintVersion.Size = new System.Drawing.Size(48, 15);
            mintVersion.TabIndex = 0;
            mintVersion.Text = "Version:";
            // 
            // ViewArchiveForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(419, 167);
            Controls.Add(splitContainer);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ViewArchiveForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "ViewArchiveForm";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)xdataVerMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)xdataVerMaj).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown xdataVerMaj;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown xdataVerMin;
        private System.Windows.Forms.ComboBox endianness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox isCompressed;
        private System.Windows.Forms.Label mintVersion;
    }
}