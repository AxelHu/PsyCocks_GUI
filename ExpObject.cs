using UnityEngine;
using System.Collections;

public class ExpObject : MonoBehaviour
{
	public float timePass;

	public UISprite popoutPic;

	public virtual void Init()
	{
		timePass = 0f;
		Object prefab3 = Resources.Load ("Prefabs/Exp5Pic");
		GameObject go3 = GameObject.Instantiate (prefab3) as GameObject;
		popoutPic = go3.GetComponent<UISprite> ();
		PanelData pd3 = new PanelData ();
		pd3.Init (0, 0, 0, 0, 2);
		popoutPic.InitWithPic (pd3, null, "Pics/TaskR/ST_3D");
		popoutPic.gameObject.SetActive (false);
	}

	public virtual void LoadData()
	{

	}

	public virtual void UpdateExpLogic()
	{
		timePass += ExpManager.GetDeltaTime ();
	}

	public void ShowPopout(string path, float posX, float posY, float time)
	{
		popoutPic.gameObject.SetActive (true);
		popoutPic.SetPic (path);
		popoutPic.posX = posX;
		popoutPic.posY = posY;
		StartCoroutine (ClosePopout (time));
	}
	
	public IEnumerator ClosePopout(float time)
	{
		yield return new WaitForSeconds (time);
		if(popoutPic.gameObject != null)
			popoutPic.gameObject.SetActive (false);
	}
}

