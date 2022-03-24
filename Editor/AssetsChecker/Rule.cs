using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Editor.AssetsChecker
{
    public abstract class Rule
    {
        [HideInInspector]
        public RuleDataTable Table;
        
        [ShowInInspector]
        [HideLabel]
        protected abstract string Description { get; }

        protected abstract RulePriority Priority { get; }

        public abstract void Run();
        
        public virtual string HelpUrl { get; }

        public virtual void TryFix()
        {
        }
    }

    public enum RulePriority
    {
        Medium,
        High,
    }

    public class BelongToCollectionAttribute : Attribute
    {
        public Type BelongType { get; }

        public BelongToCollectionAttribute(Type type)
        {
            if (!type.IsSubclassOf(typeof(CheckerCollection)))
            {
                Debug.LogError($"{type} 不属于CheckerCollection的子类");
                return;
            }
            BelongType = type;
        }
    }
}