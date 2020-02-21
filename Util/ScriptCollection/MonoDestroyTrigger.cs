using System;
using UnityEngine;

namespace Framework
{
    public class MonoDestroyTrigger : MonoBehaviour
    {
        private event Action _onDestroy;
        
        public void AddDisposeOnDestroy(Action action)
        {
            _onDestroy += action;
        }

        private void OnDestroy()
        {
            _onDestroy?.Invoke();
        }
    }
}