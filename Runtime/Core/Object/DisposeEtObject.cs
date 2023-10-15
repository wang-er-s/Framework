using System;
using System.ComponentModel;

namespace Framework
{
    public abstract class DisposeEtObject : ETObject, IDisposable, ISupportInitialize
    {
        public virtual void Dispose()
        {
        }

        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }
    }
}