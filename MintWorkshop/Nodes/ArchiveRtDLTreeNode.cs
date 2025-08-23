using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintWorkshop.Nodes
{
    public class ArchiveRtDLTreeNode : DynamicTreeNode
    {
        public ArchiveRtDL Archive;

        public ArchiveRtDLTreeNode(ArchiveRtDL archive)
        {
            Archive = archive;

            ImageIndex = 0;
            SelectedImageIndex = 0;

            Close();
        }

        public override void Open()
        {
            TreeView.BeginUpdate();

            Nodes.Clear();

            var roots = Archive.Modules.Select(x => x.Name.Split('.')[0]).ToArray();
            List<string> rootNamespaces = new List<string>();
            for (int i = 0; i < roots.Length; i++)
            {
                if (!rootNamespaces.Contains(roots[i]) && !Archive.ModuleExists(roots[i]))
                    rootNamespaces.Add(roots[i]);
            }

            var sorted = rootNamespaces.Order(StringComparer.Ordinal).ToArray();
            for (int i = 0; i < sorted.Length; i++)
                Nodes.Add(new NamespaceTreeNode(sorted[i]));

            var children = Archive.Modules
                .Where(x => x.Name.Count(x => x == '.') == 0)
                .OrderBy(x => x.Name, StringComparer.Ordinal);
            foreach (ModuleRtDL module in children)
                Nodes.Add(new ModuleRtDLTreeNode(module));

            TreeView.EndUpdate();
        }

        public override void Close()
        {
            Nodes.Clear();
            Nodes.Add("_dummy");
        }
    }
}
