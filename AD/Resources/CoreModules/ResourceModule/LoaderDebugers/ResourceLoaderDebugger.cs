using System;
using UnityEngine;

namespace AD
{
    /// <summary>
    /// 只在编辑器下出现，分别对应一个Loader~生成一个GameObject对象，为了方便调试！
    /// </summary>
    public class ResourceLoaderDebugger : MonoBehaviour
    {
        public AbstractResourceLoader TheLoader;
        public int RefCount;
        public float FinishUsedTime; // 参考，完成所需时间
        public static bool IsApplicationQuit = false;

        public static ResourceLoaderDebugger Create(string type, string url, AbstractResourceLoader loader)
        {
            if (IsApplicationQuit) return null;

            const string bigType = "ResourceLoaderDebuger";

            Func<string> getName = () => string.Format("{0}-{1}-{2}", type, url, loader.Desc);

            var newHelpGameObject = new GameObject(getName());
            DebuggerObjectTool.SetParent(bigType, type, newHelpGameObject);
            var newHelp = newHelpGameObject.AddComponent<ResourceLoaderDebugger>();
            newHelp.TheLoader = loader;

            loader.SetDescEvent += (newDesc) =>
            {
                if (loader.RefCount > 0)
                    newHelpGameObject.name = getName();
            };


            loader.DisposeEvent += () =>
            {
                if (!IsApplicationQuit)
                    DebuggerObjectTool.RemoveFromParent(bigType, type, newHelpGameObject);
            };


            return newHelp;
        }

        private void Update()
        {
            RefCount = TheLoader.RefCount;
            FinishUsedTime = TheLoader.FinishUsedTime;
        }

        private void OnApplicationQuit()
        {
            IsApplicationQuit = true;
        }

    }

}
