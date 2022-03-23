using System;
using System.Collections.Generic;

namespace Framework.Editor.AssetsChecker
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;

    public class AssetsChecker : OdinMenuEditorWindow
    {
        [MenuItem("Tools/AssetsChecker")]
        private static void OpenWindow()
        {
            GetWindow<AssetsChecker>().Show();
        }

        private OdinMenuTree menuTree;

        protected override OdinMenuTree BuildMenuTree()
        {
            menuTree = new OdinMenuTree();
            List<BasicAssetCheckerCollection> collections = new List<BasicAssetCheckerCollection>();
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BasicAssetCheckerCollection)))
                {
                    collections.Add(Activator.CreateInstance(type) as BasicAssetCheckerCollection);
                }
            }
            return menuTree;
        }
    }

    public class AA
    {
        public string Name;

        [Button]
        public void CX()
        {

        }
    }
}