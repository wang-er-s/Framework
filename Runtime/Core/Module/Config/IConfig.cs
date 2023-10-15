using System.ComponentModel;

namespace Framework
{
    /// <summary>
    /// 每个Config的基类
    /// </summary>
    public abstract class BaseConfig : ISupportInitialize
    {
        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }
    }
}