using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public static UIManager instance;
	PanelData data;
	UIPanel panel1;
	UIPanel panel2;

	void Awake()
	{
		instance = this;
		UIList = new List<UIPanel>();
		ClearUI ();
	}

	List<UIPanel> UIList;
	public void OnUIUpdate()
	{
		foreach (UIPanel panel in UIList)
			panel.OnUIUpdate ();
	}

	public void AddOnUIUpdate(UIPanel uip)
	{
		if (!UIList.Contains (uip))
			this.UIList.Add (uip);
	}

	public void InitUI()
	{

	}

	public void ClearUI()
	{
		UIPanel[] uipList = GameObject.FindObjectsOfType<UIPanel> ();
		foreach(UIPanel uip in uipList)
		{
			Destroy(uip.gameObject);
		}
	}
}
