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
            vm = new ListBindViewModel();
            pair_vm = new ListPairsBindViewModel();
        }
    }
}