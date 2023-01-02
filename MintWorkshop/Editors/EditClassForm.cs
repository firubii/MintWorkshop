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
        MintClass baseClass;

        public string ClassName { get; private set; }
        public uint ClassFlags { get; private set; }
        public List<ushort> ClassImpl { get; private set; }
        public List<MintClass.ClassExtend> ClassExt { get; private set; }

        public EditClassForm(MintClass baseClass, ref Dictionary<byte[], string> hashes)
        {
            this.baseClass = baseClass;

            ClassName = baseClass.Name;
            ClassFlags = baseClass.Flags;
            ClassImpl = baseClass.ClassImpl;
            ClassExt = baseClass.Extends;

            InitializeComponent();
            classFlags.Maximum = uint.MaxValue;

            className.Text = ClassName;
            classFlags.Value = ClassFlags;

            for (int i = 0; i < ClassImpl.Count; i++)
            {
                byte[] h = baseClass.ParentScript.XRef[ClassImpl[i]];
                if (hashes.ContainsKey(h))
                    classImpl.Text += $"{hashes[h]}";
                else
                    classImpl.Text += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";

                if (i < ClassImpl.Count - 1)
                    classImpl.Text += ", ";
            }

            for (int i = 0; i < ClassExt.Count; i++)
            {
                byte[] h = baseClass.ParentScript.XRef[ClassExt[i].Index];
                if (hashes.ContainsKey(h))
                    classExt.Text += $"{hashes[h]}";
                else
                    classExt.Text += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";

                if (i < ClassExt.Count - 1)
                    classExt.Text += ", ";
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ClassName = className.Text;
            ClassFlags = (uint)classFlags.Value;
            ClassImpl = new List<ushort>();
            if (classImpl.Text != "")
            {
                string[] impl = classImpl.Text.Split(',');
                for (int i = 0; i < impl.Length; i++)
                    ClassImpl.Add((ushort)baseClass.ParentScript.AddXRef(impl[i].Trim()));
            }

            ClassExt = new List<MintClass.ClassExtend>();
            if (classExt.Text != "")
            {
                string[] ext = classExt.Text.Split(',');
                for (int i = 0; i < ext.Length; i++)
                {
                    string ex = ext[i].Trim();
                    bool std = false;
                    foreach (KeyValuePair<ushort, string> pair in MintClass.StdTypes)
                    {
                        if (pair.Value == ex)
                        {
                            ClassExt.Add(new MintClass.ClassExtend(pair.Key, true));
                            std = true;
                            break;
                        }
                    }
                    if (!std)
                        ClassExt.Add(new MintClass.ClassExtend((ushort)baseClass.ParentScript.AddXRef(ex), false));
                }
            }

            DialogResult = DialogResult.OK;
        }
    }
}
