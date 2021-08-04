using Framework;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBind : MonoBehaviour
{
    public GameObject view;
    // Use this for initialization
    private void Start()
    {
        var vm = new SetupViewModel();
        vm.Visible = new ObservableProperty<bool>(true);
        vm.Name = new ObservableProperty<string>("JJ");
        var setup = new SetupView();
        setup.SetGameObject(view);
        setup.SetVm(vm);
    }
}
