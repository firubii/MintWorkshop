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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openRtDLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            reloadHashesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            saveTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeAllTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            parseAsFloatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            convertToDecimalToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            searchForHashUsageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            instructionDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tabControl = new System.Windows.Forms.TabControl();
            arcTree = new System.Windows.Forms.TreeView();
            imageList = new System.Windows.Forms.ImageList(components);
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            scriptCtxMenu = new System.Windows.Forms.ContextMenuStrip(components);
            editModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            exportScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            replaceScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            editXRefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            optimizeScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            classCtxMenu = new System.Windows.Forms.ContextMenuStrip(components);
            findUsesOfObjectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            editClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            addVariableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addConstantToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            genericCtxMenu = new System.Windows.Forms.ContextMenuStrip(components);
            findUsesOfObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyFullNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            editObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editorCtxMenu = new System.Windows.Forms.ContextMenuStrip(components);
            parseAsFloatToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            convertToDecimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            archiveMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            buildMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            buildAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            addModuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            importModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            viewPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            dumpHashesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            closeArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            namespaceMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            exportAllModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            scriptCtxMenu.SuspendLayout();
            classCtxMenu.SuspendLayout();
            genericCtxMenu.SuspendLayout();
            editorCtxMenu.SuspendLayout();
            archiveMenuStrip.SuspendLayout();
            namespaceMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editorToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(1279, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openToolStripMenuItem, openRtDLToolStripMenuItem, closeToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            openToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // openRtDLToolStripMenuItem
            // 
            openRtDLToolStripMenuItem.Name = "openRtDLToolStripMenuItem";
            openRtDLToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.O;
            openRtDLToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            openRtDLToolStripMenuItem.Text = "Open RtDL";
            openRtDLToolStripMenuItem.Click += openRtDLToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            closeToolStripMenuItem.Text = "Close All Archives";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // editorToolStripMenuItem
            // 
            editorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { settingsToolStripMenuItem, reloadHashesToolStripMenuItem, toolStripSeparator4, saveTabToolStripMenuItem, closeTabToolStripMenuItem, closeAllTabsToolStripMenuItem });
            editorToolStripMenuItem.Name = "editorToolStripMenuItem";
            editorToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            editorToolStripMenuItem.Text = "Editor";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // reloadHashesToolStripMenuItem
            // 
            reloadHashesToolStripMenuItem.Name = "reloadHashesToolStripMenuItem";
            reloadHashesToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            reloadHashesToolStripMenuItem.Text = "Reload Hashes";
            reloadHashesToolStripMenuItem.Click += reloadHashesToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(220, 6);
            // 
            // saveTabToolStripMenuItem
            // 
            saveTabToolStripMenuItem.Name = "saveTabToolStripMenuItem";
            saveTabToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            saveTabToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            saveTabToolStripMenuItem.Text = "Save Tab";
            saveTabToolStripMenuItem.Click += saveTabToolStripMenuItem_Click;
            // 
            // closeTabToolStripMenuItem
            // 
            closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            closeTabToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W;
            closeTabToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            closeTabToolStripMenuItem.Text = "Close Tab";
            closeTabToolStripMenuItem.Click += closeTabToolStripMenuItem_Click;
            // 
            // closeAllTabsToolStripMenuItem
            // 
            closeAllTabsToolStripMenuItem.Name = "closeAllTabsToolStripMenuItem";
            closeAllTabsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.W;
            closeAllTabsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            closeAllTabsToolStripMenuItem.Text = "Close All Tabs";
            closeAllTabsToolStripMenuItem.Click += closeAllTabsToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { parseAsFloatToolStripMenuItem, convertToDecimalToolStripMenuItem1, searchForHashUsageToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // parseAsFloatToolStripMenuItem
            // 
            parseAsFloatToolStripMenuItem.Name = "parseAsFloatToolStripMenuItem";
            parseAsFloatToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            parseAsFloatToolStripMenuItem.Text = "Convert to Float";
            parseAsFloatToolStripMenuItem.Click += parseAsFloatToolStripMenuItem_Click;
            // 
            // convertToDecimalToolStripMenuItem1
            // 
            convertToDecimalToolStripMenuItem1.Name = "convertToDecimalToolStripMenuItem1";
            convertToDecimalToolStripMenuItem1.Size = new System.Drawing.Size(191, 22);
            convertToDecimalToolStripMenuItem1.Text = "Convert to Decimal";
            convertToDecimalToolStripMenuItem1.Click += convertToDecimalToolStripMenuItem_Click;
            // 
            // searchForHashUsageToolStripMenuItem
            // 
            searchForHashUsageToolStripMenuItem.Name = "searchForHashUsageToolStripMenuItem";
            searchForHashUsageToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            searchForHashUsageToolStripMenuItem.Text = "Search for Hash usage";
            searchForHashUsageToolStripMenuItem.Click += searchForHashUsageToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { instructionDictionaryToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // instructionDictionaryToolStripMenuItem
            // 
            instructionDictionaryToolStripMenuItem.Name = "instructionDictionaryToolStripMenuItem";
            instructionDictionaryToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            instructionDictionaryToolStripMenuItem.Text = "Instruction Dictionary";
            instructionDictionaryToolStripMenuItem.Click += instructionDictionaryToolStripMenuItem_Click;
            // 
            // tabControl
            // 
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl.Location = new System.Drawing.Point(4, 19);
            tabControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(934, 735);
            tabControl.TabIndex = 2;
            tabControl.MouseClick += tabControl_MouseClick;
            // 
            // arcTree
            // 
            arcTree.Dock = System.Windows.Forms.DockStyle.Fill;
            arcTree.HideSelection = false;
            arcTree.ImageIndex = 0;
            arcTree.ImageList = imageList;
            arcTree.Location = new System.Drawing.Point(4, 19);
            arcTree.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            arcTree.Name = "arcTree";
            arcTree.PathSeparator = ".";
            arcTree.SelectedImageIndex = 0;
            arcTree.Size = new System.Drawing.Size(324, 735);
            arcTree.TabIndex = 1;
            arcTree.AfterCollapse += arcTree_AfterCollapse;
            arcTree.BeforeExpand += arcTree_BeforeExpand;
            arcTree.AfterSelect += arcTree_AfterSelect;
            arcTree.NodeMouseClick += arcTree_NodeMouseClick;
            // 
            // imageList
            // 
            imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList.ImageStream");
            imageList.TransparentColor = System.Drawing.Color.Transparent;
            imageList.Images.SetKeyName(0, "Library_16x.png");
            imageList.Images.SetKeyName(1, "Namespace_16x.png");
            imageList.Images.SetKeyName(2, "ModuleFile_16x.png");
            imageList.Images.SetKeyName(3, "Class_16x.png");
            imageList.Images.SetKeyName(4, "Field_16x.png");
            imageList.Images.SetKeyName(5, "Method_16x.png");
            imageList.Images.SetKeyName(6, "Constant_16x.png");
            // 
            // groupBox1
            // 
            groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(arcTree);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(332, 757);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Archive View";
            // 
            // groupBox2
            // 
            groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            groupBox2.Controls.Add(tabControl);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(0, 0);
            groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Size = new System.Drawing.Size(942, 757);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Editor";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 24);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Size = new System.Drawing.Size(1279, 757);
            splitContainer1.SplitterDistance = 332;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 5;
            // 
            // scriptCtxMenu
            // 
            scriptCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editModuleToolStripMenuItem, addClassToolStripMenuItem, deleteScriptToolStripMenuItem, toolStripSeparator2, exportScriptToolStripMenuItem, replaceScriptToolStripMenuItem, toolStripSeparator3, editXRefsToolStripMenuItem, optimizeScriptToolStripMenuItem });
            scriptCtxMenu.Name = "scriptCtxMenu";
            scriptCtxMenu.Size = new System.Drawing.Size(167, 170);
            // 
            // editModuleToolStripMenuItem
            // 
            editModuleToolStripMenuItem.Name = "editModuleToolStripMenuItem";
            editModuleToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            editModuleToolStripMenuItem.Text = "Edit Module";
            editModuleToolStripMenuItem.Click += editModuleToolStripMenuItem_Click;
            // 
            // addClassToolStripMenuItem
            // 
            addClassToolStripMenuItem.Name = "addClassToolStripMenuItem";
            addClassToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            addClassToolStripMenuItem.Text = "Add Class";
            addClassToolStripMenuItem.Click += addClassToolStripMenuItem_Click;
            // 
            // deleteScriptToolStripMenuItem
            // 
            deleteScriptToolStripMenuItem.Name = "deleteScriptToolStripMenuItem";
            deleteScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            deleteScriptToolStripMenuItem.Text = "Delete Module";
            deleteScriptToolStripMenuItem.Click += deleteScriptToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(163, 6);
            // 
            // exportScriptToolStripMenuItem
            // 
            exportScriptToolStripMenuItem.Name = "exportScriptToolStripMenuItem";
            exportScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            exportScriptToolStripMenuItem.Text = "Export Module";
            exportScriptToolStripMenuItem.Click += exportScriptToolStripMenuItem_Click;
            // 
            // replaceScriptToolStripMenuItem
            // 
            replaceScriptToolStripMenuItem.Name = "replaceScriptToolStripMenuItem";
            replaceScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            replaceScriptToolStripMenuItem.Text = "Replace Module";
            replaceScriptToolStripMenuItem.Click += replaceScriptToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(163, 6);
            // 
            // editXRefsToolStripMenuItem
            // 
            editXRefsToolStripMenuItem.Name = "editXRefsToolStripMenuItem";
            editXRefsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            editXRefsToolStripMenuItem.Text = "Edit XRefs";
            editXRefsToolStripMenuItem.Click += editXRefsToolStripMenuItem_Click;
            // 
            // optimizeScriptToolStripMenuItem
            // 
            optimizeScriptToolStripMenuItem.Name = "optimizeScriptToolStripMenuItem";
            optimizeScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            optimizeScriptToolStripMenuItem.Text = "Optimize Module";
            optimizeScriptToolStripMenuItem.Click += optimizeScriptToolStripMenuItem_Click;
            // 
            // classCtxMenu
            // 
            classCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { findUsesOfObjectToolStripMenuItem1, editClassToolStripMenuItem, deleteClassToolStripMenuItem, toolStripSeparator1, addVariableToolStripMenuItem, addFunctionToolStripMenuItem, addConstantToolStripMenuItem });
            classCtxMenu.Name = "classCtxMenu";
            classCtxMenu.Size = new System.Drawing.Size(177, 142);
            // 
            // findUsesOfObjectToolStripMenuItem1
            // 
            findUsesOfObjectToolStripMenuItem1.Name = "findUsesOfObjectToolStripMenuItem1";
            findUsesOfObjectToolStripMenuItem1.Size = new System.Drawing.Size(176, 22);
            findUsesOfObjectToolStripMenuItem1.Text = "Find Uses of Object";
            findUsesOfObjectToolStripMenuItem1.Click += findUsesOfObjectToolStripMenuItem_Click;
            // 
            // editClassToolStripMenuItem
            // 
            editClassToolStripMenuItem.Name = "editClassToolStripMenuItem";
            editClassToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            editClassToolStripMenuItem.Text = "Edit Class";
            editClassToolStripMenuItem.Click += editObjectToolStripMenuItem_Click;
            // 
            // deleteClassToolStripMenuItem
            // 
            deleteClassToolStripMenuItem.Name = "deleteClassToolStripMenuItem";
            deleteClassToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            deleteClassToolStripMenuItem.Text = "Delete Class";
            deleteClassToolStripMenuItem.Click += deleteClassToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // addVariableToolStripMenuItem
            // 
            addVariableToolStripMenuItem.Name = "addVariableToolStripMenuItem";
            addVariableToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            addVariableToolStripMenuItem.Text = "Add Variable";
            addVariableToolStripMenuItem.Click += addVariableToolStripMenuItem_Click;
            // 
            // addFunctionToolStripMenuItem
            // 
            addFunctionToolStripMenuItem.Name = "addFunctionToolStripMenuItem";
            addFunctionToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            addFunctionToolStripMenuItem.Text = "Add Function";
            addFunctionToolStripMenuItem.Click += addFunctionToolStripMenuItem_Click;
            // 
            // addConstantToolStripMenuItem
            // 
            addConstantToolStripMenuItem.Name = "addConstantToolStripMenuItem";
            addConstantToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            addConstantToolStripMenuItem.Text = "Add Enum";
            addConstantToolStripMenuItem.Click += addConstantToolStripMenuItem_Click;
            // 
            // genericCtxMenu
            // 
            genericCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { findUsesOfObjectToolStripMenuItem, copyFullNameToolStripMenuItem, toolStripSeparator5, editObjectToolStripMenuItem, deleteObjectToolStripMenuItem });
            genericCtxMenu.Name = "genericCtxMenu";
            genericCtxMenu.Size = new System.Drawing.Size(177, 98);
            // 
            // findUsesOfObjectToolStripMenuItem
            // 
            findUsesOfObjectToolStripMenuItem.Name = "findUsesOfObjectToolStripMenuItem";
            findUsesOfObjectToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            findUsesOfObjectToolStripMenuItem.Text = "Find Uses of Object";
            findUsesOfObjectToolStripMenuItem.Click += findUsesOfObjectToolStripMenuItem_Click;
            // 
            // copyFullNameToolStripMenuItem
            // 
            copyFullNameToolStripMenuItem.Name = "copyFullNameToolStripMenuItem";
            copyFullNameToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            copyFullNameToolStripMenuItem.Text = "Copy Full Name";
            copyFullNameToolStripMenuItem.Click += copyFullNameToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(173, 6);
            // 
            // editObjectToolStripMenuItem
            // 
            editObjectToolStripMenuItem.Name = "editObjectToolStripMenuItem";
            editObjectToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            editObjectToolStripMenuItem.Text = "Edit Object";
            editObjectToolStripMenuItem.Click += editObjectToolStripMenuItem_Click;
            // 
            // deleteObjectToolStripMenuItem
            // 
            deleteObjectToolStripMenuItem.Name = "deleteObjectToolStripMenuItem";
            deleteObjectToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            deleteObjectToolStripMenuItem.Text = "Delete Object";
            deleteObjectToolStripMenuItem.Click += deleteObjectToolStripMenuItem_Click;
            // 
            // editorCtxMenu
            // 
            editorCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { parseAsFloatToolStripMenuItem1, convertToDecimalToolStripMenuItem });
            editorCtxMenu.Name = "editorCtxMenu";
            editorCtxMenu.Size = new System.Drawing.Size(228, 48);
            // 
            // parseAsFloatToolStripMenuItem1
            // 
            parseAsFloatToolStripMenuItem1.Name = "parseAsFloatToolStripMenuItem1";
            parseAsFloatToolStripMenuItem1.Size = new System.Drawing.Size(227, 22);
            parseAsFloatToolStripMenuItem1.Text = "Convert Selection to Float";
            parseAsFloatToolStripMenuItem1.Click += parseAsFloatToolStripMenuItem_Click;
            // 
            // convertToDecimalToolStripMenuItem
            // 
            convertToDecimalToolStripMenuItem.Name = "convertToDecimalToolStripMenuItem";
            convertToDecimalToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            convertToDecimalToolStripMenuItem.Text = "Convert Selection to Decimal";
            convertToDecimalToolStripMenuItem.Click += convertToDecimalToolStripMenuItem_Click;
            // 
            // archiveMenuStrip
            // 
            archiveMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { buildMenuItem, buildAsMenuItem, toolStripSeparator6, addModuleMenuItem, importModulesToolStripMenuItem, toolStripSeparator7, viewPropertiesToolStripMenuItem, dumpHashesToolStripMenuItem1, closeArchiveToolStripMenuItem });
            archiveMenuStrip.Name = "archiveMenuStrip";
            archiveMenuStrip.Size = new System.Drawing.Size(160, 170);
            // 
            // buildMenuItem
            // 
            buildMenuItem.Name = "buildMenuItem";
            buildMenuItem.Size = new System.Drawing.Size(159, 22);
            buildMenuItem.Text = "Build";
            buildMenuItem.Click += buildMenuItem_Click;
            // 
            // buildAsMenuItem
            // 
            buildAsMenuItem.Name = "buildAsMenuItem";
            buildAsMenuItem.Size = new System.Drawing.Size(159, 22);
            buildAsMenuItem.Text = "Build As...";
            buildAsMenuItem.Click += buildAsMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(156, 6);
            // 
            // addModuleMenuItem
            // 
            addModuleMenuItem.Name = "addModuleMenuItem";
            addModuleMenuItem.Size = new System.Drawing.Size(159, 22);
            addModuleMenuItem.Text = "Add Module";
            // 
            // importModulesToolStripMenuItem
            // 
            importModulesToolStripMenuItem.Name = "importModulesToolStripMenuItem";
            importModulesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            importModulesToolStripMenuItem.Text = "Import Modules";
            importModulesToolStripMenuItem.Click += importModulesToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(156, 6);
            // 
            // viewPropertiesToolStripMenuItem
            // 
            viewPropertiesToolStripMenuItem.Name = "viewPropertiesToolStripMenuItem";
            viewPropertiesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            viewPropertiesToolStripMenuItem.Text = "View Properties";
            viewPropertiesToolStripMenuItem.Click += viewPropertiesToolStripMenuItem_Click;
            // 
            // dumpHashesToolStripMenuItem1
            // 
            dumpHashesToolStripMenuItem1.Name = "dumpHashesToolStripMenuItem1";
            dumpHashesToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            dumpHashesToolStripMenuItem1.Text = "Dump Hashes";
            // 
            // closeArchiveToolStripMenuItem
            // 
            closeArchiveToolStripMenuItem.Name = "closeArchiveToolStripMenuItem";
            closeArchiveToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            closeArchiveToolStripMenuItem.Text = "Close Archive";
            closeArchiveToolStripMenuItem.Click += closeArchiveToolStripMenuItem_Click;
            // 
            // namespaceMenuStrip
            // 
            namespaceMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { exportAllModulesToolStripMenuItem });
            namespaceMenuStrip.Name = "namespaceMenuStrip";
            namespaceMenuStrip.Size = new System.Drawing.Size(175, 26);
            // 
            // exportAllModulesToolStripMenuItem
            // 
            exportAllModulesToolStripMenuItem.Name = "exportAllModulesToolStripMenuItem";
            exportAllModulesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            exportAllModulesToolStripMenuItem.Text = "Export All Modules";
            exportAllModulesToolStripMenuItem.Click += exportAllModulesToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1279, 781);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Mint Workshop";
            FormClosing += MainForm_FormClosing;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            scriptCtxMenu.ResumeLayout(false);
            classCtxMenu.ResumeLayout(false);
            genericCtxMenu.ResumeLayout(false);
            editorCtxMenu.ResumeLayout(false);
            archiveMenuStrip.ResumeLayout(false);
            namespaceMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructionDictionaryToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem editXRefsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parseAsFloatToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip editorCtxMenu;
        private System.Windows.Forms.ToolStripMenuItem parseAsFloatToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem findUsesOfObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem convertToDecimalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToDecimalToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optimizeScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadHashesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyFullNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchForHashUsageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findUsesOfObjectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openRtDLToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip archiveMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem buildMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildAsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem addModuleMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem closeArchiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importModulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpHashesToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip namespaceMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exportAllModulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPropertiesToolStripMenuItem;
    }
}

