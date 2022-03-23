using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.AssetsChecker
{
    using System.Collections.Generic;

    public abstract class CheckerCollection
    {
        public abstract string Name { get; }

        [ShowInInspector]
        [TableList(AlwaysExpanded = true, IsReadOnly = true, HideToolbar = true)]
        protected List<Rule> Rules = new List<Rule>();

        public virtual void AddRule(Rule rule)
        {
            var attrs = rule.GetType().GetCustomAttributes(typeof(BelongToCollectionAttribute), false);
            if (attrs.Length != 1)
            {
                Debug.LogError($"{rule.GetType()} 缺少属性BelongToCollectionAttribute");
                return;
            }

            var belongType = (attrs[0] as BelongToCollectionAttribute).BelongType;
            if (belongType != GetType())
                return;
            Rules.Add(rule);
        }

        [Button("运行", ButtonSizes.Medium)]
        protected virtual void Run()
        {
            foreach (var rule in Rules)
            {
                rule.Run();
            }
        }
    }
}