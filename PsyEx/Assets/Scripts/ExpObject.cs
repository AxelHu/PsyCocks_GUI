using UnityEngine;
using System.Collections;

public class ExpObject : MonoBehaviour
{
	public float timePass;

	public UISprite popoutPic;

	protected UIPanel background;

	public PsyEx.Mapper.Tester tester;
	public PsyEx.Mapper.ExConfig config;
	public PsyEx.Mapper.HDConfig hdConfig;

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

		Object prefab4 = Resources.Load ("Prefabs/Background");
		GameObject go4 = GameObject.Instantiate (prefab4) as GameObject;
		background = go4.GetComponent<UIPanel> ();
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

	public void ClosePopout()
	{
		if(popoutPic.gameObject != null)
			popoutPic.gameObject.SetActive (false);
	}

	public GameObject PopoutWithText(string text, float time, float posX = 0, float posY = 0)
	{
		Object prefab = Resources.Load ("Prefabs/Text");
		GameObject go = GameObject.Instantiate (prefab) as GameObject;
		TextMesh tm = go.GetComponentInChildren<TextMesh> ();
		go.transform.position += new Vector3 (posX / 100f, posY / 100f, 0);
		tm.text = text;
		StartCoroutine (ClosePopout (time, go));
		return go;
	}

	public IEnumerator ClosePopout(float time, GameObject go)
	{
		yield return new WaitForSeconds (time);
		if (go != null)
			Destroy (go);
	}

	public virtual void EndExp()
	{}

	public virtual void ClearUI()
	{
		if(background.gameObject != null)
			Destroy (background.gameObject);
		if(popoutPic.gameObject != null)
			Destroy (popoutPic.gameObject);
		GameObject[] goList = GameObject.FindGameObjectsWithTag ("Text");
		foreach (GameObject go in goList)
			Destroy (go);
	}

	public void SetBackgroundColor()
	{
		Material mat = background.gameObject.GetComponentInChildren<Renderer> ().material;
	}
}

