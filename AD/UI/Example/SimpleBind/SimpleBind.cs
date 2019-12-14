using System.Collections;
using System.Collections.Generic;
using AD.UI.Core;
using AD.UI.Example;
using UnityEngine;

public class SimpleBind : MonoBehaviour
{
	private SetupViewModel vm;
	// Use this for initialization
	void Start ()
	{
		vm = new SetupViewModel();
		UIMgr.Create("panel", vm: vm);
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			vm.Name.Value = "hahah";
			vm.Visible.Value = true;
		}
	}
}
