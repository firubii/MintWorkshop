using KirbyLib;
using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Editors
{
    public partial class ViewArchiveForm : Form
    {
        public ViewArchiveForm(ArchiveContext ctx)
        {
            InitializeComponent();

            XData xdata = ctx.ArchiveRtDL != null ? ctx.ArchiveRtDL.XData : ctx.Archive.XData;

            xdataVerMaj.Value = xdata.Version[0];
            xdataVerMin.Value = xdata.Version[1];

            endianness.SelectedIndex = (int)xdata.Endianness;

            mintVersion.Text = "Version: " + (ctx.ArchiveRtDL != null ? ctx.ArchiveRtDL.GetVersionString() : ctx.Archive.GetVersionString());

            isCompressed.Checked = ctx.IsCompressed;

            Text = Path.GetFileName(ctx.Path);
        }
    }
}
