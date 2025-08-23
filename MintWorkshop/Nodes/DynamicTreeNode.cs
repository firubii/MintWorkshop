using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Nodes
{
    public abstract class DynamicTreeNode : TreeNode
    {
        public abstract void Open();
        public abstract void Close();
    }
}
