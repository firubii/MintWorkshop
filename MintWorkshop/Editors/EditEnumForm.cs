using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MintWorkshop.Editors
{
    public partial class EditEnumForm : Form
    {
        MintEnum _enum;

        public EditEnumForm(MintEnum mintEnum, ModuleFormat format)
        {
            _enum = mintEnum;

            InitializeComponent();
            value.Maximum = uint.MaxValue;

            name.Text = _enum.Name;
            value.Value = _enum.Value;
            flags.Value = _enum.Flags;

            flags.Visible = format >= ModuleFormat.Basil;
            flagsLabel.Visible = format >= ModuleFormat.Basil;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _enum.Name = name.Text;
            _enum.Value = (int)value.Value;
            _enum.Flags = (uint)flags.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
