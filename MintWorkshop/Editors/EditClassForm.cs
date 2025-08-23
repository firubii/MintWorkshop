using KirbyLib.Crypto;
using KirbyLib.Mint;
using MintWorkshop.Mint;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace MintWorkshop.Editors
{
    public partial class EditClassForm : Form
    {
        MintObject _object;
        Archive _archive;
        Module _module;

        public EditClassForm(MintObject obj, Archive archive, Module module, ref Dictionary<uint, string> hashes)
        {
            _object = obj;
            _archive = archive;
            _module = module;

            InitializeComponent();

            name.Text = _object.Name;
            flags.Value = _object.Flags;

            for (int i = 0; i < _object.Implements.Count; i++)
            {
                implements.Text +=
                    hashes.ContainsKey(_object.Implements[i])
                    ? hashes[_object.Implements[i]]
                    : _object.Implements[i].ToString("X8");

                if (i < _object.Implements.Count - 1)
                    implements.Text += ", ";
            }

            var opcodes = MintVersions.Versions[archive.Version];
            for (int i = 0; i < _object.Extends.Count; i++)
            {
                byte[] e = _object.Extends[i];
                byte op = e[0];
                ushort v = BitConverter.ToUInt16(e, 2);

                if (op < opcodes.Length)
                {
                    if (opcodes[op].Name == "_xref")
                    {
                        uint hash = _module.XRef[v];
                        extends.Text +=
                            hashes.ContainsKey(hash)
                            ? hashes[hash]
                            : hash.ToString("X8");
                    }
                    else
                    {
                        extends.Text += FlagLabels.StdTypes.ContainsKey(v)
                            ? FlagLabels.StdTypes[v]
                            : v;
                    }

                    if (i < _object.Extends.Count - 1)
                        extends.Text += ", ";
                }
            }

            ModuleFormat format = archive.GetModuleFormat();
            flags.Visible = format > ModuleFormat.RtDL;
            flagsLabel.Visible = format > ModuleFormat.RtDL;

            implements.Visible = format >= ModuleFormat.Mint;
            implementsLabel.Visible = format >= ModuleFormat.Mint;

            extends.Visible = format >= ModuleFormat.BasilKatFL;
            extendsLabel.Visible = format >= ModuleFormat.BasilKatFL;
        }

        public EditClassForm(MintObject obj)
        {
            _object = obj;

            InitializeComponent();

            name.Text = _object.Name;

            flags.Visible = false;
            flagsLabel.Visible = false;
            implements.Visible = false;
            implementsLabel.Visible = false;
            extends.Visible = false;
            extendsLabel.Visible = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _object.Name = name.Text;

            if (_module != null)
            {
                _object.Flags = (uint)flags.Value;

                _object.Implements = new List<uint>();
                string[] xrefs = FunctionUtil.Tokenize(implements.Text);
                for (int i = 0; i < xrefs.Length; i++)
                {
                    _object.Implements.Add(
                        uint.TryParse(xrefs[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint h)
                        ? h
                        : Crc32C.CalculateInv(xrefs[i])
                    );
                }

                var opcodes = MintVersions.Versions[_archive.Version];
                _object.Extends = new List<byte[]>();
                string[] ext = FunctionUtil.Tokenize(extends.Text);
                for (int i = 0; i < ext.Length; i++)
                {
                    byte[] b = { 0xFF, 0xFF, 0xFF, 0xFF };
                    string token = ext[i];
                    if (ushort.TryParse(token, out ushort std))
                    {
                        b[0] = (byte)opcodes.ToList().FindIndex(x => x.Name == "_short");
                        Array.Copy(BitConverter.GetBytes(std), 0, b, 2, 2);
                    }
                    else if (FlagLabels.StdTypes.ContainsValue(token))
                    {
                        b[0] = (byte)opcodes.ToList().FindIndex(x => x.Name == "_short");

                        ushort v = FlagLabels.StdTypes.Keys.First(x => FlagLabels.StdTypes[x] == token);
                        Array.Copy(BitConverter.GetBytes(v), 0, b, 2, 2);
                    }
                    else
                    {
                        b[0] = (byte)opcodes.ToList().FindIndex(x => x.Name == "_xref");
                        
                        uint hash;
                        if (!uint.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hash))
                            hash = Crc32C.CalculateInv(token);

                        int v = _module.XRef.IndexOf(hash);
                        if (v < 0)
                        {
                            v = _module.XRef.Count;
                            _module.XRef.Add(hash);
                        }

                        Array.Copy(BitConverter.GetBytes(v), 0, b, 2, 2);
                    }
                }
            }

            DialogResult = DialogResult.OK;
        }
    }
}
