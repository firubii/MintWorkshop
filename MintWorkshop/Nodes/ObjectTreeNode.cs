using KirbyLib.Mint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop.Nodes
{
    public class ObjectTreeNode : DynamicTreeNode
    {
        public MintObject Object;

        public ObjectTreeNode(MintObject mintObject, string moduleName)
        {
            Object = mintObject;

            Name = Object.Name;
            Text = Object.Name.StartsWith(moduleName)
                ? Object.Name[(moduleName.LastIndexOf('.') + 1)..]
                : Object.Name;

            ImageIndex = 3;
            SelectedImageIndex = 3;

            ToolTipText = Object.Name;

            Close();
        }

        public void Update()
        {
            Name = Object.Name;
            
            string moduleName = GetModule() is ModuleRtDLTreeNode
                ? (GetModule() as ModuleRtDLTreeNode).Module.Name
                : (GetModule() as ModuleTreeNode).Module.Name;

            Text = Object.Name.StartsWith(moduleName)
                ? Object.Name[(moduleName.LastIndexOf('.') + 1)..]
                : Object.Name;

            ToolTipText = Object.Name;
        }

        public override void Open()
        {
            Update();

            Nodes.Clear();

            TreeNode varList = new TreeNode();
            varList.Text = "Variables";
            varList.ImageIndex = 4;
            varList.SelectedImageIndex = 4;

            for (int i = 0; i < Object.Variables.Count; i++)
                varList.Nodes.Add(new VariableTreeNode(Object.Variables[i]));

            Nodes.Add(varList);

            TreeNode funcList = new TreeNode();
            funcList.Text = "Functions";
            funcList.ImageIndex = 5;
            funcList.SelectedImageIndex = 5;

            for (int i = 0; i < Object.Functions.Count; i++)
                funcList.Nodes.Add(new FunctionTreeNode(Object.Functions[i]));

            Nodes.Add(funcList);

            if (!(GetModule() is ModuleRtDLTreeNode))
            {
                TreeNode enumList = new TreeNode();
                enumList.Text = "Enums";
                enumList.ImageIndex = 6;
                enumList.SelectedImageIndex = 6;

                for (int i = 0; i < Object.Enums.Count; i++)
                    enumList.Nodes.Add(new EnumTreeNode(Object.Enums[i]));

                Nodes.Add(enumList);
            }
        }

        public override void Close()
        {
            Nodes.Clear();
            Nodes.Add("_dummy");
        }

        public TreeNode GetModule()
        {
            return Parent;
        }
    }
}
