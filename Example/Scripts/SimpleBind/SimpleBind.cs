using Framework;
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
        Log.Msg();
        vm = new SetupViewModel();
        vm.Visible = new ObservableProperty<bool>(true);
        vm.Name = new ObservableProperty<string>("JJ");
        //UIManager.Ins.Open<SetupView>(vm);
        UIManager.Ins.OpenAsync<ListBindView>(new ListBindViewModel());
        //UIManager.Ins.Open<ListPairsBindView>(new ListPairsBindViewModel());
    }
}
