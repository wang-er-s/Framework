namespace Framework
{
    public abstract class BaseBind : IResetBind , IReference
    {
        protected object Container;

        public virtual void Init(object container)
        {
            Container = container;
        }

        /// <summary>
        /// 换绑的时候
        /// </summary>
        public void Reset()
        {
            OnReset();
        }
        protected abstract void OnReset();

        /// <summary>
        /// 销毁的时候
        /// </summary>
        public void Clear()
        {
            OnClear();
        }
        protected abstract void OnClear();
    }
}