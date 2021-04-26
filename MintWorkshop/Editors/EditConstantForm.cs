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
    public partial class EditConstantForm : Form
    {
        public string ConstantName { get; private set; }
        public uint ConstantValue { get; private set; }

        public EditConstantForm(MintClass.MintConstant baseConstant)
        {
            ConstantName = baseConstant.Name;
            ConstantValue = baseConstant.Value;

            InitializeComponent();
            constValue.Maximum = uint.MaxValue;

            constName.Text = ConstantName;
            constValue.Value = ConstantValue;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ConstantName = constName.Text;
            ConstantValue = (uint)constValue.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
