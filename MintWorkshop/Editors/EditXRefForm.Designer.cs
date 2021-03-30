namespace MintWorkshop.Editors
{
    partial class EditXRefForm
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
            this.xrefList = new System.Windows.Forms.ListView();
            this.hashHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // xrefList
            // 
            this.xrefList.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.xrefList.AutoArrange = false;
            this.xrefList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hashHeader,
            this.nameHeader});
            this.xrefList.FullRowSelect = true;
            this.xrefList.GridLines = true;
            this.xrefList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.xrefList.HideSelection = false;
            this.xrefList.LabelEdit = true;
            this.xrefList.Location = new System.Drawing.Point(12, 12);
            this.xrefList.MultiSelect = false;
            this.xrefList.Name = "xrefList";
            this.xrefList.ShowGroups = false;
            this.xrefList.Size = new System.Drawing.Size(440, 364);
            this.xrefList.TabIndex = 0;
            this.xrefList.UseCompatibleStateImageBehavior = false;
            this.xrefList.View = System.Windows.Forms.View.Details;
            this.xrefList.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.xrefList_AfterLabelEdit);
            // 
            // hashHeader
            // 
            this.hashHeader.Text = "Hash";
            this.hashHeader.Width = 73;
            // 
            // nameHeader
            // 
            this.nameHeader.Text = "Name";
            this.nameHeader.Width = 363;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(12, 382);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // delButton
            // 
            this.delButton.Location = new System.Drawing.Point(93, 382);
            this.delButton.Name = "delButton";
            this.delButton.Size = new System.Drawing.Size(75, 23);
            this.delButton.TabIndex = 2;
            this.delButton.Text = "Delete";
            this.delButton.UseVisualStyleBackColor = true;
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(174, 382);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(278, 23);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // EditXRefForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 417);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.xrefList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditXRefForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "XRef Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditXRefForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView xrefList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button delButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ColumnHeader hashHeader;
        private System.Windows.Forms.ColumnHeader nameHeader;
    }
}