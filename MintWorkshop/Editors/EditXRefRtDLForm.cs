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
using KirbyLib.Crypto;
using KirbyLib.Mint;
using MintWorkshop.Mint;
using MintWorkshop.Util;

namespace MintWorkshop.Editors
{
    public partial class EditXRefRtDLForm : Form
    {
        ModuleRtDL module;

        public EditXRefRtDLForm(ModuleRtDL module)
        {
            this.module = module;

            InitializeComponent();

            for (int i = 0; i < module.XRef.Count; i++)
            {
                ListViewItem item = new ListViewItem(module.XRef[i]);
                xrefList.Items.Add(item);
            }
        }

        private void xrefList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListViewItem item = new ListViewItem(e.Label);
            xrefList.Items[e.Item] = item;
            xrefList.Invalidate();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            //Default to App since it always exists
            ListViewItem item = new ListViewItem("App");
            item.SubItems.Add("App");

            xrefList.Items.Add(item);
        }

        private void delButton_Click(object sender, EventArgs e)
        {
            if (xrefList.SelectedItems.Count > 0)
            {
                int index = xrefList.SelectedIndices[0];
                xrefList.Items.RemoveAt(index);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            module.XRef.Clear();
            for (int i = 0; i < xrefList.Items.Count; i++)
                module.XRef.Add(xrefList.Items[i].Text);

            DialogResult = DialogResult.OK;
        }

        private void xrefList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (xrefList.SelectedItems.Count > 0)
                xrefList.SelectedItems[0].BeginEdit();
        }
    }
}
