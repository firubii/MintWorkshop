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

            string imageKey;
            switch (Object.Type)
            {
                default:
                    imageKey = "Unknown";
                    break;
                case ObjectType.Class:
                    imageKey = "Class";
                    break;
                case ObjectType.Enum:
                    imageKey = "Enum";
                    break;
                case ObjectType.Interface:
                    imageKey = "Interface";
                    break;
                case ObjectType.Pod:
                    imageKey = "Pod";
                    break;
                case ObjectType.Rawptr:
                    imageKey = "Rawptr";
                    break;
                case ObjectType.Struct:
                    imageKey = "Struct";
                    break;
                case ObjectType.Utility:
                    imageKey = "Utility";
                    break;
            }

            ImageKey = imageKey;
            SelectedImageKey = imageKey;

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

            string imageKey;
            switch (Object.Type)
            {
                default:
                    imageKey = "Unknown";
                    break;
                case ObjectType.Class:
                    imageKey = "Class";
                    break;
                case ObjectType.Enum:
                    imageKey = "Enum";
                    break;
                case ObjectType.Interface:
                    imageKey = "Interface";
                    break;
                case ObjectType.Pod:
                    imageKey = "Pod";
                    break;
                case ObjectType.Rawptr:
                    imageKey = "Rawptr";
                    break;
                case ObjectType.Struct:
                    imageKey = "Struct";
                    break;
                case ObjectType.Utility:
                    imageKey = "Utility";
                    break;
            }

            ImageKey = imageKey;
            SelectedImageKey = imageKey;
        }

        public override void Open()
        {
            Update();

            Nodes.Clear();

            TreeNode varList = new TreeNode();
            varList.Text = "Variables";
            varList.ImageKey = "Variable";
            varList.SelectedImageKey = "Variable";

            for (int i = 0; i < Object.Variables.Count; i++)
                varList.Nodes.Add(new VariableTreeNode(Object.Variables[i]));

            Nodes.Add(varList);

            TreeNode funcList = new TreeNode();
            funcList.Text = "Functions";
            funcList.ImageKey = "Function";
            funcList.SelectedImageKey = "Function";

            for (int i = 0; i < Object.Functions.Count; i++)
                funcList.Nodes.Add(new FunctionTreeNode(Object.Functions[i]));

            Nodes.Add(funcList);

            if (!(GetModule() is ModuleRtDLTreeNode))
            {
                TreeNode enumList = new TreeNode();
                enumList.Text = "Enums";
                enumList.ImageKey = "EnumItem";
                enumList.SelectedImageKey = "EnumItem";

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
