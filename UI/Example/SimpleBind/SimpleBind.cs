using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBind : MonoBehaviour
{
    private SetupViewModel vm;
    private BindableProperty<int> Age;
    private View view;
    private VMCreator _vmCreator;

    // Use this for initialization
    private void Start()
    {
        var sceneViewLocator = new SceneViewLocator();
        _vmCreator = new VMCreator(sceneViewLocator);
        vm = SetupViewModel.Create(_vmCreator);
        vm.Visible = new BindableProperty<bool>(true);
        vm.Name = new BindableProperty<string>("JJ");
        vm.ShowView();
        view = _vmCreator.GetView(vm);
    }
}