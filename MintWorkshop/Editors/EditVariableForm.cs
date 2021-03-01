using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MintWorkshop.Types;

namespace MintWorkshop.Editors
{
    public partial class EditVariableForm : Form
    {
        public string VariableName { get; private set; }
        public string VariableType { get; private set; }
        public uint VariableFlags { get; private set; }

        public EditVariableForm(MintVariable baseVariable)
        {
            VariableName = baseVariable.Name;
            VariableType = baseVariable.Type;
            VariableFlags = baseVariable.Flags;

            InitializeComponent();
            varFlags.Maximum = uint.MaxValue;

            varName.Text = VariableName;
            varType.Text = VariableType;
            varFlags.Value = VariableFlags;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            VariableName = varName.Text;
            VariableType = varType.Text;
            VariableFlags = (uint)varFlags.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
