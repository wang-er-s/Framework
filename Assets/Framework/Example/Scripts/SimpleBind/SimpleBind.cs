using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBind : MonoBehaviour
{
    private SetupViewModel vm;
    private ObservableProperty<int> Age;
    private View view;

    // Use this for initialization
    private void Start()
    {
        var sceneViewLocator = new SceneViewLocator();
        vm = new SetupViewModel();
        vm.Visible = new ObservableProperty<bool>(true);
        vm.Name = new ObservableProperty<string>("JJ");
        vm.ShowView();
    }
}