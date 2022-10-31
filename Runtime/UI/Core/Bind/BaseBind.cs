namespace Framework
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