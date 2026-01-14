using FastColoredTextBoxNS;
using KirbyLib.Mint;
using MintWorkshop.Mint;
using MintWorkshop.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FCTBRange = FastColoredTextBoxNS.Range;

namespace MintWorkshop.Editors
{
    public class TextEditorTab : TabPage
    {
        static readonly TextStyle Error = new TextStyle(new SolidBrush(Color.Red), null, FontStyle.Regular);
        static readonly TextStyle Mneumonic = new TextStyle(new SolidBrush(Color.FromArgb(0, 0, 128)), null, FontStyle.Regular);
        static readonly TextStyle MneumonicExt = new TextStyle(new SolidBrush(Color.FromArgb(0, 128, 255)), null, FontStyle.Regular);
        static readonly TextStyle Register = new TextStyle(new SolidBrush(Color.FromArgb(155, 150, 50)), null, FontStyle.Regular);
        static readonly TextStyle Constant = new TextStyle(new SolidBrush(Color.FromArgb(0, 128, 54)), null, FontStyle.Regular);
        static readonly TextStyle String = new TextStyle(new SolidBrush(Color.FromArgb(100, 100, 100)), null, FontStyle.Regular);
        static readonly TextStyle Label = new TextStyle(new SolidBrush(Color.FromArgb(180, 0, 240)), null, FontStyle.Regular);
        static readonly TextStyle Comment = new TextStyle(new SolidBrush(Color.Green), null, FontStyle.Regular);

        static readonly Regex CommentRegex = new Regex(@"//.*$", RegexOptions.Multiline);
        static readonly Regex CommentMultiRegex = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);

        public byte[] Version;

        public bool IsLoading;
        public bool IsDirty;

        public Archive Archive;
        public ArchiveRtDL ArchiveRtDL;
        public Module Module;
        public ModuleRtDL ModuleRtDL;
        public MintObject Object;
        public MintFunction Function;

        public FastColoredTextBox TextBox;

        AutocompleteMenu autoComplete;

        public TextEditorTab(Archive archive, Module module, MintObject obj, MintFunction function, byte[] version)
        {
            Archive = archive;
            Module = module;
            Object = obj;
            Function = function;
            Version = version;

            Setup();
        }

        public TextEditorTab(ArchiveRtDL archive, ModuleRtDL module, MintFunction function, byte[] version)
        {
            ArchiveRtDL = archive;
            ModuleRtDL = module;
            Function = function;
            Version = version;

            Setup();
        }

        private void Setup()
        {
            TextBox = new FastColoredTextBox();
            TextBox.Language = Language.Custom;
            TextBox.BorderStyle = BorderStyle.FixedSingle;
            TextBox.Dock = DockStyle.Fill;
            TextBox.Font = new Font(new FontFamily("Courier New"), MainForm.Config.FontSize);
            TextBox.WordWrap = false;

            string[] flags = FunctionUtil.GetFlagLabels(Function.Flags, ref FlagLabels.FunctionFlags);
            for (int i = 0; i < flags.Length; i++)
                TextBox.AppendText(flags[i] + " ");
            TextBox.AppendText(Function.Name);
            TextBox.AppendText("\n\n");

            TextBox.TextChanged += TextBox_TextChanged;
            TextBox.KeyDown += TextBox_KeyDown;

            Controls.Add(TextBox);

            autoComplete = new AutocompleteMenu(TextBox);
            autoComplete.SearchPattern = @"[\w\.(),]";
            autoComplete.AllowTabKey = true;
            autoComplete.MinFragmentLength = 1;

            List<AutocompleteItem> acItems = new List<AutocompleteItem>();
            foreach (var op in MintVersions.Versions[Version])
                acItems.Add(new AutocompleteItem(op.Name));

            foreach (var h in Program.MainForm.GetHashes())
                acItems.Add(new AutocompleteItem(h.Value));

            autoComplete.Items.SetAutocompleteItems(acItems);
            autoComplete.Items.MaximumSize = new Size(1200, 300);
            autoComplete.Items.Width = 500;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Space | Keys.Control))
            {
                autoComplete.Show(true);
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoading)
                return;
                
            SetDirty(true);
            BeginUpdateTextColor(e.ChangedRange);
        }

        public void BeginUpdateTextColor(FCTBRange range = null)
        {
            if (range == null)
                range = TextBox.Range;

            new Thread(() => UpdateTextColorThreaded(range)).Start();
        }

        void UpdateTextColorThreaded(FCTBRange changedRange)
        {
            try //i dont care
            {
                List<string> labels = new List<string>();
                for (int i = 1; i < TextBox.Lines.Count; i++)
                {
                    string line = TextBox.Lines[i].Trim();
                    if (line.EndsWith(':'))
                        labels.Add(line[..(line.Length - 1)]);
                }

                Invoke(() =>
                {
                    changedRange.ClearStyle(
                        Error,
                        Mneumonic,
                        MneumonicExt,
                        Register,
                        Constant,
                        String,
                        Label,
                        Comment
                    );
                });

                var opcodes = MintVersions.Versions[Version];

                int start = TextBox.PlaceToPosition(changedRange.Start);
                int end = TextBox.PlaceToPosition(changedRange.End);

                for (int i = start; i < end;)
                {
                    if (i >= TextBox.Text.Length)
                        break;

                    int nextNewline = Math.Min(TextBox.Text.IndexOf('\n', i) + 1, TextBox.Text.Length);
                    if (nextNewline <= 0)
                        break;

                    string rawLine = TextBox.Text[i..nextNewline];
                    string line = rawLine.Trim();
                    if (line.EndsWith(':'))
                    {
                        Invoke(() => TextBox.GetRange(i, i + line.Length).SetStyle(Label));
                    }
                    else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                    {
                        string[] tokens = FunctionUtil.Tokenize(line);
                        if (tokens.Length > 0)
                        {
                            string opName = tokens[0];
                            Opcode? opcode = null;
                            if (opcodes.Any(x => x.Name == opName.ToLower()))
                                opcode = opcodes.First(x => x.Name == opName.ToLower());

                            int instIdx = rawLine.IndexOf(opName);
                            if (instIdx >= 0)
                            {
                                FCTBRange range = TextBox.GetRange(i + instIdx, i + instIdx + opName.Length);
                                if (opcode.HasValue)
                                    Invoke(() => range.SetStyle(opName.StartsWith('_') ? MneumonicExt : Mneumonic));
                                else
                                    Invoke(() => range.SetStyle(Error));

                                int tokenIdx = instIdx + opName.Length;
                                for (int t = 1; t < tokens.Length; t++)
                                {
                                    string token = tokens[t];
                                    int index = rawLine.IndexOf(token, tokenIdx);
                                    if (index < 0)
                                        continue;

                                    range = TextBox.GetRange(i + index, i + index + token.Length);

                                    Match match;
                                    if (labels.Contains(token))
                                    {
                                        Invoke(() => range.SetStyle(Label));
                                        goto loop;
                                    }
                                    match = MintRegex.String().Match(token);
                                    if (match.Success)
                                    {
                                        Invoke(() => range.SetStyle(String));
                                        goto loop;
                                    }
                                    match = MintRegex.Register().Match(token);
                                    if (match.Success && match.Value == token)
                                    {
                                        Invoke(() => range.SetStyle(Register));
                                        goto loop;
                                    }
                                    match = MintRegex.Value().Match(token);
                                    if (match.Success && match.Value == token)
                                    {
                                        Invoke(() => range.SetStyle(Constant));
                                        goto loop;
                                    }

                                loop:
                                    tokenIdx = index + token.Length;
                                }
                            }
                        }
                    }

                    i += rawLine.Length;
                }

                Invoke(() =>
                {
                    TextBox.Range.ClearStyle(Comment);

                    TextBox.Range.SetStyle(Comment, CommentRegex);
                    //TextBox.Range.SetStyle(Comment, CommentMultiRegex);

                    //TextBox.Range.SetFoldingMarkers(@"/\*", @"\*/");
                });
            }
            catch { }
        }

        public void SetContextMenuStrip(ContextMenuStrip ctxMenu)
        {
            TextBox.ContextMenuStrip = ctxMenu;
        }

        public void SetDirty(bool isDirty)
        {
            if (isDirty == IsDirty)
                return;

            IsDirty = isDirty;
            if (IsDirty)
                Text += " *";
            else
                Text = Text[..Text.IndexOf(" *")];
        }
    }
}
