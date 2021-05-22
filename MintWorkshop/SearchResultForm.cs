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
    public partial class SearchResultForm : Form
    {
        public SearchResultForm(string[] results)
        {
            InitializeComponent();
            for (int i = 0; i < results.Length; i++)
            {
                listView.Items.Add(results[i]);
            }
        }

        private void SearchResultForm_Resize(object sender, EventArgs e)
        {
            listView.Columns[0].Width = Width - 20;
        }
    }
}
