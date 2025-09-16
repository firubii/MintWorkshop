using KirbyLib.Mint;
using MintWorkshop.Mint;
using MintWorkshop.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Editors
{
    public class TextEditorTab : TabPage
    {
        public byte[] Version;

        public bool IsLoading;
        public bool IsDirty;

        public Archive Archive;
        public ArchiveRtDL ArchiveRtDL;
        public Module Module;
        public ModuleRtDL ModuleRtDL;
        public MintObject Object;
        public MintFunction Function;

        public RichTextBox TextBox;

        bool isColoring;

        List<Range> comments = new List<Range>();

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
            TextBox = new RichTextBox();
            TextBox.BorderStyle = BorderStyle.FixedSingle;
            TextBox.Dock = DockStyle.Fill;
            TextBox.Font = new Font(new FontFamily("Courier New"), MainForm.Config.FontSize);
            TextBox.ScrollBars = RichTextBoxScrollBars.Both;
            TextBox.WordWrap = false;

            string[] flags = FunctionUtil.GetFlagLabels(Function.Flags, ref FlagLabels.FunctionFlags);
            for (int i = 0; i < flags.Length; i++)
                TextBox.AppendText(flags[i] + " ");
            TextBox.AppendText(Function.Name);
            TextBox.AppendText("\n\n");

            TextBox.TextChanged += TextBox_TextChanged;

            Controls.Add(TextBox);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading || isColoring) return;

            SetDirty(true);

            UpdateTextColor(TextBox.SelectionStart, 1);
        }

        public void UpdateTextColor(int start = -1, int lines = -1)
        {
            isColoring = true;

            /*
             * TODO: Find out a way to do this that doesn't break the ability to undo
             * 
             * I might have to rewrite how this class draws to do that but I don't want to do that right now
            */
            
            Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    int selStart = TextBox.SelectionStart;
                    int selLen = TextBox.SelectionLength;

                    DrawingControl.SuspendDrawing(TextBox);
                    TextBox.SuspendLayout();

                    List<string> labels = new List<string>();
                    for (int i = 1; i < TextBox.Lines.Length; i++)
                    {
                        string line = TextBox.Lines[i].Trim();
                        if (line.EndsWith(':'))
                            labels.Add(line[..(line.Length - 1)]);
                    }

                    var opcodes = MintVersions.Versions[Version];

                    int firstLine = TextBox.Text.IndexOf('\n') + 1;
                    start = Math.Max(start - 1, firstLine);
                    while (start > firstLine && TextBox.Text[start - 1] != '\n')
                        start--;

                    int end = TextBox.Text.Length - 1;
                    if (lines > 0)
                    {
                        end = start;
                        while (lines > 0 && end < TextBox.Text.Length - 1)
                        {
                            end++;
                            if (TextBox.Text[end] == '\n')
                                lines--;
                        }
                    }

                    TextBox.SelectionStart = start;
                    TextBox.SelectionLength = end - start;
                    TextBox.SelectionColor = Color.Black;

                    for (int i = start; i < end;)
                    {
                        if (TextBox.Text.IndexOf('\n', i) < 0)
                            break;

                        string rawLine = TextBox.Text[i..(TextBox.Text.IndexOf('\n', i) + 1)];
                        string line = rawLine.Trim();
                        if (line.EndsWith(':'))
                        {
                            TextBox.SelectionStart = i;
                            TextBox.SelectionLength = line.Length;
                            TextBox.SelectionColor = TextColors.LabelColor;
                        }
                        else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                        {
                            string[] tokens = FunctionUtil.Tokenize(line);
                            if (tokens.Length > 0)
                            {
                                string opcode = tokens[0];
                                int instIdx = rawLine.IndexOf(opcode);
                                if (instIdx >= 0)
                                {
                                    TextBox.SelectionStart = i + instIdx;
                                    TextBox.SelectionLength = opcode.Length;
                                    if (opcodes.Count(x => x.Name == opcode.ToLower()) > 0)
                                        TextBox.SelectionColor = opcode.StartsWith('_') ? TextColors.MneumonicExtColor : TextColors.MneumonicColor;
                                    else
                                        TextBox.SelectionColor = Color.Red;

                                    int tokenIdx = instIdx + opcode.Length;
                                    for (int t = 1; t < tokens.Length; t++)
                                    {
                                        string token = tokens[t];
                                        int index = rawLine.IndexOf(token, tokenIdx);
                                        if (index < 0)
                                            continue;

                                        TextBox.SelectionStart = i + index;
                                        TextBox.SelectionLength = token.Length;

                                        Match match;
                                        if (labels.Contains(token))
                                        {
                                            TextBox.SelectionColor = TextColors.LabelColor;
                                            goto loop;
                                        }
                                        match = MintRegex.String().Match(token);
                                        if (match.Success)
                                        {
                                            TextBox.SelectionColor = TextColors.StringColor;
                                            goto loop;
                                        }
                                        match = MintRegex.Register().Match(token);
                                        if (match.Success && match.Value == token)
                                        {
                                            TextBox.SelectionColor = TextColors.RegisterColor;
                                            goto loop;
                                        }
                                        match = MintRegex.Value().Match(token);
                                        if (match.Success && match.Value == token)
                                        {
                                            TextBox.SelectionColor = TextColors.ConstantColor;
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

                    for (int i = 0; i < comments.Count; i++)
                    {
                        Range range = comments[i];
                        TextBox.SelectionStart = range.Start.Value;
                        TextBox.SelectionLength = range.End.Value - range.Start.Value;
                        TextBox.SelectionColor = Color.Green;
                    }

                    // Restore selection at the end of the thread
                    TextBox.SelectionStart = selStart;
                    TextBox.SelectionLength = selLen;

                    TextBox.ResumeLayout();
                    DrawingControl.ResumeDrawing(TextBox);
                    TextBox.Invalidate();

                    isColoring = false;
                });
            });

            // Get all comments to store them for coloring, this is the most demanding operation
            Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    comments.Clear();

                    var cmtMatches = MintRegex.Comment().Matches(TextBox.Text);
                    for (int i = 0; i < cmtMatches.Count; i++)
                    {
                        var match = cmtMatches[i];
                        comments.Add(new Range(match.Index, match.Index + match.Length + 1));
                    }

                    var mltcmtMatches = MintRegex.MultilineComment().Matches(TextBox.Text);
                    for (int i = 0; i < mltcmtMatches.Count; i++)
                    {
                        var match = mltcmtMatches[i];
                        comments.Add(new Range(match.Index, match.Index + match.Length));
                    }
                });
            });
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
