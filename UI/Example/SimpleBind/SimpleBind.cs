using Framework.UI.Core;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBind : MonoBehaviour
{
	private SetupViewModel vm;
	private SetupViewModel newVm;
	private BindableProperty<int> Age;
	private View view;
	private VMCreator _vmCreator;
	// Use this for initialization
	void Start ()
	{
		SceneViewLocator sceneViewLocator = new SceneViewLocator();
		_vmCreator = new VMCreator(sceneViewLocator);
		vm = SetupViewModel.Create(_vmCreator);
		vm.Visible = new BindableProperty<bool>(true);
		vm.Name = new BindableProperty<string>("JJ");
		newVm = SetupViewModel.Create(_vmCreator);
		vm.ShowView();
		view = _vmCreator.GetView(vm);
	}
	
	private void OnGUI()
	{
		if (GUI.Button(new Rect(100, 300, 100, 100), "切换vm"))
		{
			_vmCreator.ChangeVM(view.ViewModel == vm ? newVm : vm, view);
		}
	}
}
