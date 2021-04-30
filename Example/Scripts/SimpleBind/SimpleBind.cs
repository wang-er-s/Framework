using Framework;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBind : MonoBehaviour
{

    // Use this for initialization
    private void Start()
    {
        var vm = new SetupViewModel();
        vm.Visible = new ObservableProperty<bool>(true);
        vm.Name = new ObservableProperty<string>("JJ");
        var go = (GameObject) Instantiate(Resources.Load("SimpleBind"), GameObject.Find("UIRoot").transform);
        var setup = new SetupView();
        setup.SetGameObject(go);
        setup.SetVm(vm);
    }
}
