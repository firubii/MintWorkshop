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
    public partial class EditGenericForm : Form
    {
        public string StringName { get; private set; }

        public EditGenericForm(string name)
        {
            StringName = name;

            InitializeComponent();

            strName.Text = StringName;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            StringName = strName.Text;

            DialogResult = DialogResult.OK;
        }
    }
}
