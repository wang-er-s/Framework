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
            List<IRule> rules = new List<IRule>();
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(CheckerCollection)))
                {
                    collections.Add(Activator.CreateInstance(type) as BasicAssetCheckerCollection);
                    continue;
                }
                
                if (typeof(IRule).IsAssignableFrom(type) && !type.IsInterface)
                {
                    rules.Add(Activator.CreateInstance(type) as IRule);
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

        public void Run()
        {
            List<BasicAssetCheckerCollection> collections = new List<BasicAssetCheckerCollection>();
            List<IRule> rules = new List<IRule>();
            List<IAssetRule> assetRules = new List<IAssetRule>();
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BasicAssetCheckerCollection)))
                {
                    collections.Add(Activator.CreateInstance(type) as BasicAssetCheckerCollection);
                    continue;
                }
                
                if (type.IsSubclassOf(typeof(IRule)))
                {
                    rules.Add(Activator.CreateInstance(type) as IRule);
                }

                if (type.IsSubclassOf(typeof(IAssetRule)))
                {
                    assetRules.Add(Activator.CreateInstance(type) as IAssetRule);
                }
            }
            foreach (var collection in collections)
            {
                foreach (var rule in rules)
                {
                    collection.AddRule(rule);
                }
            }
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