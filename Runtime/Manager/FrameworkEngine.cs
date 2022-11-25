using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FrameworkEngine : MonoBehaviour 
    {

        private static List<IGameModule> gameModules = new();
        public static event Action<float> LogicUpdateEvent;
        public static event Action<float> RenderUpdateEvent;
        public static event Action<bool> ApplicationPauseEvent;
        public static event Action<bool> ApplicationFocusEvent;

        private static float logicUpdateInterval = 0.03f;
        private static float renderUpdateInterval = 0.02f;

        private static float logicTimer;
        private static float renderTimer;

        static FrameworkEngine()
        {
            LogicUpdateEvent += f => { };
            RenderUpdateEvent += f => { };
        }

        private static FrameworkEngine mono;
        public static void AddModule(IEnumerable<IGameModule> modules)
        {
            if (mono == null)
            {
                mono = new GameObject(nameof(FrameworkEngine)).AddComponent<FrameworkEngine>();
                DontDestroyOnLoad(mono.gameObject);
            }
            gameModules.AddRange(modules);
        }

        public void Update()
        {
            logicTimer += Time.deltaTime;
            renderTimer += Time.deltaTime;
            if (logicTimer >= logicUpdateInterval)
            {
                LogicUpdateEvent(logicTimer);
                for (int i = 0; i < gameModules.Count; i++)
                {
                    gameModules[i].OnUpdate(logicTimer);
                }

                logicTimer = 0;
            }

            if (renderTimer >= renderUpdateInterval)
            {
                RenderUpdateEvent(renderTimer);
                renderTimer = 0;
            }
        }

        public static void DrawModulesGui()
        {
            for (int i = 0; i < gameModules.Count; i++)
            {
                gameModules[i].OnGUI();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            ApplicationPauseEvent?.Invoke(pauseStatus);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            ApplicationFocusEvent?.Invoke(hasFocus);
        }

    }
}