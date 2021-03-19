using System;
using UnityEngine;

namespace Framework.UI.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TransformPath : Attribute
    {
        public string Path;

        public TransformPath(string path)
        {
            this.Path = path;
        }
    }
}