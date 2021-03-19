using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Framework.Editor
{
    public class ILRuntimeConfigWindow : OdinEditorWindow
    {
        [MenuItem("Framework/ILRuntime")]
        public static void Open()
        {
            var window = GetWindow<ILRuntimeConfigWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(400, 500);
            window.Init();
        }

        private void Init()
        {
            BuildDll = new ILRuntimeBuildDll();
            AdapterGenerator = new ILRuntimeAdapterGenerator();
            AdapterGenerator.Init();
            ClrBinding = new ILRuntimeCLRBinding();
        }
        
        public ILRuntimeBuildDll BuildDll;
        
        public ILRuntimeAdapterGenerator AdapterGenerator;
        
        public ILRuntimeCLRBinding ClrBinding;

    }
}