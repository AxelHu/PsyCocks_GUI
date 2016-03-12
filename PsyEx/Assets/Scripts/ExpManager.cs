using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PsyEx.Mapper;
using System.IO;
using Newtonsoft.Json;

public class ExpManager : MonoBehaviour
{
	public static ExpManager instance;
	public EXP_STATUS currentStatus;

	public ExpObject currentExp;
	public GameObject UICamera;

	protected int totalExpNum;
	protected int currentExpNum;
	List<ExpObject> expList;

	string configFilePath = "./ExpRun/play.set";

	bool currentExpInitFlag = false;

	bool useAssetBundle = false;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		currentStatus = EXP_STATUS.INIT;
		Debug.Log (Directory.GetCurrentDirectory ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		//TODO
		if(currentStatus == EXP_STATUS.INIT)
		{
			ReadExpList ();
			InitCamera ();
			currentExpInitFlag = true;
			currentStatus = EXP_STATUS.RUN;
		}
		else if(currentStatus == EXP_STATUS.RUN)
		{
			if (currentExpInitFlag) 
			{
				currentExpInitFlag = false;
				currentExp.Init ();
			}
			currentExp.UpdateExpLogic();
			UIManager.instance.OnUIUpdate();
		}
		else if(currentStatus == EXP_STATUS.PAUSE)
		{

		}
		else if(currentStatus == EXP_STATUS.PAUSE)
		{

		}
		else if(currentStatus == EXP_STATUS.OVER)
		{

		}
	}


	public static float GetDeltaTime()
	{
		return Time.deltaTime;
	}

	public static float GetSystemTime()
	{
		// TODO
		return 0f;
	}

	public enum EXP_STATUS
	{
		INIT,
		RUN,
		PAUSE,
		OVER
	}



	// 1-pixel in game world = 0.01 unit, 1 unit = 100pixel
	// orthoCamera orthosize = half-camera height, 
	// window resolution A x B, 
	// pic 1-pixel -> resol 1-pixel
	public void InitCamera()
	{
		Camera uicam = GameObject.FindObjectOfType<Camera> ();
		Resolution a = Screen.currentResolution;
		Debug.Log (a.width + "_" + a.height);
		Debug.Log (Screen.width + "_" + Screen.height);
		if (uicam != null)
		{
			uicam.GetComponent<OrthoCam> ().Init ();
			UICamera = uicam.gameObject;
			UICamera.transform.position = new Vector3 (0, 0, 100);
		}
		else 
		{
			GameObject prefab = Resources.Load ("/Prefabs/UICamera") as GameObject;
			UICamera = GameObject.Instantiate (prefab);
			UICamera.transform.position = new Vector3 (0, 0, 100);
			UICamera.GetComponent<OrthoCam> ().Init ();
		}
	}

	public void ReadExpList()
	{
		string jsonStr = Utils.DoFileJsonInput(configFilePath);
		List<ExConfig> exConList;
		exConList = JsonConvert.DeserializeObject<List<ExConfig>> (jsonStr);
		Debug.Log (exConList);
		expList = new List<ExpObject> ();
		foreach (ExConfig exCon in exConList) 
		{
			int num = int.Parse (exCon.exId.Substring(0, 1));
			ExpObject exp = LoadExp (num);
			exp.config = exCon;
			expList.Add (exp);
		}

		totalExpNum = expList.Count;
		currentExpNum = 0;
		currentExp = expList [0];

		//Test code
		//expList = new List<ExpObject> ();
		//ExpObject ex = LoadExp (5);
		//expList.Add (ex);
		//totalExpNum = 1;
		//currentExpNum = 0;
		//currentExp = expList [0];
	}

	public ExpObject LoadExp(int expNum)
	{
		GameObject prefab = Resources.Load ("Prefabs/Exp" + expNum) as GameObject;
		GameObject go = GameObject.Instantiate (prefab);
		ExpObject exp = go.GetComponent<ExpObject> ();
		return exp;
	}

	public void GotoNextExp()
	{
		//currentExp.EndExp ();
		currentExp.ClearUI();
		currentExpNum++;
		if (currentExpNum >= totalExpNum) 
		{
			EndAll ();
			return;
		}
		currentExp = expList [currentExpNum];
		currentExpInitFlag = true;
	}

	public void EndAll()
	{
		Destroy (UICamera);
		Application.Quit ();
	}
}

