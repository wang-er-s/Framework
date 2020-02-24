using Framework.UI.Core;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBind : MonoBehaviour
{
	private SetupViewModel vm;
	private SetupViewModel newVm;
	private BindableProperty<int> Age;
	private IView view;
	// Use this for initialization
	void Start ()
	{
		UIManager uiManager = new UIManager();
		VMFactory vmFactory = new VMFactory(uiManager);
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
			view.SetVM(view.ViewModel == vm ? newVm : vm);
		}
	}
}
