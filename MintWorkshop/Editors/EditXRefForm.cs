using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MintWorkshop.Mint;
using MintWorkshop.Util;

namespace MintWorkshop.Editors
{
    public partial class EditXRefForm : Form
    {
        public List<byte[]> XRef;
        Dictionary<byte[], string> Hashes;

        public EditXRefForm(List<byte[]> xrefs, Dictionary<byte[], string> hashes)
        {
            XRef = xrefs;
            Hashes = hashes;

            InitializeComponent();

            for (int i = 0; i < XRef.Count; i++)
            {
                ListViewItem item = new ListViewItem($"{XRef[i][0]:X2}{XRef[i][1]:X2}{XRef[i][2]:X2}{XRef[i][3]:X2}");

                if (Hashes.ContainsKey(XRef[i]))
                {
                    item.SubItems.Add(Hashes[XRef[i]]);
                }
                else
                {
                    item.BackColor = Color.Red;
                    item.SubItems.Add("");
                }

                xrefList.Items.Add(item);
            }
        }

        private void xrefList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label)) return;

            byte[] hash;
            string str = e.Label;
            if (str.Length == 8)
            {
                if (uint.TryParse(str, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint h))
                {
                    hash = new byte[] { byte.Parse(string.Join("", str.Take(2)), NumberStyles.HexNumber),
                                byte.Parse(string.Join("", str.Skip(2).Take(2)), NumberStyles.HexNumber),
                                byte.Parse(string.Join("", str.Skip(4).Take(2)), NumberStyles.HexNumber),
                                byte.Parse(string.Join("", str.Skip(6).Take(2)), NumberStyles.HexNumber) };
                }
                else
                {
                    hash = HashCalculator.Calculate(str);
                }
            }
            else
            {
                hash = HashCalculator.Calculate(str);
            }
            XRef[e.Item] = hash;

            ListViewItem item = new ListViewItem($"{hash[0]:X2}{hash[1]:X2}{hash[2]:X2}{hash[3]:X2}");
            if (Hashes.ContainsKey(hash))
            {
                item.SubItems.Add(Hashes[hash]);
            }
            else
            {
                item.BackColor = Color.Red;
                item.SubItems.Add("");
            }
            xrefList.Items[e.Item] = item;
            xrefList.Invalidate();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            //Default to App since it always exists
            byte[] hash = HashCalculator.Calculate("App");
            XRef.Add(hash);

            ListViewItem item = new ListViewItem($"{hash[0]:X2}{hash[1]:X2}{hash[2]:X2}{hash[3]:X2}");
            item.SubItems.Add("App");

            xrefList.Items.Add(item);
        }

        private void delButton_Click(object sender, EventArgs e)
        {
            if (xrefList.SelectedItems.Count > 0)
            {
                int index = xrefList.SelectedIndices[0];
                XRef.RemoveAt(index);
                xrefList.Items.RemoveAt(index);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void EditXRefForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                DialogResult = DialogResult.Cancel;
        }
    }
}
