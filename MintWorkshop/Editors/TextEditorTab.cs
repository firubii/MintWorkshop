using KirbyLib.Mint;
using MintWorkshop.Mint;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;

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

        static readonly Regex ErrorRegex = new Regex(@"^.+?\s", RegexOptions.Multiline);
        static readonly Regex RegisterRegex = new Regex(@"\b(r[0-9]+?)\b", RegexOptions.Multiline);
        static readonly Regex ConstantRegex = new Regex(@"\b((?:(?:0x[0-9A-Fa-f]+)|(?:-?[0-9]+(?:\.[0-9]+)?f?)))\b", RegexOptions.Multiline);
        static readonly Regex StringRegex = new Regex(@"\b(u?"".*?"")\b", RegexOptions.Multiline);
        static readonly Regex LabelRefRegex = new Regex(@"(\b\S+?\b)(?:(?=.+\b\1:)|(?<=\b\1:.+))", RegexOptions.Singleline);
        static readonly Regex LabelRegex = new Regex(@"(\S+?):", RegexOptions.Multiline);
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
        Regex mnemonicRegex;
        Regex mnemonicExRegex;
        bool isColoring;

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
            mnemonicRegex = new Regex(@$"\b({string.Join('|',MintVersions.Versions[Version].Select(x => x.Name).Where(x => !x.StartsWith('_')))})\s+");
            mnemonicExRegex = new Regex(@$"\b({string.Join('|', MintVersions.Versions[Version].Select(x => x.Name).Where(x => x.StartsWith('_')))})\s+");

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
            if (IsLoading || isColoring)
                return;
            
            SetDirty(true);

            UpdateTextColor(e);
        }

        public void UpdateTextColor(TextChangedEventArgs e)
        {
            isColoring = true;

            e.ChangedRange.ClearStyle(
                Error,
                Mneumonic,
                MneumonicExt,
                Register,
                Constant,
                String,
                Label,
                Comment
            );

            e.ChangedRange.ClearFoldingMarkers();

            //e.ChangedRange.SetStyle(Error, ErrorRegex);
            e.ChangedRange.SetStyle(Mneumonic, mnemonicRegex);
            e.ChangedRange.SetStyle(MneumonicExt, mnemonicExRegex);

            e.ChangedRange.SetStyle(Register, RegisterRegex);

            e.ChangedRange.SetStyle(Constant, ConstantRegex);
            e.ChangedRange.SetStyle(String, StringRegex);

            // Manual label coloring
            if (!e.ChangedRange.Text.Contains('\n'))
            {
                //Find all label references
                foreach (string part in e.ChangedRange.Text.Split(' '))
                {
                    if (TextBox.Text.Contains(part + ":"))
                        e.ChangedRange.SetStyle(Label, part.Replace(".", "\\."));
                }

                //Correct label references if this line is a label
                var match = LabelRegex.Match(e.ChangedRange.Text);
                if (match.Success)
                {
                    string label = match.Groups[1].Value;
                    TextBox.Range.SetStyle(Label, label);
                }
            }
            else
            {
                e.ChangedRange.SetStyle(Label, LabelRefRegex);
                e.ChangedRange.SetStyle(Label, LabelRegex);
            }

            e.ChangedRange.SetStyle(Comment, CommentRegex);
            e.ChangedRange.SetStyle(Comment, CommentMultiRegex);

            e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");

            isColoring = false;
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
