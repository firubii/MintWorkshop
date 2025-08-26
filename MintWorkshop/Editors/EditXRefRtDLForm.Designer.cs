namespace MintWorkshop.Editors
{
    partial class EditXRefRtDLForm
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
            xrefList = new System.Windows.Forms.ListView();
            nameHeader = new System.Windows.Forms.ColumnHeader();
            addButton = new System.Windows.Forms.Button();
            delButton = new System.Windows.Forms.Button();
            saveButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // xrefList
            // 
            xrefList.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            xrefList.AutoArrange = false;
            xrefList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { nameHeader });
            xrefList.FullRowSelect = true;
            xrefList.GridLines = true;
            xrefList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            xrefList.LabelEdit = true;
            xrefList.Location = new System.Drawing.Point(14, 14);
            xrefList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            xrefList.MultiSelect = false;
            xrefList.Name = "xrefList";
            xrefList.ShowGroups = false;
            xrefList.Size = new System.Drawing.Size(513, 419);
            xrefList.TabIndex = 0;
            xrefList.UseCompatibleStateImageBehavior = false;
            xrefList.View = System.Windows.Forms.View.Details;
            xrefList.AfterLabelEdit += xrefList_AfterLabelEdit;
            xrefList.MouseDoubleClick += xrefList_MouseDoubleClick;
            // 
            // nameHeader
            // 
            nameHeader.Text = "Name";
            nameHeader.Width = 363;
            // 
            // addButton
            // 
            addButton.Location = new System.Drawing.Point(14, 441);
            addButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            addButton.Name = "addButton";
            addButton.Size = new System.Drawing.Size(88, 27);
            addButton.TabIndex = 1;
            addButton.Text = "Add";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += addButton_Click;
            // 
            // delButton
            // 
            delButton.Location = new System.Drawing.Point(108, 441);
            delButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            delButton.Name = "delButton";
            delButton.Size = new System.Drawing.Size(88, 27);
            delButton.TabIndex = 2;
            delButton.Text = "Delete";
            delButton.UseVisualStyleBackColor = true;
            delButton.Click += delButton_Click;
            // 
            // saveButton
            // 
            saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            saveButton.Location = new System.Drawing.Point(203, 441);
            saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(324, 27);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // EditXRefRtDLForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(541, 481);
            Controls.Add(saveButton);
            Controls.Add(delButton);
            Controls.Add(addButton);
            Controls.Add(xrefList);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "EditXRefRtDLForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "XRef Editor";
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView xrefList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button delButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ColumnHeader nameHeader;
    }
}