using KirbyLib.Crypto;
using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Editors
{
    public partial class EditModuleForm : Form
    {
        Module _module;
        ModuleRtDL _moduleRtDL;

        public EditModuleForm(Module module, ref Dictionary<uint, string> hashes)
        {
            _module = module;

            InitializeComponent();

            name.Text = _module.Name;
            if (_module.UnkHash != 0xFFFFFFFF)
                unk.Text = hashes.ContainsKey(_module.UnkHash) ? hashes[_module.UnkHash] : _module.UnkHash.ToString("X8");

            unk.Visible = module.Format >= ModuleFormat.BasilKatFL;
            unkLabel.Visible = module.Format >= ModuleFormat.BasilKatFL;
        }

        public EditModuleForm(ModuleRtDL module)
        {
            _moduleRtDL = module;

            InitializeComponent();

            name.Text = _module.Name;
            unk.Visible = false;
            unkLabel.Visible = false;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (_module != null)
            {
                _module.Name = name.Text;
                if (string.IsNullOrWhiteSpace(unk.Text))
                    _module.UnkHash = 0xFFFFFFFF;
                else
                    _module.UnkHash =
                        uint.TryParse(unk.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint h)
                        ? h
                        : Crc32C.CalculateInv(unk.Text);
            }
            else if (_moduleRtDL != null)
            {
                _moduleRtDL.Name = name.Text;
            }

            Close();
        }
    }
}
