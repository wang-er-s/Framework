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
	// Use this for initialization
	void Start ()
	{
		UIMgr.RefreshCanvas();
		vm = new SetupViewModel();
		UIMgr.Create("SimpleBind", vm: vm);
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(100, 100, 100, 100), "跳转场景"))
		{
			SceneManager.LoadScene(1);
		}
		if (GUI.Button(new Rect(200, 200, 100, 100), "更改数据"))
		{
			vm.Name.Value = "hahah";
			vm.Visible.Value = true;
			vm.ATK.Value = 12;
		}
	}
}
