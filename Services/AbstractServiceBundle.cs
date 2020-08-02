namespace Framework.Services
{
    public abstract class AbstractServiceBundle : IServiceBundle
    {
        private readonly IServiceContainer _container;
        public AbstractServiceBundle(IServiceContainer container)
        {
            _container = container;
        }

        public void Start()
        {
            OnStart(_container);
        }

        protected abstract void OnStart(IServiceContainer container);

        public void Stop()
        {
            OnStop(_container);
        }

        protected abstract void OnStop(IServiceContainer container);

    }
}
