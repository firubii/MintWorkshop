using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Nodes
{
    public class ArchiveTreeNode : DynamicTreeNode
    {
        public Archive Archive;

        public ArchiveTreeNode(Archive archive)
        {
            Archive = archive;

            ImageKey = "Archive";
            SelectedImageKey = "Archive";

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
            foreach (Module module in children)
                Nodes.Add(new ModuleTreeNode(module));

            TreeView.EndUpdate();
        }

        public override void Close()
        {
            Nodes.Clear();
            Nodes.Add("_dummy");
        }
    }
}
