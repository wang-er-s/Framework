using UnityEngine;

public class LoopScrollTest : MonoBehaviour
{
    public GameObject go;
    public int count = 100;
    private void Start()
    {
        LoopView view = new LoopView();
        view.SetGameObject(go);
        view.SetVm(new LoopVM(count));
    }
}

