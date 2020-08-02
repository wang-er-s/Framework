using Framework.UI.Core;

namespace Framework.Services
{
    public class SceneServiceBundle : AbstractServiceBundle
    {
        public SceneServiceBundle(IServiceContainer container) : base(container)
        {
        }

        protected override void OnStart(IServiceContainer container)
        {
            container.Register(new SceneViewLocator());
        }

        protected override void OnStop(IServiceContainer container)
        {
        }
    }
}