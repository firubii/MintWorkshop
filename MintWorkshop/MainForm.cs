using BrawlLib.Internal;
using BrawlLib.SSBB.ResourceNodes;
using BrawlLib.Wii;
using BrawlLib.Wii.Compression;
using KirbyLib;
using KirbyLib.Crypto;
using KirbyLib.IO;
using KirbyLib.Mint;
using MintWorkshop.Editors;
using MintWorkshop.Mint;
using MintWorkshop.Nodes;
using MintWorkshop.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAPICodePack.Dialogs;

namespace MintWorkshop
{
    public struct ArchiveContext
    {
        public string Path;
        public Archive Archive;
        public ArchiveRtDL ArchiveRtDL;
        public bool IsCompressed;
    }

    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static Config Config;

        public static readonly byte[] RTDL_VERSION = { 0, 2, 0, 0 };

        string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        bool loading = false;

        Dictionary<uint, string> hashes = new();
        List<ArchiveContext> archives = new();

        HashSelector hashSelector;

        public MainForm()
        {
            Config = new Config();
            if (File.Exists(exeDir + "\\Config.xml"))
                Config.Load(exeDir + "\\Config.xml");
            else
                Config.Save(exeDir + "\\Config.xml");

            InitializeComponent();

            this.arcTree.NodeMouseClick += (sender, args) => arcTree.SelectedNode = args.Node;

            //AllocConsole();

            hashSelector = new HashSelector();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void CloseAllArchives()
        {
            while (tabControl.TabPages.Count > 0)
                CloseEditor(0, true);

            arcTree.Nodes.Clear();

            archives.Clear();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Binary Files|*.bin;*.bin.cmp";
            open.CheckFileExists = true;
            open.AddExtension = true;
            open.DefaultExt = ".bin";
            if (open.ShowDialog() == DialogResult.OK)
            {
                ProgressBar progress = new ProgressBar();
                Task.Run(() =>
                {
                    Task.Run(() => { Invoke((MethodInvoker)delegate { progress.ShowDialog(); }); });

                    Invoke((MethodInvoker)delegate
                    {
                        progress.SetValue(0);
                        progress.SetMax(1);
                        progress.SetTitle("Reading Archive...");
                    });

                    bool isCompressed = false;
                    EndianBinaryReader reader = new EndianBinaryReader(new FileStream(open.FileName, FileMode.Open, FileAccess.Read));
                    if (reader.ReadByte() == 0x11)
                    {
                        isCompressed = true;
                        DataSource dataSrc = new DataSource(new MemoryStream(File.ReadAllBytes(open.FileName)), CompressionType.LZ77);
                        FileStream stream = Compressor.TryExpand(ref dataSrc, false).BaseStream;
                        stream.Lock(0, stream.Length);
                        reader = new EndianBinaryReader(stream);
                    }

                    reader.BaseStream.Position = 0;
                    Archive archive = new Archive(reader);

                    if (isCompressed)
                        (reader.BaseStream as FileStream).Unlock(0, reader.BaseStream.Length);

                    reader.Dispose();

                    archives.Add(new ArchiveContext()
                    {
                        Path = open.FileName,
                        Archive = archive,
                        IsCompressed = isCompressed
                    });

                    Invoke((MethodInvoker)delegate
                    {
                        progress.SetValue(0);
                        progress.SetMax(1);
                        progress.SetTitle("Updating hash list...");
                    });
                    ReloadHashes();

                    Invoke((MethodInvoker)delegate
                    {
                        progress.SetValue(0);
                        progress.SetMax(1);
                        progress.SetTitle("Cleaning up...");
                    });

                    Invoke((MethodInvoker)delegate
                    {
                        arcTree.BeginUpdate();
                        arcTree.Nodes.Add(new ArchiveTreeNode(archive)
                        {
                            Text = Path.GetFileName(open.FileName) + $" ({archive.GetVersionString()})",
                            ContextMenuStrip = archiveMenuStrip
                        });
                        progress.Close();
                        arcTree.EndUpdate();
                        closeToolStripMenuItem.Enabled = true;
                    });
                });
            }
        }

        private void openRtDLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Binary Files|*.bin;*.bin.cmp";
            open.CheckFileExists = true;
            open.AddExtension = true;
            open.DefaultExt = ".bin";
            if (open.ShowDialog() == DialogResult.OK)
            {
                ProgressBar progress = new ProgressBar();
                Task.Run(() =>
                {
                    Task.Run(() => { Invoke((MethodInvoker)delegate { progress.ShowDialog(); }); });

                    Invoke((MethodInvoker)delegate
                    {
                        progress.SetValue(0);
                        progress.SetMax(1);
                        progress.SetTitle("Reading Archive");
                    });

                    bool isCompressed = false;
                    EndianBinaryReader reader = new EndianBinaryReader(new FileStream(open.FileName, FileMode.Open, FileAccess.Read));
                    if (reader.ReadByte() == 0x11)
                    {
                        isCompressed = true;
                        DataSource dataSrc = new DataSource(new MemoryStream(File.ReadAllBytes(open.FileName)), CompressionType.LZ77);
                        FileStream stream = Compressor.TryExpand(ref dataSrc, false).BaseStream;
                        stream.Lock(0, stream.Length);
                        reader = new EndianBinaryReader(stream);
                    }

                    reader.BaseStream.Position = 0;
                    ArchiveRtDL archive = new ArchiveRtDL(reader);

                    if (isCompressed)
                        (reader.BaseStream as FileStream).Unlock(0, reader.BaseStream.Length);

                    reader.Dispose();

                    archives.Add(new ArchiveContext()
                    {
                        Path = open.FileName,
                        ArchiveRtDL = archive,
                        IsCompressed = isCompressed
                    });

                    Invoke((MethodInvoker)delegate
                    {
                        progress.SetValue(0);
                        progress.SetMax(1);
                        progress.SetTitle("Cleaning up...");
                    });

                    Invoke((MethodInvoker)delegate
                    {
                        arcTree.BeginUpdate();
                        arcTree.Nodes.Add(new ArchiveRtDLTreeNode(archive)
                        {
                            Text = Path.GetFileName(open.FileName) + $" ({archive.GetVersionString()})",
                            ContextMenuStrip = archiveMenuStrip
                        });
                        progress.Close();
                        arcTree.EndUpdate();
                        closeToolStripMenuItem.Enabled = true;
                    });
                });
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllArchives();
        }

        private void ReloadHashes()
        {
            hashes.Clear();
            hashes = new Dictionary<uint, string>();

            foreach (var ctx in archives)
            {
                if (ctx.Archive == null)
                    continue;

                Archive archive = ctx.Archive;
                if (File.Exists(exeDir + $"\\hashes_{archive.GetVersionString()}.txt"))
                {
                    Console.WriteLine($"Hash file found for version {archive.GetVersionString()}");
                    using (StreamReader reader = new StreamReader(exeDir + $"\\hashes_{archive.GetVersionString()}.txt"))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (!line.StartsWith("#"))
                            {
                                uint hash = Crc32C.CalculateInv(line);
                                if (!hashes.ContainsKey(hash)) hashes.Add(hash, line);
                            }
                        }
                    }
                    Console.WriteLine($"Finished reading {hashes.Keys.Count} hashes");
                }

                for (int i = 0; i < archive.Modules.Count; i++)
                {
                    Module mod = archive[i];
                    for (int o = 0; o < mod.Objects.Count; o++)
                    {
                        MintObject obj = mod[o];
                        uint hash = Crc32C.CalculateInv(obj.Name);
                        if (!hashes.ContainsKey(hash))
                            hashes.Add(hash, obj.Name);

                        for (int j = 0; j < obj.Variables.Count; j++)
                        {
                            hash = Crc32C.CalculateInv(obj.Name + "." + obj.Variables[j].Name);
                            if (!hashes.ContainsKey(hash))
                                hashes.Add(hash, obj.Name + "." + obj.Variables[j].Name);
                        }

                        for (int j = 0; j < obj.Functions.Count; j++)
                        {
                            hash = Crc32C.CalculateInv(obj.Name + "." + obj.Functions[j].NameWithoutType());
                            if (!hashes.ContainsKey(hash))
                                hashes.Add(hash, obj.Name + "." + obj.Functions[j].NameWithoutType());
                        }
                    }
                }
            }

            hashSelector.UpdateHashList(hashes.Values.ToArray());
        }

        void CreateEditor(Archive archive, Module module, MintObject obj, MintFunction function)
        {
            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                var page = tabControl.TabPages[i];
                if (page is TextEditorTab)
                {
                    if ((page as TextEditorTab).Function == function)
                    {
                        tabControl.SelectedTab = page;
                        return;
                    }
                }
            }

            TextEditorTab tab = new TextEditorTab(archive, module, obj, function, archive.Version);
            tab.Name = function.Name;
            tab.Text = module.Name + "." + function.NameWithoutType();

            tab.TextBox.SelectionStart = 0;
            tab.TextBox.ScrollToCaret();
            tab.TextBox.ClearUndo();

            tabControl.TabPages.Add(tab);

            tabControl.SelectedTab = tab;

            tab.IsLoading = true;
            tab.TextBox.Enabled = false;
            tab.TextBox.UseWaitCursor = true;
            Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        string disasm = FunctionUtil.Disassemble(module, obj, function, archive.Version, ref hashes);

                        tab.TextBox.AppendText(disasm);
                        tab.UpdateTextColor();
                        tab.TextBox.Enabled = true;
                        tab.TextBox.UseWaitCursor = false;

                        tab.IsLoading = false;

                        tab.SetContextMenuStrip(editorCtxMenu);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[{tab.Text}] Disasm error: {e}");
                    }
                });
            });
        }

        void CreateEditor(ArchiveRtDL archive, ModuleRtDL module, MintFunction function)
        {
            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                var page = tabControl.TabPages[i];
                if (page is TextEditorTab)
                {
                    if ((page as TextEditorTab).Function == function)
                    {
                        tabControl.SelectedTab = page;
                        return;
                    }
                }
            }

            TextEditorTab tab = new TextEditorTab(archive, module, function, RTDL_VERSION);
            tab.Name = function.Name;
            tab.Text = module.Name + "." + function.NameWithoutType();

            tab.TextBox.SelectionStart = 0;
            tab.TextBox.ScrollToCaret();
            tab.TextBox.ClearUndo();

            tabControl.TabPages.Add(tab);

            tabControl.SelectedTab = tab;

            tab.IsLoading = true;
            tab.TextBox.Enabled = false;
            tab.TextBox.UseWaitCursor = true;
            Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        string disasm = FunctionUtil.Disassemble(module, function);

                        tab.TextBox.AppendText(disasm);
                        tab.UpdateTextColor();
                        tab.TextBox.Enabled = true;
                        tab.TextBox.UseWaitCursor = false;

                        tab.IsLoading = false;

                        tab.SetContextMenuStrip(editorCtxMenu);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[{tab.Text}] Disasm error: {e}");
                    }
                });
            });
        }

        void CloseEditor(int index, bool forceClose)
        {
            TextEditorTab tab = tabControl.TabPages[index] as TextEditorTab;
            if (tab.IsDirty && !forceClose)
            {
                if (MessageBox.Show("Are you sure you want to close this tab?" +
                                    "\nThis function has been edited, closing it without saving will lose any changes you have made.",
                                    "Mint Workshop", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            loading = true;
            if (tabControl.SelectedIndex == index)
                tabControl.SelectedIndex = index - 1;

            tab.TextBox.Clear();
            tab.TextBox.ClearUndo();

            tabControl.TabPages.RemoveAt(index);
            loading = false;
        }

        private void SearchForHash(uint searchHash)
        {
            List<string> scripts = new List<string>();
            foreach (var ctx in archives)
            {
                if (ctx.Archive == null)
                    continue;

                Archive archive = ctx.Archive;
                Opcode[] opcodes = MintVersions.Versions[archive.Version];
                foreach (var module in archive.Modules)
                {
                    if (!module.XRef.Contains(searchHash))
                        continue;

                    for (int c = 0; c < module.Objects.Count; c++)
                    {
                        MintObject obj = module[c];
                        for (int f = 0; f < obj.Implements.Count; f++)
                        {
                            if (obj.Implements[f] == searchHash)
                                scripts.Add("[Implemented by] " + obj.Name);
                        }
                        /*
                        for (int f = 0; f < module.Objects[c].Extends.Count; f++)
                        {
                            if (!pair.Value.Classes[c].Extends[f].StdType &&
                                ByteArrayComparer.Equal(searchHash, pair.Value.XRef[pair.Value.Classes[c].Extends[f].Index]))
                                scripts.Add("[Extended by] " + pair.Value.Classes[c].Name);
                        }
                        */

                        for (int f = 0; f < obj.Functions.Count; f++)
                        {
                            MintFunction func = obj.Functions[f];
                            for (int i = 0; i < func.Data.Length; i += 4)
                            {
                                if (func.Data[i] >= opcodes.Length)
                                    continue;
                                if (opcodes[func.Data[i]].Arguments == null)
                                    continue;
                                for (int a = 0; a < opcodes[func.Data[i]].Arguments.Length; a++)
                                {
                                    int xrefIndex = -1;
                                    switch (opcodes[func.Data[i]].Arguments[a])
                                    {
                                        case InstructionArg.XRefV:
                                            {
                                                byte[] b = { func.Data[i + 2], func.Data[i + 3] };
                                                if (archive.XData.Endianness == Endianness.Big)
                                                    b = b.Reverse().ToArray();
                                                xrefIndex = BitConverter.ToUInt16(b);
                                                break;
                                            }
                                        case InstructionArg.XRefZ:
                                            xrefIndex = func.Data[i + 1];
                                            break;
                                        case InstructionArg.XRefX:
                                            xrefIndex = func.Data[i + 2];
                                            break;
                                        case InstructionArg.XRefY:
                                            xrefIndex = func.Data[i + 3];
                                            break;
                                        case InstructionArg.XRefE:
                                            {
                                                byte[] b = { func.Data[i + 6], func.Data[i + 7] };
                                                if (archive.XData.Endianness == Endianness.Big)
                                                    b = b.Reverse().ToArray();
                                                xrefIndex = BitConverter.ToUInt16(b);
                                                break;
                                            }
                                        case InstructionArg.XRefA:
                                            xrefIndex = func.Data[i + 5];
                                            break;
                                        case InstructionArg.XRefB:
                                            xrefIndex = func.Data[i + 6];
                                            break;
                                        case InstructionArg.XRefC:
                                            xrefIndex = func.Data[i + 7];
                                            break;
                                    }

                                    if (xrefIndex < 0 || xrefIndex >= module.XRef.Count)
                                        continue;

                                    if (module.XRef[xrefIndex] == searchHash)
                                        scripts.Add(module.Name + "." + func.NameWithoutType() + ": " + (i / 4));
                                }
                            }
                        }
                    }
                }
            }

            SearchResultForm results = new SearchResultForm(scripts.ToArray());
            results.Text = "Search Results - " + (hashes.ContainsKey(searchHash) ? hashes[searchHash] : searchHash.ToString("X8"));
            results.Show();
        }

        private void SearchForHashRtDL(string searchString)
        {
            List<string> scripts = new List<string>();
            foreach (var ctx in archives)
            {
                if (ctx.ArchiveRtDL == null)
                    continue;

                ArchiveRtDL archive = ctx.ArchiveRtDL;
                Opcode[] opcodes = MintVersions.Versions[RTDL_VERSION];
                foreach (var module in archive.Modules)
                {
                    if (!module.XRef.Contains(searchString))
                        continue;

                    for (int c = 0; c < module.Objects.Count; c++)
                    {
                        MintObject obj = module[c];

                        for (int f = 0; f < obj.Functions.Count; f++)
                        {
                            MintFunction func = obj.Functions[f];
                            for (int i = 0; i < func.Data.Length; i += 4)
                            {
                                if (func.Data[i] >= opcodes.Length)
                                    continue;
                                if (opcodes[func.Data[i]].Arguments == null)
                                    continue;
                                for (int a = 0; a < opcodes[func.Data[i]].Arguments.Length; a++)
                                {
                                    int xrefIndex = -1;
                                    switch (opcodes[func.Data[i]].Arguments[a])
                                    {
                                        case InstructionArg.XRefV:
                                            {
                                                byte[] b = { func.Data[i + 2], func.Data[i + 3] };
                                                if (archive.XData.Endianness == Endianness.Big)
                                                    b = b.Reverse().ToArray();
                                                xrefIndex = BitConverter.ToUInt16(b);
                                                break;
                                            }
                                        case InstructionArg.XRefZ:
                                            xrefIndex = func.Data[i + 1];
                                            break;
                                        case InstructionArg.XRefX:
                                            xrefIndex = func.Data[i + 2];
                                            break;
                                        case InstructionArg.XRefY:
                                            xrefIndex = func.Data[i + 3];
                                            break;
                                        case InstructionArg.XRefE:
                                            {
                                                byte[] b = { func.Data[i + 6], func.Data[i + 7] };
                                                if (archive.XData.Endianness == Endianness.Big)
                                                    b = b.Reverse().ToArray();
                                                xrefIndex = BitConverter.ToUInt16(b);
                                                break;
                                            }
                                        case InstructionArg.XRefA:
                                            xrefIndex = func.Data[i + 5];
                                            break;
                                        case InstructionArg.XRefB:
                                            xrefIndex = func.Data[i + 6];
                                            break;
                                        case InstructionArg.XRefC:
                                            xrefIndex = func.Data[i + 7];
                                            break;
                                    }

                                    if (xrefIndex < 0 || xrefIndex >= module.XRef.Count)
                                        continue;

                                    if (module.XRef[xrefIndex] == searchString)
                                        scripts.Add(module.Name + "." + func.NameWithoutType() + ": " + (i / 4));
                                }
                            }
                        }
                    }
                }
            }

            SearchResultForm results = new SearchResultForm(scripts.ToArray());
            results.Text = "Search Results - " + searchString;
            results.Show();
        }

        private void arcTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is FunctionTreeNode)
            {
                FunctionTreeNode fNode = e.Node as FunctionTreeNode;
                ObjectTreeNode oNode = fNode.GetObject();

                if (oNode.GetModule() is ModuleRtDLTreeNode)
                {
                    ModuleRtDLTreeNode mNode = oNode.GetModule() as ModuleRtDLTreeNode;
                    CreateEditor(mNode.GetArchive().Archive, mNode.Module, fNode.Function);
                }
                else
                {
                    ModuleTreeNode mNode = oNode.GetModule() as ModuleTreeNode;
                    CreateEditor(mNode.GetArchive().Archive, mNode.Module, oNode.Object, fNode.Function);
                }
            }
        }

        private void tabControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
                return;

            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                if (tabControl.GetTabRect(i).Contains(e.Location))
                {
                    CloseEditor(i, false);
                    break;
                }
            }
        }

        private void saveTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextEditorTab tab = tabControl.SelectedTab as TextEditorTab;
            string text = tab.TextBox.Text;
            text = FunctionUtil.StripComments(text);

            List<string> lines = text.Split('\n').ToList();

            if (tab.Module != null)
                tab.Function.Data = FunctionUtil.AssembleFunction(tab.Module, tab.Object, lines.Skip(1).ToList(), tab.Version);
            else if (tab.ModuleRtDL != null)
                tab.Function.Data = FunctionUtil.AssembleFunction(tab.ModuleRtDL, lines.Skip(1).ToList(), tab.Version);

            tab.SetDirty(false);
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseEditor(tabControl.SelectedIndex, false);
        }

        private void closeAllTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (tabControl.TabPages.Count > 0)
                CloseEditor(0, true);
        }

        private void addClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(arcTree.SelectedNode is ModuleTreeNode) && !(arcTree.SelectedNode is ModuleRtDLTreeNode))
                return;

            MintObject newObject = new MintObject();
            if (arcTree.SelectedNode is ModuleRtDLTreeNode)
            {
                ModuleRtDLTreeNode mNode = arcTree.SelectedNode as ModuleRtDLTreeNode;
                EditClassForm editor = new EditClassForm(newObject);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    mNode.Module.Objects.Add(newObject);
                    mNode.Open();
                }
            }
            else
            {
                ModuleTreeNode mNode = arcTree.SelectedNode as ModuleTreeNode;
                EditClassForm editor = new EditClassForm(newObject, mNode.GetArchive().Archive, mNode.Module, ref hashes);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    mNode.Module.Objects.Add(newObject);
                    mNode.Open();
                }
            }
        }

        private void deleteScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(arcTree.SelectedNode is ModuleTreeNode) && !(arcTree.SelectedNode is ModuleRtDLTreeNode))
                return;

            if (MessageBox.Show("Are you sure you want to delete this module?\nThis action cannot be undone.", "Mint Workshop", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (arcTree.SelectedNode is ModuleTreeNode)
                {
                    ModuleTreeNode node = arcTree.SelectedNode as ModuleTreeNode;

                    Archive archive = node.GetArchive().Archive;
                    archive.Modules.Remove(node.Module);
                }
                else if (arcTree.SelectedNode is ModuleRtDLTreeNode)
                {
                    ModuleRtDLTreeNode node = arcTree.SelectedNode as ModuleRtDLTreeNode;

                    ArchiveRtDL archive = node.GetArchive().Archive;
                    archive.Modules.Remove(node.Module);
                }

                arcTree.SelectedNode.Remove();
            }
        }

        private void editClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            string parentScript = arcTree.SelectedNode.Parent.FullPath;
            int index = arcTree.SelectedNode.Index;
            EditClassForm edit = new EditClassForm(archive.Scripts[parentScript].Classes[index], ref hashes);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                archive.Scripts[parentScript].Classes[index].SetName(edit.ClassName);
                archive.Scripts[parentScript].Classes[index].Flags = edit.ClassFlags;
                archive.Scripts[parentScript].Classes[index].ClassImpl = edit.ClassImpl;
                archive.Scripts[parentScript].Classes[index].Extends = edit.ClassExt;

                string[] pSplit = archive.Scripts[parentScript].Name.Split('.');
                arcTree.SelectedNode.Text = edit.ClassName.Replace(string.Join(".", pSplit.Take(pSplit.Length - 1)), "");
            }
            */
        }

        private void deleteClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ObjectTreeNode)
            {
                if (MessageBox.Show("Are you sure you want to delete this class?\nThis action cannot be undone.", "Mint Workshop", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ObjectTreeNode node = arcTree.SelectedNode as ObjectTreeNode;

                    TreeNode moduleNode = node.GetModule();
                    if (moduleNode is ModuleTreeNode)
                        (moduleNode as ModuleTreeNode).Module.Objects.Remove(node.Object);
                    else if (moduleNode is ModuleRtDLTreeNode)
                        (moduleNode as ModuleRtDLTreeNode).Module.Objects.Remove(node.Object);

                    arcTree.SelectedNode.Remove();
                }
            }
        }

        private void addVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(arcTree.SelectedNode is ObjectTreeNode))
                return;

            ObjectTreeNode node = arcTree.SelectedNode as ObjectTreeNode;
            ModuleFormat fmt = ModuleFormat.RtDL;
            if (node.GetModule() is ModuleTreeNode)
                fmt = (node.GetModule() as ModuleTreeNode).Module.Format;

            MintVariable newVariable = new MintVariable("int", "newVariable");
            EditVariableForm editor = new EditVariableForm(newVariable, fmt);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                node.Object.Variables.Add(newVariable);
                node.Open();
            }
        }

        private void addFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(arcTree.SelectedNode is ObjectTreeNode))
                return;

            ObjectTreeNode node = arcTree.SelectedNode as ObjectTreeNode;
            MintFunction newFunction = new MintFunction("void newFunction()");

            byte[] version = { 0, 2, 0, 0 };
            ModuleFormat fmt = ModuleFormat.RtDL;

            if (node.GetModule() is ModuleTreeNode)
            {
                ModuleTreeNode mNode = node.GetModule() as ModuleTreeNode;
                fmt = mNode.Module.Format;

                newFunction.Data = FunctionUtil.AssembleFunction(mNode.Module, node.Object, new List<string>() { "fenter 1, 0, 0", "fleave r0" }, version);
            }
            else
            {
                ModuleRtDLTreeNode mNode = node.GetModule() as ModuleRtDLTreeNode;
                newFunction.Data = FunctionUtil.AssembleFunction(mNode.Module, new List<string>() { "fenter 1, 0, 0", "fleave r0" }, version);
            }

            EditFunctionForm editor = new EditFunctionForm(newFunction, fmt);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                node.Object.Functions.Add(newFunction);
                node.Open();
            }
        }

        private void addConstantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(arcTree.SelectedNode is ObjectTreeNode))
                return;

            ObjectTreeNode node = arcTree.SelectedNode as ObjectTreeNode;
            ModuleFormat fmt = ModuleFormat.RtDL;
            if (node.GetModule() is ModuleTreeNode)
                fmt = (node.GetModule() as ModuleTreeNode).Module.Format;
            else
                return;

            MintEnum newEnum = new MintEnum("NewEnum", 0);
            EditEnumForm editor = new EditEnumForm(newEnum, fmt);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                node.Object.Enums.Add(newEnum);
                node.Open();
            }
        }

        private void editObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ModuleTreeNode)
            {
                EditModuleForm editor = new EditModuleForm((arcTree.SelectedNode as ModuleTreeNode).Module, ref hashes);
                editor.ShowDialog();
                (arcTree.SelectedNode as ModuleTreeNode).Update();
            }
            else if (arcTree.SelectedNode is ModuleRtDLTreeNode)
            {
                EditModuleForm editor = new EditModuleForm((arcTree.SelectedNode as ModuleRtDLTreeNode).Module);
                editor.ShowDialog();
                (arcTree.SelectedNode as ModuleRtDLTreeNode).Update();
            }
            else if (arcTree.SelectedNode is ObjectTreeNode)
            {
                ObjectTreeNode node = arcTree.SelectedNode as ObjectTreeNode;
                TreeNode module = node.GetModule();
                if (module is ModuleRtDLTreeNode)
                {
                    EditClassForm editor = new EditClassForm(node.Object);
                    editor.ShowDialog();
                }
                else if (module is ModuleTreeNode)
                {
                    ModuleTreeNode mNode = module as ModuleTreeNode;
                    EditClassForm editor = new EditClassForm(node.Object, mNode.GetArchive().Archive, mNode.Module, ref hashes);
                    editor.ShowDialog();
                }

                (arcTree.SelectedNode as ObjectTreeNode).Update();
            }
            else if (arcTree.SelectedNode is VariableTreeNode)
            {
                VariableTreeNode node = arcTree.SelectedNode as VariableTreeNode;

                ModuleFormat fmt = ModuleFormat.RtDL;
                var m = node.GetObject().GetModule();
                if (m is ModuleTreeNode)
                    fmt = (m as ModuleTreeNode).Module.Format;

                EditVariableForm editor = new EditVariableForm(node.Variable, fmt);
                editor.ShowDialog();

                (arcTree.SelectedNode as VariableTreeNode).Update();
            }
            else if (arcTree.SelectedNode is FunctionTreeNode)
            {
                FunctionTreeNode node = arcTree.SelectedNode as FunctionTreeNode;

                ModuleFormat fmt = ModuleFormat.RtDL;
                var m = node.GetObject().GetModule();
                if (m is ModuleTreeNode)
                    fmt = (m as ModuleTreeNode).Module.Format;

                EditFunctionForm editor = new EditFunctionForm(node.Function, fmt);
                editor.ShowDialog();

                (arcTree.SelectedNode as FunctionTreeNode).Update();
            }
            else if (arcTree.SelectedNode is EnumTreeNode)
            {
                EnumTreeNode node = arcTree.SelectedNode as EnumTreeNode;

                ModuleFormat fmt = ModuleFormat.RtDL;
                var m = node.GetObject().GetModule();
                if (m is ModuleTreeNode)
                    fmt = (m as ModuleTreeNode).Module.Format;

                EditEnumForm editor = new EditEnumForm(node.Enum, fmt);
                editor.ShowDialog();

                (arcTree.SelectedNode as EnumTreeNode).Update();
            }
        }

        private void deleteObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // sure, why not, this kinda sucks
            string type;
            if (arcTree.SelectedNode is VariableTreeNode)
                type = "variable";
            else if (arcTree.SelectedNode is FunctionTreeNode)
                type = "function";
            else if (arcTree.SelectedNode is EnumTreeNode)
                type = "enum";
            else
                return;

            if (MessageBox.Show($"Are you sure you want to delete this {type}?\nThis action cannot be undone.", "Mint Workshop", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (arcTree.SelectedNode is VariableTreeNode)
                {
                    VariableTreeNode node = arcTree.SelectedNode as VariableTreeNode;
                    node.GetObject().Object.Variables.Remove(node.Variable);
                }
                else if (arcTree.SelectedNode is FunctionTreeNode)
                {
                    FunctionTreeNode node = arcTree.SelectedNode as FunctionTreeNode;
                    for (int i = 0; i < tabControl.TabPages.Count; i++)
                    {
                        TabPage page = tabControl.TabPages[i];
                        if (page is TextEditorTab && (page as TextEditorTab).Function == node.Function)
                        {
                            page.Dispose();
                            break;
                        }
                    }

                    node.GetObject().Object.Functions.Remove(node.Function);
                }
                else if (arcTree.SelectedNode is EnumTreeNode)
                {
                    EnumTreeNode node = arcTree.SelectedNode as EnumTreeNode;
                    node.GetObject().Object.Enums.Remove(node.Enum);
                }

                arcTree.SelectedNode.Remove();
            }
        }

        private void findUsesOfObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ObjectTreeNode)
            {
                ObjectTreeNode node = arcTree.SelectedNode as ObjectTreeNode;
                MintObject obj = node.Object;
                if (node.GetModule() is ModuleRtDLTreeNode)
                    SearchForHashRtDL(obj.Name);
                else
                    SearchForHash(Crc32C.CalculateInv(obj.Name));
            }
            else if (arcTree.SelectedNode is VariableTreeNode)
            {
                VariableTreeNode node = arcTree.SelectedNode as VariableTreeNode;
                MintObject obj = node.GetObject().Object;
                if (node.GetObject().GetModule() is ModuleRtDLTreeNode)
                    SearchForHashRtDL($"{obj.Name}.{node.Variable.Name}");
                else
                    SearchForHash(Crc32C.CalculateInv($"{obj.Name}.{node.Variable.Name}"));
            }
            else if (arcTree.SelectedNode is FunctionTreeNode)
            {
                FunctionTreeNode node = arcTree.SelectedNode as FunctionTreeNode;
                MintObject obj = node.GetObject().Object;
                if (node.GetObject().GetModule() is ModuleRtDLTreeNode)
                    SearchForHashRtDL($"{obj.Name}.{node.Function.NameWithoutType()}");
                else
                    SearchForHash(Crc32C.CalculateInv($"{obj.Name}.{node.Function.NameWithoutType()}"));
            }
        }

        private void copyFullNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ObjectTreeNode)
            {
                MintObject obj = (arcTree.SelectedNode as ObjectTreeNode).Object;
                Clipboard.SetText(obj.Name);
            }
            else if (arcTree.SelectedNode is VariableTreeNode)
            {
                VariableTreeNode node = arcTree.SelectedNode as VariableTreeNode;
                MintObject obj = node.GetObject().Object;
                Clipboard.SetText($"{obj.Name}.{node.Variable.Name}");
            }
            else if (arcTree.SelectedNode is FunctionTreeNode)
            {
                FunctionTreeNode node = arcTree.SelectedNode as FunctionTreeNode;
                MintObject obj = node.GetObject().Object;
                Clipboard.SetText($"{obj.Name}.{node.Function.NameWithoutType()}");
            }
        }

        private void exportScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string modName = arcTree.SelectedNode is ModuleRtDLTreeNode
                ? (arcTree.SelectedNode as ModuleRtDLTreeNode).Name
                : (arcTree.SelectedNode as ModuleTreeNode).Name;

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Mint Script Source Files|*.mints|Mint Script Binary Files|*.bin";
            save.FileName = modName;
            save.AddExtension = true;
            if (save.ShowDialog() == DialogResult.OK)
            {
                if (save.FilterIndex == 1)
                {
                    string disasm;
                    if (arcTree.SelectedNode is ModuleRtDLTreeNode)
                    {
                        ModuleRtDLTreeNode node = arcTree.SelectedNode as ModuleRtDLTreeNode;
                        disasm = FunctionUtil.Disassemble(node.Module);
                    }
                    else
                    {
                        ModuleTreeNode node = arcTree.SelectedNode as ModuleTreeNode;
                        disasm = FunctionUtil.Disassemble(node.Module, node.GetArchive().Archive.Version, ref hashes);
                    }
                    File.WriteAllText(save.FileName, disasm);
                }
                else
                {
                    using (EndianBinaryWriter writer = new EndianBinaryWriter(new FileStream(save.FileName, FileMode.Create, FileAccess.Write)))
                    {
                        if (arcTree.SelectedNode is ModuleRtDLTreeNode)
                            (arcTree.SelectedNode as ModuleRtDLTreeNode).Module.Write(writer);
                        else
                            (arcTree.SelectedNode as ModuleTreeNode).Module.Write(writer);
                    }
                }
                MessageBox.Show($"Exported Mint script to\n{save.FileName}", "Mint Workshop", MessageBoxButtons.OK);
            }
        }

        private Module OpenModule(string path, Archive archive)
        {
            Module mod;

            if (path.EndsWith(".bin"))
            {
                using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                {
                    mod = new Module();
                    mod.Format = archive.GetModuleFormat();
                    mod.Read(reader);
                }
            }
            else
            {
                string text = FunctionUtil.StripComments(File.ReadAllText(path));

                mod = FunctionUtil.Assemble(text.Split('\n'), archive.Version);
                mod.Format = archive.GetModuleFormat();
            }

            mod.XData.Version = archive.XData.Version;
            mod.XData.Endianness = archive.XData.Endianness;
            mod.XData.Unknown_0xC = archive.XData.Unknown_0xC;

            return mod;
        }

        private ModuleRtDL OpenModuleRtDL(string path, ArchiveRtDL archive)
        {
            ModuleRtDL mod;

            if (path.EndsWith(".bin"))
            {
                using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                {
                    mod = new ModuleRtDL();
                    mod.Read(reader);
                }
            }
            else
            {
                string text = FunctionUtil.StripComments(File.ReadAllText(path));

                mod = FunctionUtil.AssembleRtDL(text.Split('\n'));
            }

            mod.XData.Version = archive.XData.Version;
            mod.XData.Endianness = archive.XData.Endianness;
            mod.XData.Unknown_0xC = archive.XData.Unknown_0xC;

            return mod;
        }

        private void replaceScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string moduleName = arcTree.SelectedNode is ModuleRtDLTreeNode
                ? (arcTree.SelectedNode as ModuleRtDLTreeNode).Name
                : (arcTree.SelectedNode as ModuleTreeNode).Name;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Valid Files|*.mints;*.bin|Mint Script Source Files|*.mints|Mint Script Binary Files|*.bin";
            open.FileName = moduleName;
            open.CheckFileExists = true;
            open.AddExtension = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                if (arcTree.SelectedNode is ModuleRtDLTreeNode)
                {
                    ArchiveRtDL arc = (arcTree.SelectedNode as ModuleRtDLTreeNode).GetArchive().Archive;

                    ModuleRtDL newModule = OpenModuleRtDL(open.FileName, arc);
                    if (newModule.Name != moduleName)
                    {
                        MessageBox.Show($"Module name \"{newModule.Name}\" does not match!");
                        return;
                    }

                    int idx = arc.Modules.FindIndex(x => x.Name == newModule.Name);
                    arc.Modules[idx] = newModule;
                }
                else
                {
                    Archive arc = (arcTree.SelectedNode as ModuleTreeNode).GetArchive().Archive;

                    Module newModule = OpenModule(open.FileName, arc);
                    if (newModule.Name != moduleName)
                    {
                        MessageBox.Show($"Module name \"{newModule.Name}\" does not match!");
                        return;
                    }

                    int idx = arc.Modules.FindIndex(x => x.Name == newModule.Name);
                    arc.Modules[idx] = newModule;
                }

                (arcTree.SelectedNode as DynamicTreeNode).Open();
            }
        }

        private void editXRefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ModuleRtDLTreeNode)
            {
                EditXRefRtDLForm edit = new EditXRefRtDLForm((arcTree.SelectedNode as ModuleRtDLTreeNode).Module);
                edit.ShowDialog();
            }
            else
            {
                EditXRefForm edit = new EditXRefForm((arcTree.SelectedNode as ModuleTreeNode).Module, ref hashes);
                edit.ShowDialog();
            }
        }

        private void optimizeScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ModuleRtDLTreeNode)
            {
                ModuleRtDLTreeNode node = arcTree.SelectedNode as ModuleRtDLTreeNode;
                ArchiveRtDL archive = node.GetArchive().Archive;

                ModuleRtDL newModule = FunctionUtil.AssembleRtDL(FunctionUtil.Disassemble(node.Module).Split("\n"));
                for (int i = 0; i < archive.Modules.Count; i++)
                {
                    if (archive.Modules[i].Name == newModule.Name)
                    {
                        archive.Modules[i] = newModule;
                        break;
                    }
                }

                node.Module = newModule;
                node.Update();
                node.Open();

                MessageBox.Show($"Successfully optimized {newModule.Name}", "Mint Workshop", MessageBoxButtons.OK);
            }
            else
            {
                ModuleTreeNode node = arcTree.SelectedNode as ModuleTreeNode;
                Archive archive = node.GetArchive().Archive;

                Module newModule = FunctionUtil.Assemble(FunctionUtil.Disassemble(node.Module, archive.Version, ref hashes).Split("\n"), archive.Version);
                for (int i = 0; i < archive.Modules.Count; i++)
                {
                    if (archive.Modules[i].Name == newModule.Name)
                    {
                        archive.Modules[i] = newModule;
                        break;
                    }
                }

                node.Module = newModule;
                node.Update();
                node.Open();

                MessageBox.Show($"Successfully optimized {newModule.Name}", "Mint Workshop", MessageBoxButtons.OK);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigForm configForm = new ConfigForm(Config);
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                Config = configForm.Config;
                Config.Save(exeDir + "\\Config.xml");
            }
        }

        private void parseAsFloatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox box = tabControl.SelectedTab.Controls[0] as RichTextBox;
            string text = box.SelectedText;
            if (text.StartsWith("0x"))
            {
                text = text.Remove(0, 2);
                if (int.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out int val))
                {
                    box.SelectedText = BitConverter.ToSingle(BitConverter.GetBytes(val), 0).ToString() + "f";
                }
                else
                    MessageBox.Show("Error: Could not convert selected text to float.", "Mint Workshop", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("Error: Selected text is not hexadecimal.", "Mint Workshop", MessageBoxButtons.OK);
        }

        private void convertToDecimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox box = tabControl.SelectedTab.Controls[0] as RichTextBox;
            string text = box.SelectedText;
            if (text.StartsWith("0x"))
            {
                text = text.Remove(0, 2);
                if (int.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out int val))
                {
                    box.SelectedText = val.ToString();
                }
                else
                    MessageBox.Show("Error: Could not convert selected text to decimal.", "Mint Workshop", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("Error: Selected text is not hexadecimal.", "Mint Workshop", MessageBoxButtons.OK);
        }

        private void reloadHashesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadHashes();
        }

        private void dumpHashesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is not ArchiveTreeNode)
            {
                MessageBox.Show("Hash dumping is not available for this Archive.", "MintWorkshop", MessageBoxButtons.OK);
                return;
            }

            ArchiveContext ctx = archives[arcTree.SelectedNode.Index];

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Text Files|*.txt";
            save.AddExtension = true;
            save.DefaultExt = ".txt";
            save.FileName = $"{Path.GetFileNameWithoutExtension(ctx.Path)}.txt";
            if (save.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(save.FileName, FileMode.Create, FileAccess.Write)))
                {
                    for (int i = 0; i < ctx.Archive.Modules.Count; i++)
                    {
                        var mod = ctx.Archive.Modules[i];
                        for (int o = 0; o < mod.Objects.Count; o++)
                        {
                            var obj = mod.Objects[o];
                            writer.WriteLine(obj.Name);
                            for (int j = 0; j < obj.Variables.Count; j++)
                                writer.WriteLine(obj.Name + "." + obj.Variables[j].Name);
                            for (int j = 0; j < obj.Functions.Count; j++)
                                writer.WriteLine(obj.Name + "." + obj.Functions[j].NameWithoutType());
                        }
                    }
                }
                MessageBox.Show($"Exported hashes to\n{save.FileName}", "Mint Workshop", MessageBoxButtons.OK);
            }
        }

        private void searchForHashUsageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hashSelector.ShowWindow(() =>
            {
                if (archives.Any(x => x.Archive != null))
                    SearchForHash(Crc32C.CalculateInv(hashSelector.SelectedHash));
                if (archives.Any(x => x.ArchiveRtDL != null))
                    SearchForHashRtDL(hashSelector.SelectedHash);
            });
        }

        private void arcTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is DynamicTreeNode)
                (e.Node as DynamicTreeNode).Open();
        }

        private void arcTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node is DynamicTreeNode)
                (e.Node as DynamicTreeNode).Close();
        }

        void BuildArchive(ArchiveContext ctx)
        {
            if (ctx.IsCompressed
                && MessageBox.Show("Build with LZ77 compression?\nThis is required for 3DS and Wii Mint.", "MintWorkshop", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MemoryStream stream = new MemoryStream();
                EndianBinaryWriter writer = new EndianBinaryWriter(stream);

                if (ctx.ArchiveRtDL != null)
                    ctx.ArchiveRtDL.Write(writer);
                else
                    ctx.Archive.Write(writer);

                writer.BaseStream.Seek(0, SeekOrigin.End);
                byte[] buffer = stream.GetBuffer().Take((int)writer.BaseStream.Position).ToArray();
                unsafe
                {
                    fixed (byte* b = &buffer[0])
                    {
                        using (FileStream file = new FileStream(ctx.Path, FileMode.Create, FileAccess.Write))
                            Compressor.Compact(CompressionType.ExtendedLZ77, new VoidPtr { address = b }, buffer.Length, file, new RawDataNode { _mainForm = Program.MainForm, Name = "Mint Archive" });
                    }
                }

                writer.Dispose();
                stream.Dispose();
            }
            else
            {
                if (ctx.ArchiveRtDL != null)
                {
                    using (EndianBinaryWriter writer = new EndianBinaryWriter(new FileStream(ctx.Path, FileMode.Create, FileAccess.Write)))
                        ctx.ArchiveRtDL.Write(writer);
                }
                else
                {
                    using (EndianBinaryWriter writer = new EndianBinaryWriter(new FileStream(ctx.Path, FileMode.Create, FileAccess.Write)))
                        ctx.Archive.Write(writer);
                }
            }
        }

        private void buildMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ArchiveTreeNode || arcTree.SelectedNode is ArchiveRtDLTreeNode)
                BuildArchive(archives[arcTree.SelectedNode.Index]);
        }

        private void buildAsMenuItem_Click(object sender, EventArgs e)
        {
            if (!(arcTree.SelectedNode is ArchiveTreeNode) && !(arcTree.SelectedNode is ArchiveRtDLTreeNode))
                return;

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Binary Files|*.bin;*.bin.cmp";
            save.AddExtension = true;
            save.DefaultExt = ".bin";
            if (save.ShowDialog() == DialogResult.OK)
            {
                ArchiveContext ctx = archives[arcTree.SelectedNode.Index];
                ctx.Path = save.FileName;

                arcTree.SelectedNode.Text = Path.GetFileName(ctx.Path);

                if (arcTree.SelectedNode is ArchiveTreeNode)
                    arcTree.SelectedNode.Text += $" ({ctx.Archive.GetVersionString()})";
                else if (arcTree.SelectedNode is ArchiveRtDLTreeNode)
                    arcTree.SelectedNode.Text += $" ({ctx.ArchiveRtDL.GetVersionString()})";

                BuildArchive(ctx);

                archives[arcTree.SelectedNode.Index] = ctx;
            }
        }

        private void closeArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ArchiveTreeNode)
            {
                TreeNode node = arcTree.SelectedNode;

                ArchiveContext ctx = archives[node.Index];

                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    var page = tabControl.TabPages[i];
                    if (page is TextEditorTab)
                    {
                        if ((page as TextEditorTab).Archive == ctx.Archive)
                        {
                            page.Dispose();
                            i--;
                        }
                    }
                }

                archives.RemoveAt(node.Index);

                arcTree.Nodes.RemoveAt(node.Index);
            }
            else if (arcTree.SelectedNode is ArchiveRtDLTreeNode)
            {
                TreeNode node = arcTree.SelectedNode;

                ArchiveContext ctx = archives[node.Index];

                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    var page = tabControl.TabPages[i];
                    if (page is TextEditorTab)
                    {
                        if ((page as TextEditorTab).ArchiveRtDL == ctx.ArchiveRtDL)
                        {
                            page.Dispose();
                            i--;
                        }
                    }
                }

                archives.RemoveAt(node.Index);

                arcTree.Nodes.RemoveAt(node.Index);
            }
        }

        private void editModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (arcTree.SelectedNode is ModuleTreeNode)
            {
                EditModuleForm editor = new EditModuleForm((arcTree.SelectedNode as ModuleTreeNode).Module, ref hashes);
                editor.ShowDialog();
                (arcTree.SelectedNode as ModuleTreeNode).Update();
            }
            else if (arcTree.SelectedNode is ModuleRtDLTreeNode)
            {
                EditModuleForm editor = new EditModuleForm((arcTree.SelectedNode as ModuleRtDLTreeNode).Module);
                editor.ShowDialog();
                (arcTree.SelectedNode as ModuleRtDLTreeNode).Update();
            }
        }

        private void arcTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is ModuleTreeNode || e.Node is ModuleRtDLTreeNode)
                e.Node.ContextMenuStrip = scriptCtxMenu;
            else if (e.Node is ObjectTreeNode)
                e.Node.ContextMenuStrip = classCtxMenu;
            else if (e.Node is VariableTreeNode || e.Node is FunctionTreeNode || e.Node is EnumTreeNode)
                e.Node.ContextMenuStrip = genericCtxMenu;
            else if (e.Node is NamespaceTreeNode)
                e.Node.ContextMenuStrip = namespaceMenuStrip;
        }

        private void importModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Valid Files|*.mints;*.bin|Mint Script Source Files|*.mints|Mint Script Binary Files|*.bin";
            open.Multiselect = true;
            open.CheckFileExists = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                List<string> modNames = new List<string>();

                if (arcTree.SelectedNode is ArchiveRtDLTreeNode)
                {
                    ArchiveRtDLTreeNode node = arcTree.SelectedNode as ArchiveRtDLTreeNode;
                    for (int i = 0; i < open.FileNames.Length; i++)
                    {
                        ModuleRtDL newModule = OpenModuleRtDL(open.FileNames[i], node.Archive);
                        modNames.Add(newModule.Name);

                        if (node.Archive.ModuleExists(newModule.Name))
                        {
                            int idx = node.Archive.Modules.FindIndex(x => x.Name == newModule.Name);
                            node.Archive.Modules[idx] = newModule;
                        }
                        else
                            node.Archive.Modules.Add(newModule);
                    }
                }
                else
                {
                    ArchiveTreeNode node = arcTree.SelectedNode as ArchiveTreeNode;
                    for (int i = 0; i < open.FileNames.Length; i++)
                    {
                        Module newModule = OpenModule(open.FileNames[i], node.Archive);
                        modNames.Add(newModule.Name);

                        if (node.Archive.ModuleExists(newModule.Name))
                        {
                            int idx = node.Archive.Modules.FindIndex(x => x.Name == newModule.Name);
                            node.Archive.Modules[idx] = newModule;
                        }
                        else
                            node.Archive.Modules.Add(newModule);
                    }
                }

                for (int i = 0; i < modNames.Count; i++)
                {
                    DynamicTreeNode node = arcTree.SelectedNode as DynamicTreeNode;
                    string[] nodes = modNames[i].Split('.');
                    for (int j = 1; j < nodes.Length; j++)
                    {
                        node.Open();
                        node.Expand();
                        node = node.Nodes[string.Join('.', nodes.Take(j))] as DynamicTreeNode;
                    }
                }
            }
        }

        void ExportAllNodes(NamespaceTreeNode node, string path)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                var child = node.Nodes[i];
                if (child is NamespaceTreeNode)
                {
                    bool expanded = node.IsExpanded;
                    (child as NamespaceTreeNode).Open();

                    ExportAllNodes(child as NamespaceTreeNode, path);

                    if (!expanded)
                        (child as NamespaceTreeNode).Close();
                }
                else if (child is ModuleTreeNode)
                {
                    var moduleNode = child as ModuleTreeNode;
                    File.WriteAllText(Path.Combine(path, moduleNode.Module.Name + ".mints"),
                        FunctionUtil.Disassemble(moduleNode.Module, moduleNode.GetArchive().Archive.Version, ref hashes));
                }
                else if (child is ModuleRtDLTreeNode)
                {
                    var moduleNode = child as ModuleRtDLTreeNode;
                    File.WriteAllText(Path.Combine(path, moduleNode.Module.Name + ".mints"),
                        FunctionUtil.Disassemble(moduleNode.Module));
                }
            }
        }

        private void exportAllModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NamespaceTreeNode node = arcTree.SelectedNode as NamespaceTreeNode;

            CommonOpenFileDialog open = new CommonOpenFileDialog();
            open.IsFolderPicker = true;
            open.Title = "Select output directory";
            if (open.ShowDialog() == CommonFileDialogResult.Ok)
            {
                bool expanded = node.IsExpanded;
                node.Open();
                ExportAllNodes(node, open.FileName);

                if (!expanded)
                    node.Close();

                MessageBox.Show($"Exported modules to \"{open.FileName}\"", "MintWorkshop", MessageBoxButtons.OK);
            }
        }

        private void viewPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewArchiveForm form = new ViewArchiveForm(archives[arcTree.SelectedNode.Index]);
            form.ShowDialog();
        }

        private void instructionDictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "https://docs.google.com/spreadsheets/d/1A_08ytw1oIBhqBzpkxDIU86RwmYAjG4DopogqCQllMo",
                UseShellExecute = true
            });
        }
    }
}
