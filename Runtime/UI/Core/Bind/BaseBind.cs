namespace Framework.UI.Core.Bind
{
    public abstract class BaseBind
    {
        protected object Container;

        public BaseBind(object container)
        {
            Container = container;
        }
        public abstract void ClearBind();
    }
}