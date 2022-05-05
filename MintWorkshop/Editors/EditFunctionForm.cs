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
    public partial class EditFunctionForm : Form
    {
        public string FunctionName { get; private set; }
        public uint FunctionFlags { get; private set; }
        public uint FunctionArgs { get; private set; }
        public uint FunctionRegs { get; private set; }

        public EditFunctionForm(MintFunction baseFunction)
        {
            FunctionName = baseFunction.Name;
            FunctionFlags = baseFunction.Flags;
            FunctionArgs = baseFunction.Arguments;
            FunctionArgs = baseFunction.Registers;

            InitializeComponent();
            if (baseFunction.ParentClass.ParentScript.Version[0] < 2 && baseFunction.ParentClass.ParentScript.Version[1] < 1)
            {
                funcFlags.Visible = false;
                label2.Visible = false;
            }
            if (baseFunction.ParentClass.ParentScript.Version[0] < 7)
            {
                funcUnk1.Visible = false;
                label3.Visible = false;
                funcUnk2.Visible = false;
                label4.Visible = false;
            }

            funcFlags.Maximum = uint.MaxValue;

            funcName.Text = FunctionName;
            funcFlags.Value = FunctionFlags;
            funcUnk1.Value = FunctionArgs;
            funcUnk2.Value = FunctionRegs;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            FunctionName = funcName.Text;
            FunctionFlags = (uint)funcFlags.Value;
            FunctionArgs = (uint)funcUnk1.Value;
            FunctionRegs = (uint)funcUnk2.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
