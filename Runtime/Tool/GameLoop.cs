using System;

namespace Framework
{
    public class GameLoop : MonoSingleton<GameLoop>
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        public event Action OnFixedUpdate;
        public event Action OnApplicationQuitEvent;
        public event Action<bool> OnApplicationFocusEvent; 
        public event Action<bool> OnApplicationPauseEvent; 

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void OnApplicationQuit()
		{
            OnApplicationQuitEvent?.Invoke();
		}

        private void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusEvent?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPauseEvent?.Invoke(pauseStatus);
        }
    }
}