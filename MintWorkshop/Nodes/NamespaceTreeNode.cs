using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Nodes
{
    public class NamespaceTreeNode : DynamicTreeNode
    {
        public string Namespace;

        public NamespaceTreeNode(string name)
        {
            Namespace = name;

            Name = Namespace;
            Text = Namespace.Substring(Namespace.LastIndexOf('.') + 1);

            ImageIndex = 1;
            SelectedImageIndex = 1;

            ToolTipText = Namespace;

            Close();
        }

        public void Update()
        {
            Name = Namespace;
            Text = Namespace.Substring(Namespace.LastIndexOf('.') + 1);

            ToolTipText = Namespace;
        }

        public override void Open()
        {
            TreeView.BeginUpdate();

            Update();

            Nodes.Clear();

            TreeNode parent = Parent;
            while (!(parent is ArchiveTreeNode) && !(parent is ArchiveRtDLTreeNode))
                parent = parent.Parent;

            if (parent is ArchiveTreeNode)
            {
                ArchiveTreeNode arcNode = parent as ArchiveTreeNode;

                var names = arcNode.Archive.Modules
                    .Where(x => x.Name.StartsWith(Namespace + "."))
                    .Select(x => x.Name)
                    .Order(StringComparer.Ordinal)
                    .ToArray();
                List<string> namespaces = new List<string>();
                for (int i = 0; i < names.Length; i++)
                {
                    string sub = names[i].Substring(Namespace.Length + 1).Split('.')[0];

                    string n = Namespace + "." + sub;
                    if (n != Namespace && !namespaces.Contains(n) && !arcNode.Archive.ModuleExists(n))
                        namespaces.Add(n);
                }

                var sorted = namespaces.Order(StringComparer.Ordinal).ToArray();
                for (int i = 0; i < sorted.Length; i++)
                    Nodes.Add(new NamespaceTreeNode(sorted[i]));

                var children = arcNode.Archive.Modules
                    .Where(x => x.Name.StartsWith(Namespace + ".") && x.Name.Substring(Namespace.Length + 1).Count(x => x == '.') == 0)
                    .OrderBy(x => x.Name, StringComparer.Ordinal);
                foreach (Module module in children)
                    Nodes.Add(new ModuleTreeNode(module));
            }
            else
            {
                ArchiveRtDLTreeNode arcNode = parent as ArchiveRtDLTreeNode;

                var names = arcNode.Archive.Modules
                    .Where(x => x.Name.StartsWith(Namespace + "."))
                    .Select(x => x.Name)
                    .Order(StringComparer.Ordinal)
                    .ToArray();
                List<string> namespaces = new List<string>();
                for (int i = 0; i < names.Length; i++)
                {
                    string sub = names[i].Substring(Namespace.Length + 1).Split('.')[0];

                    string n = Namespace + "." + sub;
                    if (n != Namespace && !namespaces.Contains(n) && !arcNode.Archive.ModuleExists(n))
                        namespaces.Add(n);
                }

                var sorted = namespaces.Order(StringComparer.Ordinal).ToArray();
                for (int i = 0; i < sorted.Length; i++)
                    Nodes.Add(new NamespaceTreeNode(sorted[i]));

                var children = arcNode.Archive.Modules
                    .Where(x => x.Name.StartsWith(Namespace + ".") && !x.Name.Substring(Namespace.Length + 1).Any(x => x == '.'))
                    .OrderBy(x => x.Name, StringComparer.Ordinal);
                foreach (ModuleRtDL module in children)
                    Nodes.Add(new ModuleRtDLTreeNode(module));
            }

            TreeView.EndUpdate();
        }

        public override void Close()
        {
            Nodes.Clear();
            Nodes.Add("_dummy");
        }
    }
}
