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
//		UIManager.Init();
		vm = new SetupViewModel()
		{
			Visible = new BindableProperty<bool>(true),
			Name =  new BindableProperty<string>("JJ")
		};
		newVm = new SetupViewModel();
//		view = UIManager.Create("SimpleBind", vm: vm);
	}
	
	private void OnGUI()
	{
		if (GUI.Button(new Rect(100, 100, 100, 100), "跳转场景"))
		{
			SceneManager.LoadScene(1);
		}
		if (GUI.Button(new Rect(100, 300, 100, 100), "切换vm"))
		{
			view.ViewModel = view.ViewModel == vm ? newVm : vm;
		}
		if (GUI.Button(new Rect(100, 500, 100, 100), "更改原来的vm"))
		{
			((IBindableProperty<bool>) vm.Visible).Value = !((IBindableProperty<bool>) vm.Visible).Value;
		}
		if (GUI.Button(new Rect(100, 700, 100, 100), "更改新的vm"))
		{
			((IBindableProperty<bool>) newVm.Visible).Value = !((IBindableProperty<bool>) newVm.Visible).Value;
		}
	}
}
