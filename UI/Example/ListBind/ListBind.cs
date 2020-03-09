using Framework.UI.Core;
using UnityEngine;

public class ListBind : MonoBehaviour
{
    private ListBindViewModel vm;
    private ListPairsBindViewModel pair_vm;
    // Start is called before the first frame update
    void Start()
    {
        SceneViewLocator sceneViewLocator = new SceneViewLocator();
        VMCreator vmCreator = new VMCreator(sceneViewLocator);
        vm = ListBindViewModel.Create(vmCreator);
        vm.ShowView();
        pair_vm = ListPairsBindViewModel.Create(vmCreator);
        pair_vm.ShowView();
    }

}
