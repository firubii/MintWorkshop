using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop
{
    public partial class ConfigForm : Form
    {
        public Config Config;

        public ConfigForm(Config config)
        {
            Config = config;
            InitializeComponent();

            uppercase.Checked = Config.UppercaseMnemonics;
            optimize.Checked = Config.OptimizeOnBuild;
            fontSize.Value = (decimal)Config.FontSize;
        }

        private void restoreDefault_Click(object sender, EventArgs e)
        {
            uppercase.Checked = false;
            fontSize.Value = 9;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Config.UppercaseMnemonics = uppercase.Checked;
            Config.OptimizeOnBuild = optimize.Checked;
            Config.FontSize = (float)fontSize.Value;

            DialogResult = DialogResult.OK;
        }
    }
}
