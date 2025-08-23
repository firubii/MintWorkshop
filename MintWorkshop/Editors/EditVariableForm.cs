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

namespace MintWorkshop.Editors
{
    public partial class EditVariableForm : Form
    {
        MintVariable _variable;

        public EditVariableForm(MintVariable variable, ModuleFormat format)
        {
            _variable = variable;

            InitializeComponent();
            flags.Maximum = uint.MaxValue;

            name.Text = _variable.Name;
            type.Text = _variable.Type;
            flags.Value = _variable.Flags;

            flags.Visible = format > ModuleFormat.RtDL;
            flagsLabel.Visible = format > ModuleFormat.RtDL;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _variable.Name = name.Text;
            _variable.Type = type.Text;
            _variable.Flags = (uint)flags.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
