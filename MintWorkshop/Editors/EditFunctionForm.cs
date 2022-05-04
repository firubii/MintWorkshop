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
        public uint FunctionUnk1 { get; private set; }
        public uint FunctionUnk2 { get; private set; }

        public EditFunctionForm(MintFunction baseFunction)
        {
            FunctionName = baseFunction.Name;
            FunctionFlags = baseFunction.Flags;
            FunctionUnk1 = baseFunction.Unknown1;
            FunctionUnk1 = baseFunction.Unknown2;

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
            funcUnk1.Value = FunctionUnk1;
            funcUnk2.Value = FunctionUnk2;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            FunctionName = funcName.Text;
            FunctionFlags = (uint)funcFlags.Value;
            FunctionUnk1 = (uint)funcUnk1.Value;
            FunctionUnk2 = (uint)funcUnk2.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
