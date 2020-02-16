using System.Collections;
using Framework.Services;
using Framework.UI.Core;
using UnityEngine.SceneManagement;

namespace Framework.Context
{
    public abstract class SceneContext : Context
    {
        protected ApplicationContext applicationContext;
        protected abstract string SceneName { get; set; }

        protected SceneContext(ApplicationContext applicationContext, IServiceContainer container) : base( container, null)
        {
            this.applicationContext = applicationContext;
            AbstractServiceBundle serviceBundle = new SceneServiceBundle(GetContainer());
            serviceBundle.Start();
        }

        protected abstract void OnSceneLoaded();

        public void Load()
        {
            if (SceneManager.GetActiveScene().name == SceneName)
            {
                OnSceneLoaded();
                return;
            }
            IEnumeratorTool.StartCoroutine(load());
        }

        IEnumerator load()
        {
            var operation = SceneManager.LoadSceneAsync(SceneName);
            yield return operation;
            OnSceneLoaded();
        }
        
    }
}