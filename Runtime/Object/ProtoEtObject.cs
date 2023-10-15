using System;
using System.ComponentModel;

namespace Framework
{
    public abstract class ProtoEtObject : ETObject, ISupportInitialize
    {
        public object Clone()
        {
            //byte[] bytes = SerializeHelper.Serialize(this);
            //return SerializeHelper.Deserialize(this.GetType(), bytes, 0, bytes.Length);
            return this;
        }

        public virtual void BeginInit()
        {
        }


        public virtual void EndInit()
        {
        }


        public virtual void AfterEndInit()
        {
        }
    }
}