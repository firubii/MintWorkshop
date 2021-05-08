namespace MintWorkshop
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uppercaseMnemonicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.saveTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.arcTree = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.scriptCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editXRefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addVariableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConstantToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genericCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.namespaceCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNamespaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNamespaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.scriptCtxMenu.SuspendLayout();
            this.classCtxMenu.SuspendLayout();
            this.genericCtxMenu.SuspendLayout();
            this.namespaceCtxMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editorToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1096, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.saveToolStripMenuItem.Text = "Build";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.saveAsToolStripMenuItem.Text = "Build As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // editorToolStripMenuItem
            // 
            this.editorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uppercaseMnemonicsToolStripMenuItem,
            this.toolStripSeparator4,
            this.saveTabToolStripMenuItem,
            this.closeTabToolStripMenuItem,
            this.closeAllTabsToolStripMenuItem});
            this.editorToolStripMenuItem.Name = "editorToolStripMenuItem";
            this.editorToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.editorToolStripMenuItem.Text = "Editor";
            // 
            // uppercaseMnemonicsToolStripMenuItem
            // 
            this.uppercaseMnemonicsToolStripMenuItem.CheckOnClick = true;
            this.uppercaseMnemonicsToolStripMenuItem.Name = "uppercaseMnemonicsToolStripMenuItem";
            this.uppercaseMnemonicsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.uppercaseMnemonicsToolStripMenuItem.Text = "Uppercase Mnemonics";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(220, 6);
            // 
            // saveTabToolStripMenuItem
            // 
            this.saveTabToolStripMenuItem.Name = "saveTabToolStripMenuItem";
            this.saveTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveTabToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.saveTabToolStripMenuItem.Text = "Save Tab";
            this.saveTabToolStripMenuItem.Click += new System.EventHandler(this.saveTabToolStripMenuItem_Click);
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.closeTabToolStripMenuItem.Text = "Close Tab";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.closeTabToolStripMenuItem_Click);
            // 
            // closeAllTabsToolStripMenuItem
            // 
            this.closeAllTabsToolStripMenuItem.Name = "closeAllTabsToolStripMenuItem";
            this.closeAllTabsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.closeAllTabsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.closeAllTabsToolStripMenuItem.Text = "Close All Tabs";
            this.closeAllTabsToolStripMenuItem.Click += new System.EventHandler(this.closeAllTabsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.instructionDictionaryToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // instructionDictionaryToolStripMenuItem
            // 
            this.instructionDictionaryToolStripMenuItem.Name = "instructionDictionaryToolStripMenuItem";
            this.instructionDictionaryToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.instructionDictionaryToolStripMenuItem.Text = "Instruction Dictionary";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProperties);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 16);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(801, 634);
            this.tabControl.TabIndex = 2;
            this.tabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseClick);
            // 
            // tabProperties
            // 
            this.tabProperties.Location = new System.Drawing.Point(4, 22);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabProperties.Size = new System.Drawing.Size(793, 608);
            this.tabProperties.TabIndex = 0;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // arcTree
            // 
            this.arcTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arcTree.HideSelection = false;
            this.arcTree.ImageIndex = 0;
            this.arcTree.ImageList = this.imageList;
            this.arcTree.Location = new System.Drawing.Point(3, 16);
            this.arcTree.Name = "arcTree";
            this.arcTree.PathSeparator = ".";
            this.arcTree.SelectedImageIndex = 0;
            this.arcTree.Size = new System.Drawing.Size(279, 634);
            this.arcTree.TabIndex = 1;
            this.arcTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.arcTree_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Namespace_16x.png");
            this.imageList.Images.SetKeyName(1, "Script_16x.png");
            this.imageList.Images.SetKeyName(2, "Class_16x.png");
            this.imageList.Images.SetKeyName(3, "Field_16x.png");
            this.imageList.Images.SetKeyName(4, "Method_16x.png");
            this.imageList.Images.SetKeyName(5, "Constant_16x.png");
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.arcTree);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 653);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Archive View";
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.tabControl);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(807, 653);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Editor";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(1096, 653);
            this.splitContainer1.SplitterDistance = 285;
            this.splitContainer1.TabIndex = 5;
            // 
            // scriptCtxMenu
            // 
            this.scriptCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addClassToolStripMenuItem,
            this.deleteScriptToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportScriptToolStripMenuItem,
            this.replaceScriptToolStripMenuItem,
            this.toolStripSeparator3,
            this.editXRefsToolStripMenuItem});
            this.scriptCtxMenu.Name = "scriptCtxMenu";
            this.scriptCtxMenu.Size = new System.Drawing.Size(149, 126);
            // 
            // addClassToolStripMenuItem
            // 
            this.addClassToolStripMenuItem.Name = "addClassToolStripMenuItem";
            this.addClassToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.addClassToolStripMenuItem.Text = "Add Class";
            this.addClassToolStripMenuItem.Click += new System.EventHandler(this.addClassToolStripMenuItem_Click);
            // 
            // deleteScriptToolStripMenuItem
            // 
            this.deleteScriptToolStripMenuItem.Name = "deleteScriptToolStripMenuItem";
            this.deleteScriptToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.deleteScriptToolStripMenuItem.Text = "Delete Script";
            this.deleteScriptToolStripMenuItem.Click += new System.EventHandler(this.deleteScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(145, 6);
            // 
            // exportScriptToolStripMenuItem
            // 
            this.exportScriptToolStripMenuItem.Name = "exportScriptToolStripMenuItem";
            this.exportScriptToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exportScriptToolStripMenuItem.Text = "Export Script";
            this.exportScriptToolStripMenuItem.Click += new System.EventHandler(this.exportScriptToolStripMenuItem_Click);
            // 
            // replaceScriptToolStripMenuItem
            // 
            this.replaceScriptToolStripMenuItem.Name = "replaceScriptToolStripMenuItem";
            this.replaceScriptToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.replaceScriptToolStripMenuItem.Text = "Replace Script";
            this.replaceScriptToolStripMenuItem.Click += new System.EventHandler(this.replaceScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(145, 6);
            // 
            // editXRefsToolStripMenuItem
            // 
            this.editXRefsToolStripMenuItem.Name = "editXRefsToolStripMenuItem";
            this.editXRefsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.editXRefsToolStripMenuItem.Text = "Edit XRefs";
            this.editXRefsToolStripMenuItem.Click += new System.EventHandler(this.editXRefsToolStripMenuItem_Click);
            // 
            // classCtxMenu
            // 
            this.classCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editClassToolStripMenuItem,
            this.deleteClassToolStripMenuItem,
            this.toolStripSeparator1,
            this.addVariableToolStripMenuItem,
            this.addFunctionToolStripMenuItem,
            this.addConstantToolStripMenuItem});
            this.classCtxMenu.Name = "classCtxMenu";
            this.classCtxMenu.Size = new System.Drawing.Size(148, 120);
            // 
            // editClassToolStripMenuItem
            // 
            this.editClassToolStripMenuItem.Name = "editClassToolStripMenuItem";
            this.editClassToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.editClassToolStripMenuItem.Text = "Edit Class";
            this.editClassToolStripMenuItem.Click += new System.EventHandler(this.editClassToolStripMenuItem_Click);
            // 
            // deleteClassToolStripMenuItem
            // 
            this.deleteClassToolStripMenuItem.Name = "deleteClassToolStripMenuItem";
            this.deleteClassToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.deleteClassToolStripMenuItem.Text = "Delete Class";
            this.deleteClassToolStripMenuItem.Click += new System.EventHandler(this.deleteClassToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(144, 6);
            // 
            // addVariableToolStripMenuItem
            // 
            this.addVariableToolStripMenuItem.Name = "addVariableToolStripMenuItem";
            this.addVariableToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addVariableToolStripMenuItem.Text = "Add Variable";
            this.addVariableToolStripMenuItem.Click += new System.EventHandler(this.addVariableToolStripMenuItem_Click);
            // 
            // addFunctionToolStripMenuItem
            // 
            this.addFunctionToolStripMenuItem.Name = "addFunctionToolStripMenuItem";
            this.addFunctionToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addFunctionToolStripMenuItem.Text = "Add Function";
            this.addFunctionToolStripMenuItem.Click += new System.EventHandler(this.addFunctionToolStripMenuItem_Click);
            // 
            // addConstantToolStripMenuItem
            // 
            this.addConstantToolStripMenuItem.Name = "addConstantToolStripMenuItem";
            this.addConstantToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addConstantToolStripMenuItem.Text = "Add Constant";
            this.addConstantToolStripMenuItem.Click += new System.EventHandler(this.addConstantToolStripMenuItem_Click);
            // 
            // genericCtxMenu
            // 
            this.genericCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editObjectToolStripMenuItem,
            this.deleteObjectToolStripMenuItem});
            this.genericCtxMenu.Name = "genericCtxMenu";
            this.genericCtxMenu.Size = new System.Drawing.Size(146, 48);
            // 
            // editObjectToolStripMenuItem
            // 
            this.editObjectToolStripMenuItem.Name = "editObjectToolStripMenuItem";
            this.editObjectToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.editObjectToolStripMenuItem.Text = "Edit Object";
            this.editObjectToolStripMenuItem.Click += new System.EventHandler(this.editObjectToolStripMenuItem_Click);
            // 
            // deleteObjectToolStripMenuItem
            // 
            this.deleteObjectToolStripMenuItem.Name = "deleteObjectToolStripMenuItem";
            this.deleteObjectToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.deleteObjectToolStripMenuItem.Text = "Delete Object";
            this.deleteObjectToolStripMenuItem.Click += new System.EventHandler(this.deleteObjectToolStripMenuItem_Click);
            // 
            // namespaceCtxMenu
            // 
            this.namespaceCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addScriptToolStripMenuItem,
            this.addNamespaceToolStripMenuItem,
            this.deleteNamespaceToolStripMenuItem});
            this.namespaceCtxMenu.Name = "namespaceCtxMenu";
            this.namespaceCtxMenu.Size = new System.Drawing.Size(173, 70);
            // 
            // addScriptToolStripMenuItem
            // 
            this.addScriptToolStripMenuItem.Name = "addScriptToolStripMenuItem";
            this.addScriptToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addScriptToolStripMenuItem.Text = "Add Script";
            this.addScriptToolStripMenuItem.Click += new System.EventHandler(this.addScriptToolStripMenuItem_Click);
            // 
            // addNamespaceToolStripMenuItem
            // 
            this.addNamespaceToolStripMenuItem.Name = "addNamespaceToolStripMenuItem";
            this.addNamespaceToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addNamespaceToolStripMenuItem.Text = "Add Namespace";
            this.addNamespaceToolStripMenuItem.Click += new System.EventHandler(this.addNamespaceToolStripMenuItem_Click);
            // 
            // deleteNamespaceToolStripMenuItem
            // 
            this.deleteNamespaceToolStripMenuItem.Name = "deleteNamespaceToolStripMenuItem";
            this.deleteNamespaceToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.deleteNamespaceToolStripMenuItem.Text = "Delete Namespace";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 677);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mint Workshop";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.scriptCtxMenu.ResumeLayout(false);
            this.classCtxMenu.ResumeLayout(false);
            this.genericCtxMenu.ResumeLayout(false);
            this.namespaceCtxMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructionDictionaryToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.TreeView arcTree;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem editorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllTabsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip scriptCtxMenu;
        private System.Windows.Forms.ToolStripMenuItem addClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteScriptToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip classCtxMenu;
        private System.Windows.Forms.ToolStripMenuItem editClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addVariableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFunctionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addConstantToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip genericCtxMenu;
        private System.Windows.Forms.ToolStripMenuItem editObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteObjectToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip namespaceCtxMenu;
        private System.Windows.Forms.ToolStripMenuItem addScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNamespaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteNamespaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem editXRefsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uppercaseMnemonicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

