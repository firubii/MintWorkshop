using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Nodes
{
    public class VariableTreeNode : TreeNode
    {
        public MintVariable Variable;

        public VariableTreeNode(MintVariable variable)
        {
            Variable = variable;

            Name = Variable.Name;
            Text = Variable.Type + " " + Variable.Name;

            ImageIndex = 4;
            SelectedImageIndex = 4;
        }

        public void Update()
        {
            Name = Variable.Name;
            Text = Variable.Type + " " + Variable.Name;
        }

        public ObjectTreeNode GetObject()
        {
            return Parent.Parent as ObjectTreeNode;
        }
    }

    public class FunctionTreeNode : TreeNode
    {
        public MintFunction Function;

        public FunctionTreeNode(MintFunction function)
        {
            Function = function;

            Name = Function.Name;
            Text = Function.Name;

            ImageIndex = 5;
            SelectedImageIndex = 5;
        }

        public void Update()
        {
            Name = Function.Name;
            Text = Function.Name;
        }

        public ObjectTreeNode GetObject()
        {
            return Parent.Parent as ObjectTreeNode;
        }
    }

    public class EnumTreeNode : TreeNode
    {
        public MintEnum Enum;

        public EnumTreeNode(MintEnum mintEnum)
        {
            Enum = mintEnum;

            Name = Enum.Name;
            Text = Enum.Name + $" (0x{Enum.Value:X})";

            ImageIndex = 6;
            SelectedImageIndex = 6;
        }

        public void Update()
        {
            Name = Enum.Name;
            Text = Enum.Name + $" (0x{Enum.Value:X})";
        }

        public ObjectTreeNode GetObject()
        {
            return Parent.Parent as ObjectTreeNode;
        }
    }
}
