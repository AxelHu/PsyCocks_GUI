using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpManager : MonoBehaviour
{
	public static ExpManager instance;
	public EXP_STATUS currentStatus;

	public ExpObject currentExp;
	public GameObject UICamera;

	protected int totalExpNum;
	protected int currentExpNum;
	List<ExpObject> expList;

	bool currentExpInitFlag = false;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		currentStatus = EXP_STATUS.INIT;
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
			UIManager.instance.OnUIUpdate();
			currentExp.UpdateExpLogic();
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
		expList = new List<ExpObject> ();
		expList.Add (LoadExp (4));


		totalExpNum = expList.Count;
		currentExpNum = 0;
		currentExp = expList [0];
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
		currentExp.EndExp ();
		currentExpNum++;
		if (currentExpNum >= totalExpNum)
			EndAll ();
		currentExp = expList [currentExpNum];
		currentExpInitFlag = true;
	}

	public void EndAll()
	{}
}

