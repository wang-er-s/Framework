using System;
using System.Collections;
using System.Collections.Generic;
using AD.UI.Core;
using AD.UI.Example;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleBind : MonoBehaviour
{
	private SetupViewModel vm;
	private BindableField<int> Age;
	// Use this for initialization
	void Start ()
	{
		UIMgr.RefreshCanvas();
		vm = new SetupViewModel()
		{
			Visible = new BindableField<bool>(false),
			Name =  new BindableField<string>("JJ")
		};
		UIMgr.Create("SimpleBind", vm: vm);
	}
	

	private void OnGUI()
	{
		if (GUI.Button(new Rect(100, 100, 100, 100), "跳转场景"))
		{
			SceneManager.LoadScene(1);
		}
	}
}
