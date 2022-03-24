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
            List<Rule> rules = new List<Rule>();
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(CheckerCollection)))
                {
                    collections.Add(Activator.CreateInstance(type) as BasicAssetCheckerCollection);
                    continue;
                }
                
                if (type.IsSubclassOf(typeof(Rule))  && !type.IsAbstract)
                {
                    rules.Add(Activator.CreateInstance(type) as Rule);
                }
            }
            foreach (var collection in collections)
            {
                foreach (var rule in rules)
                {
                    collection.AddRule(rule);
                }
                
                menuTree.Add(collection.Name, collection);
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