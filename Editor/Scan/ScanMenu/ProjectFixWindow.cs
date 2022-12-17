using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;

namespace Framework.Editor
{
    public class ProjectFixWindow : OdinMenuEditorWindow
    {
        public static void Open(List<ScanRule> rules)
        {
            Init(rules);
            GetWindow<ProjectFixWindow>().Show();
        }

        private static void Init(List<ScanRule> rules)
        {
            
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false, OdinMenuStyle.TreeViewStyle);
            return tree;
        }
    }
}