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
    public partial class EditFunctionForm : Form
    {
        MintFunction _function;

        public EditFunctionForm(MintFunction function, ModuleFormat format)
        {
            _function = function;

            InitializeComponent();

            name.Text = _function.Name;
            flags.Value = _function.Flags;
            arguments.Value = _function.Arguments;
            registers.Value = _function.Registers;

            flags.Visible = format >= ModuleFormat.Mint;
            flagsLabel.Visible = format >= ModuleFormat.Mint;

            arguments.Visible = format >= ModuleFormat.BasilKatFL;
            argumentsLabel.Visible = format >= ModuleFormat.BasilKatFL;

            registers.Visible = format >= ModuleFormat.BasilKatFL;
            registersLabel.Visible = format >= ModuleFormat.BasilKatFL;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _function.Name = name.Text;
            _function.Flags = (uint)flags.Value;
            _function.Arguments = (uint)arguments.Value;
            _function.Registers = (uint)registers.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
