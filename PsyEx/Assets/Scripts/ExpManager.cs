using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PsyEx.Mapper;
using System.IO;
using Newtonsoft.Json;
using System;

public class ExpManager : MonoBehaviour
{
	public static ExpManager instance;
	public EXP_STATUS currentStatus;

	public ExpObject currentExp;
	public GameObject UICamera;

	protected int totalExpNum;
	protected int currentExpNum;
	public static Tester tester = new Tester();
	List<ExpObject> expList;
	public static HDConfig hdconfig = new HDConfig();

	string testerFilePath = "./ExpRun/play.tester";
	string configFilePath = "./ExpRun/play.set";
	string hdFilePath = "./ExpRun/play.hdset";

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
			ReadTester();
			ReadExpList ();
			ReadHdset();
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

	private void ReadHdset()
	{
		Dictionary<string, string> DataList = new Dictionary<String, String>();
		DataList = Utils.DoFileInput(testerFilePath)[0];
		string str;
		double i;
		DataList.TryGetValue("Speed", out str);
		double.TryParse(str, out i);
		hdconfig.Speed = i;
		DataList.TryGetValue("Sensibility", out str);
		double.TryParse(str, out i);
		hdconfig.Sensibility = i;
		DataList.TryGetValue("Distance", out str);
		double.TryParse(str, out i);
		hdconfig.Distance = i;
		DataList.TryGetValue("Angle", out str);
		double.TryParse(str, out i);
		hdconfig.Angle = i;
	}

	private void ReadTester()
	{
		Dictionary <string, string> DataList = new Dictionary<String, String>();
		DataList = Utils.DoFileInput(testerFilePath)[0];
		string str;
		int i;
		DataList.TryGetValue("ID", out str);
		tester.Id = str;
		DataList.TryGetValue("Name", out str);
		tester.Name = str;
		DataList.TryGetValue("Sex", out str);
		tester.Sex = str;
		DataList.TryGetValue("Age", out str);
		int.TryParse(str, out i);
		tester.Age = i;
		DataList.TryGetValue("Count", out str);
		int.TryParse(str, out i);
		tester.Count = i;
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
		//ExpObject ex = LoadExp (1);
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

