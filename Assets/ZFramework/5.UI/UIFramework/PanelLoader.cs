using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelLoader  {

	//ResLoader mResLoader = ResLoader.Allocate();

	public GameObject LoadPanelPrefab(string panelName)
	{
		//return mResLoader.LoadSync<GameObject>(panelName);
		return new GameObject ();
	}

	public GameObject LoadPanelPrefab(string assetBundleName, string panelName)
	{
		//return mResLoader.LoadSync<GameObject>(assetBundleName, panelName);
		return new GameObject ();
	}

	public void Unload()
	{
		//mResLoader.Recycle2Cache();
		//mResLoader = null;
	}
}
