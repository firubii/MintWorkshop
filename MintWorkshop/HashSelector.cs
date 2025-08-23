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
        public string SelectedHash { get => hashList.Text; }

        Action okCallback;

        public HashSelector()
        {
            InitializeComponent();

            CreateControl();
            CreateHandle();
            CreateGraphics();
        }

        public async Task UpdateHashList(string[] hashes)
        {
            await Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    hashList.Items.Clear();
                    hashList.Items.AddRange(hashes);
                });
            });
        }

        public void ShowWindow(Action? callback)
        {
            if (Visible)
                return;

            okCallback = callback;
            Show();
        }

        private void button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            okCallback?.Invoke();
            Hide();
        }

        private void HashSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
