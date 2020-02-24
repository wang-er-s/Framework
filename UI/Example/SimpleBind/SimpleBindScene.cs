using Framework.Context;
using Framework.Services;

namespace Framework.UI.Example
{
    public class SimpleBindScene : SceneContext
    {
        public SimpleBindScene(ApplicationContext applicationContext, IServiceContainer container) : base(applicationContext, container)
        {
        }

        protected override string _sceneName { get; set; } = "SimpleBind";
        protected override void OnSceneLoaded()
        {
            
        }
    }
}