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
    public partial class EditClassForm : Form
    {
        public string ClassName { get; private set; }
        public uint ClassFlags { get; private set; }
        public List<uint> ClassUnknowns { get; private set; }

        public EditClassForm(MintClass baseClass)
        {
            ClassName = baseClass.Name;
            ClassFlags = baseClass.Flags;
            ClassUnknowns = baseClass.UnknownList;

            InitializeComponent();
            classFlags.Maximum = uint.MaxValue;

            className.Text = ClassName;
            classFlags.Value = ClassFlags;
            classUnks.Text = string.Join(",", ClassUnknowns);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ClassName = className.Text;
            ClassFlags = (uint)classFlags.Value;
            List<uint> unkTemp = new List<uint>();
            if (classUnks.Text != "")
            {
                string[] unks = classUnks.Text.Split(',');
                for (int i = 0; i < unks.Length; i++)
                {
                    if (uint.TryParse(unks[i], out uint u))
                    {
                        unkTemp.Add(u);
                    }
                    else
                    {
                        MessageBox.Show("Error: Could not parse unknown values.", this.Text, MessageBoxButtons.OK);
                        return;
                    }
                }
            }
            ClassUnknowns = unkTemp;

            DialogResult = DialogResult.OK;
        }
    }
}
