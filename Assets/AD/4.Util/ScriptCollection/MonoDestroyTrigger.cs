using System;
using UnityEngine;

namespace AD
{
    public class MonoDestroyTrigger : MonoBehaviour
    {
        private event Action onDestroy;
        
        public void AddDisposeOnDestroy(Action action)
        {
            onDestroy += action;
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }
    }
}