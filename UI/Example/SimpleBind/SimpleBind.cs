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
	private VMFactory vmFactory;
	// Use this for initialization
	void Start ()
	{
		SceneViewLocator sceneViewLocator = new SceneViewLocator();
		vmFactory = new VMFactory(sceneViewLocator);
		vm = vmFactory.Create<SetupViewModel>();
		vm.Visible = new BindableProperty<bool>(true);
		vm.Name = new BindableProperty<string>("JJ");
		newVm = vmFactory.Create<SetupViewModel>();
		vm.ShowView();
		view = vmFactory.GetView(vm);
	}
	
	private void OnGUI()
	{
		if (GUI.Button(new Rect(100, 300, 100, 100), "切换vm"))
		{
			vmFactory.ChangeVM(view.ViewModel == vm ? newVm : vm, view);
		}
	}
}
