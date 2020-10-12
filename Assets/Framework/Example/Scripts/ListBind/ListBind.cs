using Framework.UI.Core;
using UnityEngine;

public class ListBind : MonoBehaviour
{
    private ListBindViewModel vm;
    private ListPairsBindViewModel pair_vm;

    // Start is called before the first frame update
    private void Start()
    {
        var sceneViewLocator = new SceneViewLocator();
        vm = new ListBindViewModel();
        pair_vm = new ListPairsBindViewModel();
    }
}