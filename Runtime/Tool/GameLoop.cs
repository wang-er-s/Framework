using System;

namespace Framework
{
    public class GameLoop : MonoSingleton<GameLoop>
    {
        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        public event Action OnApplicationQuitEvent;

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

		private void OnApplicationQuit()
		{
            OnApplicationQuitEvent?.Invoke();
		}
	}
}