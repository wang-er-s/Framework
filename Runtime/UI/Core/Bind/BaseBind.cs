namespace Framework.UI.Core.Bind
{
    public abstract class BaseBind : IClearable
    {
        protected object Container;

        public BaseBind(object container)
        {
            Container = container;
        }
        public abstract void Clear();
    }
}