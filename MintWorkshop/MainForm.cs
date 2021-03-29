using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MintWorkshop.Editors;
using MintWorkshop.Mint;
using MintWorkshop.Types;
using MintWorkshop.Util;

namespace MintWorkshop
{
    public partial class MainForm : Form
    {
        string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        float fontSize = 9;
        bool loading = false;

        Dictionary<byte[], string> hashes;
        Archive archive;
        string filePath;

        public MainForm()
        {
            hashes = new Dictionary<byte[], string>(new ByteArrayComparer());
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Binary Files|*.bin";
            open.CheckFileExists = true;
            open.AddExtension = true;
            open.DefaultExt = ".bin";
            if (open.ShowDialog() == DialogResult.OK)
            {
                arcTree.Nodes.Clear();
                arcTree.BeginUpdate();

                filePath = open.FileName;

                using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
                    archive = new Archive(reader);

                hashes = new Dictionary<byte[], string>(new ByteArrayComparer());
                if (File.Exists(exeDir + $"\\hashes_{archive.GetSemanticVersion()}.txt"))
                {
                    Console.WriteLine($"Hash file found for version {archive.GetSemanticVersion()}");
                    using (StreamReader reader = new StreamReader(exeDir + $"\\hashes_{archive.GetSemanticVersion()}.txt"))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (!line.StartsWith("#"))
                            {
                                byte[] hash = HashCalculator.Calculate(line);
                                if (!hashes.ContainsKey(hash)) hashes.Add(hash, line);
                            }
                        }
                    }
                    Console.WriteLine($"Finished reading {hashes.Keys.Count} hashes");
                }
                foreach (KeyValuePair<byte[], string> pair in archive.GetHashes())
                    if (!hashes.ContainsKey(pair.Key)) hashes.Add(pair.Key, pair.Value);

                TreeNode root = new TreeNode();
                for (int i = 0; i < archive.Namespaces.Count; i++)
                    NamespaceNodeSearch(root, archive.Namespaces[i].Name.Split('.'), 0);
                for (int i = 0; i < archive.Scripts.Count; i++)
                    ScriptNodeSearch(root, archive.Scripts.Keys.ElementAt(i).Split('.'), 0);
                foreach (TreeNode n in root.Nodes)
                    arcTree.Nodes.Add(n);

                this.Text = "MintWorkshop - " + filePath;

                arcTree.EndUpdate();
            }
        }

        private void NamespaceNodeSearch(TreeNode node, string[] search, int searchIndex)
        {
            if (searchIndex < search.Length)
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    if (node.Nodes[i].Text == search[searchIndex])
                    {
                        NamespaceNodeSearch(node.Nodes[i], search, searchIndex + 1);
                        return;
                    }
                }
                node.Nodes.Add(new TreeNode(search[searchIndex], 0, 0));
            }
        }

        private void ScriptNodeSearch(TreeNode node, string[] search, int searchIndex)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (node.Nodes[i].Text == search[searchIndex])
                {
                    ScriptNodeSearch(node.Nodes[i], search, searchIndex + 1);
                    return;
                }
            }
            TreeNode scriptNode = new TreeNode(search[searchIndex], 1, 1);
            scriptNode.ContextMenuStrip = scriptCtxMenu;

            MintScript s = archive.Scripts[string.Join(".", search)];
            for (int i = 0; i < s.Classes.Count; i++)
            {
                TreeNode cl = new TreeNode(s.Classes[i].Name.Split('.').Last(), 2, 2);
                cl.ContextMenuStrip = classCtxMenu;
                cl.Nodes.AddRange(new TreeNode[] { new TreeNode("Variables", 3, 3), new TreeNode("Functions", 4, 4), new TreeNode("Constants", 5, 5) });

                for (int v = 0; v < s.Classes[i].Variables.Count; v++)
                {
                    cl.Nodes[0].Nodes.Add($"{s.Classes[i].Variables[v].Hash[0]:X2}{s.Classes[i].Variables[v].Hash[1]:X2}{s.Classes[i].Variables[v].Hash[2]:X2}{s.Classes[i].Variables[v].Hash[3]:X2}", s.Classes[i].Variables[v].Type + " " + s.Classes[i].Variables[v].Name, 3, 3);
                    cl.Nodes[0].Nodes[v].ContextMenuStrip = genericCtxMenu;
                }
                for (int v = 0; v < s.Classes[i].Functions.Count; v++)
                {
                    cl.Nodes[1].Nodes.Add($"{s.Classes[i].Functions[v].Hash[0]:X2}{s.Classes[i].Functions[v].Hash[1]:X2}{s.Classes[i].Functions[v].Hash[2]:X2}{s.Classes[i].Functions[v].Hash[3]:X2}", s.Classes[i].Functions[v].Name, 4, 4);
                    cl.Nodes[1].Nodes[v].ContextMenuStrip = genericCtxMenu;
                }
                for (int v = 0; v < s.Classes[i].Constants.Count; v++)
                {
                    cl.Nodes[2].Nodes.Add(new TreeNode(s.Classes[i].Constants[v].Name, 5, 5));
                    cl.Nodes[2].Nodes[v].ContextMenuStrip = genericCtxMenu;
                }

                scriptNode.Nodes.Add(cl);
            }

            node.Nodes.Add(scriptNode);
        }

        void CreateEditor(MintFunction function)
        {
            loading = true; 

            string hash = $"{function.Hash[0]:X2}{function.Hash[1]:X2}{function.Hash[2]:X2}{function.Hash[3]:X2}";
            if (!tabControl.TabPages.ContainsKey(hash))
            {
                RichTextBox box = new RichTextBox();
                box.BorderStyle = BorderStyle.FixedSingle;
                box.Dock = DockStyle.Fill;
                box.Font = new Font(new FontFamily("Courier New"), fontSize);
                box.ScrollBars = RichTextBoxScrollBars.Both;
                box.WordWrap = false;
                box.TextChanged += textBoxEdited;

                box.AppendText("[");
                box.AppendText("Flags", TextColors.SDataColor);
                box.AppendText(":");
                box.AppendText(function.Flags.ToString(), TextColors.ConstantColor);
                box.AppendText("] ");
                box.AppendText(function.Name);
                box.AppendText("\n\n");

                function.Disassemble(ref hashes, ref box);
                box.SelectionStart = 0;
                box.ScrollToCaret();
                box.ClearUndo();

                tabControl.TabPages.Add(hash, function.FullNameWithoutSignature());
                tabControl.TabPages[tabControl.TabCount - 1].Controls.Add(box);
                tabControl.TabPages[tabControl.TabCount - 1].Name = hash;
            }
            tabControl.SelectedTab = tabControl.TabPages[hash];

            loading = false;
        }

        void CloseEditor(int index)
        {
            if (index > 0)
            {
                if (tabControl.SelectedTab.Text.EndsWith("*"))
                {
                    if (MessageBox.Show("Are you sure you want to close this tab?" +
                                      "\nThis function has been edited, closing it without saving will lose any changes you have made.",
                                      "MintWorkshop", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }

                loading = true;
                tabControl.TabPages.RemoveAt(index);
                tabControl.SelectedIndex = index - 1;
                loading = false;
            }
        }

        private void arcTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (arcTree.SelectedNode == null || arcTree.SelectedNode.Parent == null)
                return;

            if (arcTree.SelectedNode.Parent.Text == "Functions")
            {
                string scriptName = arcTree.SelectedNode.Parent.Parent.Parent.FullPath;
                int classIndex = arcTree.SelectedNode.Parent.Parent.Index;
                int funcIndex = arcTree.SelectedNode.Index;
                CreateEditor(archive.Scripts[scriptName].Classes[classIndex].Functions[funcIndex]);
            }
        }

        private void tabControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
                return;

            for (int i = 1; i < tabControl.TabPages.Count; i++)
            {
                if (tabControl.GetTabRect(i).Contains(e.Location))
                {
                    CloseEditor(i);
                    break;
                }
            }
        }

        private void textBoxEdited(object sender, EventArgs e)
        {
            /*
             * TODO
             * Recolor text when edited but not where it takes way too long if the function is massive
            */

            if (loading) return;

            if (tabControl.SelectedIndex > 0 && !tabControl.SelectedTab.Text.EndsWith("*"))
                tabControl.SelectedTab.Text += " *";
        }

        private void saveTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                TreeNode node = arcTree.Nodes.Find(tabControl.SelectedTab.Name, true)[0];
                string scriptName = node.Parent.Parent.Parent.FullPath;
                int classIndex = node.Parent.Parent.Index;
                int funcIndex = node.Index;
                archive.Scripts[scriptName].Classes[classIndex].Functions[funcIndex].Assemble((tabControl.SelectedTab.Controls[0] as RichTextBox).Lines);
            }
            if (tabControl.SelectedTab.Text.EndsWith("*"))
            {
                tabControl.SelectedTab.Text = tabControl.SelectedTab.Text.Remove(tabControl.SelectedTab.Text.Length - 2, 2);
            }
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseEditor(tabControl.SelectedIndex);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.Write(filePath);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Binary Files|*.bin";
            save.AddExtension = true;
            save.DefaultExt = ".bin";
            if (save.ShowDialog() == DialogResult.OK)
            {
                filePath = save.FileName;
                archive.Write(filePath);
                this.Text = "MintWorkshop - " + filePath;
            }
        }

        private void closeAllTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = 0;
            for (int i = tabControl.TabPages.Count - 1; i > 0; i--)
            {
                tabControl.TabPages.RemoveAt(i);
            }
        }

        private void addClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string parentScript = arcTree.SelectedNode.FullPath;
            MintClass newClass = new MintClass(parentScript + ".NewClass", 0, archive.Scripts[parentScript]);
            EditClassForm edit = new EditClassForm(newClass);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                newClass.SetName(edit.ClassName);
                if (!newClass.Name.StartsWith(archive.Scripts[parentScript].Name + "."))
                    newClass.SetName(archive.Scripts[parentScript].Name + "." + edit.ClassName.Split('.').Last());

                newClass.Flags = edit.ClassFlags;
                archive.Scripts[parentScript].Classes.Add(newClass);

                TreeNode cl = new TreeNode(newClass.Name.Split('.').Last(), 2, 2);
                cl.ContextMenuStrip = classCtxMenu;
                cl.Nodes.AddRange(new TreeNode[] { new TreeNode("Variables", 3, 3), new TreeNode("Functions", 4, 4), new TreeNode("Constants", 5, 5) });
                arcTree.SelectedNode.Nodes.Add(cl);
            }
        }

        private void deleteScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this script?\nThis action cannot be undone.", "MintWorkshop", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string scriptName = arcTree.SelectedNode.FullPath;
                archive.Scripts.Remove(scriptName);
                arcTree.SelectedNode.Remove();
            }
        }

        private void editClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string parentScript = arcTree.SelectedNode.Parent.FullPath;
            int index = arcTree.SelectedNode.Index;
            EditClassForm edit = new EditClassForm(archive.Scripts[parentScript].Classes[index]);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                archive.Scripts[parentScript].Classes[index].SetName(edit.ClassName);
                archive.Scripts[parentScript].Classes[index].Flags = edit.ClassFlags;
                archive.Scripts[parentScript].Classes[index].UnknownList = edit.ClassUnknowns;

                arcTree.SelectedNode.Text = edit.ClassName.Split('.').Last();
            }
        }

        private void deleteClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this class?\nThis action cannot be undone.", "MintWorkshop", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string parentScript = arcTree.SelectedNode.Parent.FullPath;
                int index = arcTree.SelectedNode.Index;
                archive.Scripts[parentScript].Classes.RemoveAt(index);
                arcTree.SelectedNode.Remove();
            }
        }

        private void addVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string parentScript = arcTree.SelectedNode.Parent.FullPath;
            int index = arcTree.SelectedNode.Index;
            MintVariable newVar = new MintVariable("newVariable", "int", 0, archive.Scripts[parentScript].Classes[index]);
            EditVariableForm edit = new EditVariableForm(newVar);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                newVar.SetName(edit.VariableName);
                newVar.Type = edit.VariableType;
                newVar.Flags = edit.VariableFlags;
                archive.Scripts[parentScript].Classes[index].Variables.Add(newVar);

                arcTree.SelectedNode.Nodes[0].Nodes.Add($"{newVar.Hash[0]:X2}{newVar.Hash[1]:X2}{newVar.Hash[2]:X2}{newVar.Hash[3]:X2}", newVar.Type + " " + newVar.Name, 3, 3);
                arcTree.SelectedNode.Nodes[0].Nodes[arcTree.SelectedNode.Nodes[0].Nodes.Count - 1].ContextMenuStrip = genericCtxMenu;
            }
        }

        private void addFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string parentScript = arcTree.SelectedNode.Parent.FullPath;
            int index = arcTree.SelectedNode.Index;
            MintFunction newFunc = new MintFunction("void newFunction()", 0, archive.Scripts[parentScript].Classes[index]);
            EditFunctionForm edit = new EditFunctionForm(newFunc);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                newFunc.SetName(edit.FunctionName);
                newFunc.Flags = edit.FunctionFlags;
                archive.Scripts[parentScript].Classes[index].Functions.Add(newFunc);

                arcTree.SelectedNode.Nodes[1].Nodes.Add($"{newFunc.Hash[0]:X2}{newFunc.Hash[1]:X2}{newFunc.Hash[2]:X2}{newFunc.Hash[3]:X2}", newFunc.Name, 4, 4);
                arcTree.SelectedNode.Nodes[1].Nodes[arcTree.SelectedNode.Nodes[1].Nodes.Count - 1].ContextMenuStrip = genericCtxMenu;
            }
        }

        private void addConstantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string parentScript = arcTree.SelectedNode.Parent.FullPath;
            int index = arcTree.SelectedNode.Index;
            MintClass.Constant newConst = new MintClass.Constant("newConstant", 0);
            EditConstantForm edit = new EditConstantForm(newConst);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                newConst.Name = edit.ConstantName;
                newConst.Value = edit.ConstantValue;
                archive.Scripts[parentScript].Classes[index].Constants.Add(newConst);

                arcTree.SelectedNode.Nodes[2].Nodes.Add(new TreeNode(newConst.Name, 5, 5));
                arcTree.SelectedNode.Nodes[2].Nodes[arcTree.SelectedNode.Nodes[2].Nodes.Count - 1].ContextMenuStrip = genericCtxMenu;
            }
        }

        private void editObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string parentScript = arcTree.SelectedNode.Parent.Parent.Parent.FullPath;
            int classIndex = arcTree.SelectedNode.Parent.Parent.Index;
            int index = arcTree.SelectedNode.Index;
            switch (arcTree.SelectedNode.Parent.Text)
            {
                case "Variables":
                    {
                        EditVariableForm edit = new EditVariableForm(archive.Scripts[parentScript].Classes[classIndex].Variables[index]);
                        if (edit.ShowDialog() == DialogResult.OK)
                        {
                            archive.Scripts[parentScript].Classes[classIndex].Variables[index].SetName(edit.VariableName);
                            archive.Scripts[parentScript].Classes[classIndex].Variables[index].Type = edit.VariableType;
                            archive.Scripts[parentScript].Classes[classIndex].Variables[index].Flags = edit.VariableFlags;

                            arcTree.SelectedNode.Text = edit.VariableType + " " + edit.VariableName;
                        }
                        break;
                    }
                case "Functions":
                    {
                        EditFunctionForm edit = new EditFunctionForm(archive.Scripts[parentScript].Classes[classIndex].Functions[index]);
                        if (edit.ShowDialog() == DialogResult.OK)
                        {
                            archive.Scripts[parentScript].Classes[classIndex].Functions[index].SetName(edit.FunctionName);
                            archive.Scripts[parentScript].Classes[classIndex].Functions[index].Flags = edit.FunctionFlags;

                            arcTree.SelectedNode.Text = edit.FunctionName;
                        }
                        break;
                    }
                case "Constants":
                    {
                        EditConstantForm edit = new EditConstantForm(archive.Scripts[parentScript].Classes[classIndex].Constants[index]);
                        if (edit.ShowDialog() == DialogResult.OK)
                        {
                            MintClass.Constant c = new MintClass.Constant(edit.ConstantName, edit.ConstantValue);
                            archive.Scripts[parentScript].Classes[classIndex].Constants[index] = c;

                            arcTree.SelectedNode.Text = edit.ConstantName;
                        }
                        break;
                    }
            }
        }

        private void deleteObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to delete this {arcTree.SelectedNode.Parent.Text.TrimEnd('s').ToLower()}?\nThis action cannot be undone.", "MintWorkshop", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string parentScript = arcTree.SelectedNode.Parent.Parent.Parent.FullPath;
                int classIndex = arcTree.SelectedNode.Parent.Parent.Index;
                int index = arcTree.SelectedNode.Index;
                switch (arcTree.SelectedNode.Parent.Text)
                {
                    case "Variables":
                        {
                            archive.Scripts[parentScript].Classes[classIndex].Variables.RemoveAt(index);
                            break;
                        }
                    case "Functions":
                        {
                            archive.Scripts[parentScript].Classes[classIndex].Functions.RemoveAt(index);
                            break;
                        }
                    case "Constants":
                        {
                            archive.Scripts[parentScript].Classes[classIndex].Constants.RemoveAt(index);
                            break;
                        }
                }
                arcTree.SelectedNode.Remove();
            }
        }

        //Unfinished
        private void addScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = arcTree.SelectedNode.FullPath;
            EditGenericForm edit = new EditGenericForm(name + ".NewScript");
            if (edit.ShowDialog() == DialogResult.OK)
            {
                MintScript newScript = new MintScript(edit.StringName, archive.Version);
                if (!newScript.Name.StartsWith(name + "."))
                    newScript.Name = name + "." + edit.StringName.Split('.').Last();

                archive.Scripts.Add(newScript.Name, newScript);

                TreeNode scriptNode = new TreeNode(newScript.Name, 1, 1);
                scriptNode.ContextMenuStrip = scriptCtxMenu;
                arcTree.SelectedNode.Nodes.Add(scriptNode);
            }
        }

        //Unfinished
        private void addNamespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = arcTree.SelectedNode.FullPath;
            EditGenericForm edit = new EditGenericForm(name + ".NewScript");
            if (edit.ShowDialog() == DialogResult.OK)
            {
                string[] n = name.Split('.');
                string parentName = string.Join(".", n.Take(n.Length - 1));
                var p = archive.Namespaces.Where(x => x.Name == parentName);
                if (p.Count() > 0)
                {
                    int i = archive.Namespaces.IndexOf(p.First());
                    Archive.Namespace parent = archive.Namespaces[i];
                    parent.ChildNamespaces++;
                    archive.Namespaces[i] = parent;

                    Archive.Namespace newNamespace = new Archive.Namespace(parent.Index, edit.StringName, 0, parent.TotalScripts, 0);
                }
                else
                {
                    MessageBox.Show("Error: Could not find parent Namespace.", "MintWorkshop", MessageBoxButtons.OK);
                }
            }
        }

        private void exportScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MintScript script = archive.Scripts[arcTree.SelectedNode.FullPath];
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Mint Script Binary Files|*.bin";
            save.FileName = script.Name + ".bin";
            if (save.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(save.FileName, script.Write());
                MessageBox.Show($"Exported Mint script to\n{save.FileName}", "MintWorkshop", MessageBoxButtons.OK);
            }
        }

        private void replaceScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Mint Script Binary Files|*.bin";
            open.CheckFileExists = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(open.FileName, FileMode.Open, FileAccess.Read)))
                {
                    MintScript script = new MintScript(reader, archive.Version);
                    if (script.Name != arcTree.SelectedNode.FullPath)
                    {
                        MessageBox.Show("Error: Script has a different name than the one being replaced.", "MintWorkshop", MessageBoxButtons.OK);
                        return;
                    }

                    archive.Scripts[arcTree.SelectedNode.FullPath] = script;
                    arcTree.SelectedNode.Nodes.Clear();
                    for (int i = 0; i < script.Classes.Count; i++)
                    {
                        TreeNode cl = new TreeNode(script.Classes[i].Name.Split('.').Last(), 2, 2);
                        cl.ContextMenuStrip = classCtxMenu;
                        cl.Nodes.AddRange(new TreeNode[] { new TreeNode("Variables", 3, 3), new TreeNode("Functions", 4, 4), new TreeNode("Constants", 5, 5) });

                        for (int v = 0; v < script.Classes[i].Variables.Count; v++)
                        {
                            cl.Nodes[0].Nodes.Add($"{script.Classes[i].Variables[v].Hash[0]:X2}{script.Classes[i].Variables[v].Hash[1]:X2}{script.Classes[i].Variables[v].Hash[2]:X2}{script.Classes[i].Variables[v].Hash[3]:X2}", script.Classes[i].Variables[v].Type + " " + script.Classes[i].Variables[v].Name, 3, 3);
                            cl.Nodes[0].Nodes[v].ContextMenuStrip = genericCtxMenu;
                        }
                        for (int v = 0; v < script.Classes[i].Functions.Count; v++)
                        {
                            cl.Nodes[1].Nodes.Add($"{script.Classes[i].Functions[v].Hash[0]:X2}{script.Classes[i].Functions[v].Hash[1]:X2}{script.Classes[i].Functions[v].Hash[2]:X2}{script.Classes[i].Functions[v].Hash[3]:X2}", script.Classes[i].Functions[v].Name, 4, 4);
                            cl.Nodes[1].Nodes[v].ContextMenuStrip = genericCtxMenu;
                        }
                        for (int v = 0; v < script.Classes[i].Constants.Count; v++)
                        {
                            cl.Nodes[2].Nodes.Add(new TreeNode(script.Classes[i].Constants[v].Name, 5, 5));
                            cl.Nodes[2].Nodes[v].ContextMenuStrip = genericCtxMenu;
                        }

                        arcTree.SelectedNode.Nodes.Add(cl);
                    }
                }
            }
        }
    }
}
