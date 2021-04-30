using UnityEngine;

namespace Framework.UI.Example
{
    public class ListBind : MonoBehaviour
    {
        private ListBindViewModel vm;
        private ListPairsBindViewModel pair_vm;

        // Start is called before the first frame update
        private void Start()
        {
            var listGo = (GameObject) Instantiate(Resources.Load("ListBind"), GameObject.Find("UIRoot").transform);
            var pairsGo = (GameObject) Instantiate(Resources.Load("ListPairsBind"), GameObject.Find("UIRoot").transform);
            var listView = new ListBindView();
            var pairsView = new ListPairsBindView();
            listView.SetGameObject(listGo);
            listView.SetVm(new ListBindViewModel());
            pairsView.SetGameObject(pairsGo);
            pairsView.SetVm(new ListPairsBindViewModel());
        }
    }
}