using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class FrameworkEngine
    {
        private static List<IGameModule> gameModules = new();
        public static event Action<float> LogicUpdate;
        public static event Action<float> RenderUpdate;

        private static float logicUpdateInterval = 0.03f;
        private static float renderUpdateInterval = 0.02f;

        private static float logicTimer;
        private static float renderTimer;

        static FrameworkEngine()
        {
            LogicUpdate += f => { };
            RenderUpdate += f => { };
        }
        
        public static void AddModule(IEnumerable<IGameModule> modules)
        {
            gameModules.AddRange(modules);
        }

        public static void Update()
        {
            logicTimer += Time.deltaTime;
            renderTimer += Time.deltaTime;
            if (logicTimer >= logicUpdateInterval)
            {
                LogicUpdate(logicTimer);
                for (int i = 0; i < gameModules.Count; i++)
                {
                    gameModules[i].OnUpdate(logicTimer);
                }

                logicTimer = 0;
            }

            if (renderTimer >= renderUpdateInterval)
            {
                RenderUpdate(renderTimer);
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
        
        
    }
}