using UnityEngine;

namespace AD
{

    /// <summary>
    /// 对XXXLoader的结果Asset进行Debug显示
    /// </summary>
    public class ResourceLoadedAssetDebugger : MonoBehaviour
    {
        public string MemorySize;
        public UnityEngine.Object TheObject;
        private const string bigType = "LoadedAssetDebugger";
        public string Type;
        private bool IsRemoveFromParent = false;

        public static ResourceLoadedAssetDebugger Create(string type, string url, UnityEngine.Object theObject)
        {
            var newHelpGameObject = new GameObject(string.Format("LoadedObject-{0}-{1}", type, url));
            DebuggerObjectTool.SetParent(bigType, type, newHelpGameObject);

            var newHelp = newHelpGameObject.AddComponent<ResourceLoadedAssetDebugger>();
            newHelp.Type = type;
            newHelp.TheObject = theObject;
            newHelp.MemorySize = $"{UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(theObject) / 1024f:F5}KB";
            return newHelp;
        }

        private void Update()
        {
            if (TheObject == null && !IsRemoveFromParent)
            {
                DebuggerObjectTool.RemoveFromParent(bigType, Type, gameObject);
                IsRemoveFromParent = true;
            }
        }

        // 可供调试删资源
        private void OnDestroy()
        {
            if (!IsRemoveFromParent)
            {
                DebuggerObjectTool.RemoveFromParent(bigType, Type, gameObject);
                IsRemoveFromParent = true;
            }
        }
    }
}
