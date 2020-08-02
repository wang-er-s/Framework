using System.Collections;
using Framework.Services;
using Framework.UI.Core;
using UnityEngine.SceneManagement;

namespace Framework.Context
{
    public abstract class SceneContext : Context
    {
        protected ApplicationContext _applicationContext;
        protected abstract string _sceneName { get; set; }

        protected SceneContext(ApplicationContext applicationContext, IServiceContainer container) : base( container, null)
        {
            this._applicationContext = applicationContext;
            AbstractServiceBundle serviceBundle = new SceneServiceBundle(GetContainer());
            serviceBundle.Start();
        }

        protected abstract void OnSceneLoaded();

        public void Load()
        {
            if (SceneManager.GetActiveScene().name == _sceneName)
            {
                OnSceneLoaded();
                return;
            }
            IEnumeratorUtil.StartCoroutine(load());
        }

        IEnumerator load()
        {
            var operation = SceneManager.LoadSceneAsync(_sceneName);
            yield return operation;
            OnSceneLoaded();
        }
        
    }
}