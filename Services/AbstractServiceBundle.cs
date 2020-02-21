namespace Framework.Services
{
    public abstract class AbstractServiceBundle : IServiceBundle
    {
        private IServiceContainer _container;
        public AbstractServiceBundle(IServiceContainer container)
        {
            this._container = container;
        }

        public void Start()
        {
            this.OnStart(_container);
        }

        protected abstract void OnStart(IServiceContainer container);

        public void Stop()
        {
            this.OnStop(_container);
        }

        protected abstract void OnStop(IServiceContainer container);

    }
}
