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

        public EditFunctionForm(MintFunction baseFunction)
        {
            FunctionName = baseFunction.Name;
            FunctionFlags = baseFunction.Flags;

            InitializeComponent();
            funcFlags.Maximum = uint.MaxValue;

            funcName.Text = FunctionName;
            funcFlags.Value = FunctionFlags;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            FunctionName = funcName.Text;
            FunctionFlags = (uint)funcFlags.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
