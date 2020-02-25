using System.Collections;
using System.Collections.Generic;
using Framework.UI.Core;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ListBind : MonoBehaviour
{
    private ListBindViewModel vm;
    private ListPairsBindViewModel pair_vm;
    // Start is called before the first frame update
    void Start()
    {
        SceneViewLocator sceneViewLocator = new SceneViewLocator();
        VMFactory vmFactory = new VMFactory(sceneViewLocator);
        vm = vmFactory.Create<ListBindViewModel>();
        vm.ShowView();
        pair_vm = vmFactory.Create<ListPairsBindViewModel>();
        pair_vm.ShowView();
    }

}
