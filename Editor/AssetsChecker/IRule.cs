using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Editor.AssetsChecker
{
    public interface IRule
    {
        [ShowInInspector]
        string Description { get; }

        RulePriority Priority { get; }

        void Run(out bool hasTable, out RuleDataTable table);
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