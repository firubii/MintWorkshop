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
    public partial class HashSelector : Form
    {
        public string selectedHash;
        string[] hashes;

        public HashSelector(string[] hashNames)
        {
            hashes = hashNames;
            InitializeComponent();
            this.Text = "Hash Selector - Loading Hashes...";
        }

        private void HashSearch_Shown(object sender, EventArgs e)
        {
            Task.Run(() => Invoke((MethodInvoker)delegate
            {
                hashList.Items.AddRange(hashes);
                this.Text = "Hash Selector";
            }));
        }

        private void button_Click(object sender, EventArgs e)
        {
            selectedHash = hashList.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
