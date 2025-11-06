using MintWorkshop.Mint;
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
    public partial class NewArchiveForm : Form
    {
        byte[][] versionList;

        public string ArcName;
        public byte[] Version;
        public bool LittleEndian;

        public NewArchiveForm()
        {
            InitializeComponent();

            versionList = MintVersions.Versions.Keys.ToArray();
            foreach (var key in versionList)
                version.Items.Add($"{key[0]}.{key[1]}.{key[2]}.{key[3]}");

            version.SelectedIndex = version.Items.Count - 1;
        }

        private void save_Click(object sender, EventArgs e)
        {
            ArcName = name.Text;
            Version = versionList[version.SelectedIndex];
            LittleEndian = isLittleEndian.Checked;

            DialogResult = DialogResult.OK;
        }
    }
}
