using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Nodes
{
    public class ModuleTreeNode : DynamicTreeNode
    {
        public Module Module;

        public ModuleTreeNode(Module module)
        {
            Module = module;

            Name = Module.Name;
            Text = Module.Name.Split('.').Last();

            ImageIndex = 2;
            SelectedImageIndex = 2;

            ToolTipText = Module.Name;

            Close();
        }

        public void Update()
        {
            Name = Module.Name;
            Text = Module.Name.Split('.').Last();
        }

        public override void Open()
        {
            Nodes.Clear();

            for (int i = 0; i < Module.Objects.Count; i++)
                Nodes.Add(new ObjectTreeNode(Module.Objects[i], Module.Name));
        }

        public override void Close()
        {
            Nodes.Clear();
            if (Module.Objects.Count > 0)
                Nodes.Add("_dummy");
        }

        public ArchiveTreeNode GetArchive()
        {
            TreeNode parent = Parent;
            while (!(parent is ArchiveTreeNode))
                parent = parent.Parent;

            return parent as ArchiveTreeNode;
        }
    }
}
