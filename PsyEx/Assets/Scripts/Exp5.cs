using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Exp5 : ExpObject
{

	public UISprite leftPic;
	public UISprite rightPic;

	public EXP5_STATUS currentExpStatus;

	public RanGenExp5 ran8x;
	public RanGenExp5 ran8z;
	public RanGenExp5 ran9x;
	public RanGenExp5 ran9z;

	// setting parameters get from file
	public float pointTime = 3f;//注视点时间
	public float viewTime = 5f;//呈现时间
	public float lastTime = 3f;//倒数时间

	float resPopoutTime = 3f;
	float timeCountdown; // used to count down and show tips etc.
	bool introPopoutFlag = false;
	bool introPopoutFlag2 = false;

    protected string saveTime;
    protected DateTime startTime;
    protected DateTime endTime;

    protected int taskno = 0;

    protected string savePath;
    protected string saveFilename;

    float waitTime; // used in several states to manually record time pass

	public void InitPara()
	{
		
		this.pointTime = config.config5.pointTime;
		this.viewTime = config.config5.viewTime;
		this.lastTime = config.config5.lastTime;

		eightNineFlag = (1 == (int)UnityEngine.Random.Range(0, 2));
		//pointTime = 0.1f;
		//viewTime = 3f;
		//lastTime = 2f;
		ansResTime = 0.5f;
		corOrWroFlag = (1 == (int)UnityEngine.Random.Range(0, 2));
	}

	public override void Init()
	{
		base.Init ();
		InitPara ();
		InitPrefab ();
		InitRan ();

        startTime = DateTime.Now;
		currentExpStatus = EXP5_STATUS.EXP_INIT;
		waitTime = 0.2f;// pretend loading
	}

    void InitOutput()
    {
        List<string> outputlist = new List<string>();
        outputlist = new List<string> { "序号", "左侧图片", "右侧图片", "能否重合", "反应按键", "是否正确", "反应时" };

        saveTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        savePath = Utils.MakeDirectoy("Data\\" + ExpManager.tester.Id + "-" + ExpManager.tester.Name);
        saveFilename = "T5-" + config.sortId + "-任务5-三维心理旋转测试-" + ExpManager.tester.Id + "-" + ExpManager.tester.Name + "-" + ExpManager.tester.Count + "-" + saveTime + ".csv";
        Utils.DoFileOutputLine(savePath, saveFilename, outputlist);
    }

	bool tempCheck = true;
	public override void UpdateExpLogic ()
	{
		base.UpdateExpLogic ();
		ProcessInput ();
		ProcessLogic ();
	}

	public enum PIC_STATUS
	{
		PIC_RIGHT = 0,
		PIC_WRONG = 1
	}

	public enum ANSWER_STATUS
	{
		ANS_RIGHT = 0,
		ANS_WRONG = 1,
		ANS_TIMEOUT = 2
	}

	public enum EXP5_STATUS
	{
		EXP_STOP,
		EXP_INIT,
		EXP_PAUSE,
		EXP_INTRO,
		EXP_PRACTICE,
		EXP_RUN,
		EXP_OVER
	}
	
	public void ChangeLeftPic(string path)
	{
		leftPic.gameObject.SetActive (true);
		leftPic.SetPic (path);
		leftPic.panel.transform.position += new Vector3 (3f, 0, 0);
	}

	public void ChangeRightPic(string path)
	{
		rightPic.gameObject.SetActive (true);
		rightPic.SetPic (path);
		rightPic.panel.transform.position += new Vector3 (-3f, 0, 0);
	}

	public string RespondPicPath(PIC_STATUS picStatus, ANSWER_STATUS answerStatus)
	{
		string res = null;
		if (picStatus == PIC_STATUS.PIC_RIGHT && answerStatus == ANSWER_STATUS.ANS_RIGHT) 
		{
			res = "Pics/TaskR/right_f";
		}
		else if(picStatus == PIC_STATUS.PIC_RIGHT && answerStatus == ANSWER_STATUS.ANS_WRONG)
		{
			res = "Pics/TaskR/wrong_f";
		}
		else if(picStatus == PIC_STATUS.PIC_RIGHT && answerStatus == ANSWER_STATUS.ANS_TIMEOUT)
		{
			res = "Pics/TaskR/none_f";
		}
		else if(picStatus == PIC_STATUS.PIC_WRONG && answerStatus == ANSWER_STATUS.ANS_RIGHT)
		{
			res = "Pics/TaskR/right_j";
		}
		else if(picStatus == PIC_STATUS.PIC_WRONG && answerStatus == ANSWER_STATUS.ANS_WRONG)
		{
			res = "Pics/TaskR/wrong_j";
		}
		else if(picStatus == PIC_STATUS.PIC_WRONG && answerStatus == ANSWER_STATUS.ANS_TIMEOUT)
		{
			res = "Pics/TaskR/none_j";
		}
		return res;
	}

	private void InitPrefab()
	{
        // leftPic
        UnityEngine.Object prefab1 = Resources.Load ("Prefabs/Exp5Pic");
		GameObject go1 = GameObject.Instantiate (prefab1) as GameObject;
		leftPic = go1.GetComponent<UISprite> ();
		PanelData pd1 = new PanelData ();
		pd1.Init (600, 600, 300, 0, 1);
		leftPic.InitWithPic (pd1, null, "Pics/TaskR/1_x_15_a");
		leftPic.gameObject.SetActive (false);

        // rightPic
        UnityEngine.Object prefab2 = Resources.Load ("Prefabs/Exp5Pic");
		GameObject go2 = GameObject.Instantiate (prefab1) as GameObject;
		rightPic = go2.GetComponent<UISprite> ();
		PanelData pd2 = new PanelData ();
		pd2.Init (600, 600, -300, 0, 1);
		rightPic.InitWithPic (pd2, null, "Pics/TaskR/1_x_15_b");
		rightPic.gameObject.SetActive (false);

        UnityEngine.Object prefab3 = Resources.Load ("Prefabs/Exp5Pic");
		GameObject go3 = GameObject.Instantiate (prefab1) as GameObject;
		popoutPic = go3.GetComponent<UISprite> ();
		PanelData pd3 = new PanelData ();
		pd3.Init (0, 0, 0, 0, 2);
		popoutPic.InitWithPic (pd3, null, "Pics/TaskR/ST_3D");
		popoutPic.gameObject.SetActive (false);
	}

	private void InitRan()
	{
		ran8x = new RanGenExp5 ();
		ran8x.InitExp5 ("8", "x");
		ran8z = new RanGenExp5 ();
		ran8z.InitExp5 ("8", "z");
		ran9x = new RanGenExp5 ();
		ran9x.InitExp5 ("9", "x");
		ran9z = new RanGenExp5 ();
		ran9z.InitExp5 ("9", "z");

		ran1xaa = new RanGenExp5 ();
		ran1xaa.InitExp5Prac ("1", "x", "a", "a");
		ran1xab = new RanGenExp5 ();
		ran1xab.InitExp5Prac ("1", "z", "a", "b");
		ran1xba = new RanGenExp5 ();
		ran1xba.InitExp5Prac ("1", "x", "b", "a");
		ran1xbb = new RanGenExp5 ();
		ran1xbb.InitExp5Prac ("1", "x", "b", "b");
		ran1zaa = new RanGenExp5 ();
		ran1zaa.InitExp5Prac ("1", "z", "a", "a");
		ran1zab = new RanGenExp5 ();
		ran1zab.InitExp5Prac ("1", "z", "a", "b");
		ran1zba = new RanGenExp5 ();
		ran1zba.InitExp5Prac ("1", "z", "b", "a");
		ran1zbb = new RanGenExp5 ();
		ran1zbb.InitExp5Prac ("1", "z", "b", "b");

	}

	public void ShowIntro()
	{
		ShowPopout ("Pics/TaskR/ST_3D", 0, 0, 99999);
	}

	protected PIC_STATUS currentPicCombStatus;
	protected ANSWER_STATUS currentAnswerStatus;

	bool ansGetFlag = false;
	bool ansCheckFlag = false;
	bool timeWarningFlag = false;
	bool answerVal;

	public void ShowRes()
	{
		string picPath = RespondPicPath (currentPicCombStatus, currentAnswerStatus);
		ShowPopout (picPath, 0, 0, resPopoutTime);
	}

    public void SaveData(int taskno, string leftimg, string rightimg, int iscoincide, int keypress, int isright, double reacttime)
    {
        List<string> savelist = new List<string>();
        savelist.Add(taskno.ToString());
        savelist.Add(leftimg);
        savelist.Add(rightimg);
        savelist.Add(iscoincide.ToString());
        savelist.Add(keypress.ToString());
        savelist.Add(isright.ToString());
        savelist.Add(reacttime.ToString("f1"));

        Utils.DoFileOutputLine(savePath, saveFilename, savelist);
    }

	float pointTimeCount2;
	bool pointShowFlag = false;
	float ansResShowCount;
	float ansResTime;
	bool ansResFlag = false;
	bool showFlag = false;
	int practiceShowCount = 4;
	bool gotoRunFlag = false;
	bool gotoRunTipFlag = false;
	ANSWER_STATUS tempPressCon;
	GameObject toRunPopout;
	public void ProcessLogic()
	{
		if(currentExpStatus == EXP5_STATUS.EXP_INIT)
		{
			waitTime -= Utils.GetDeltaTime ();
			if (waitTime < 0f) 
			{
				currentExpStatus = EXP5_STATUS.EXP_INTRO;
				introPopoutFlag = true;
				waitTime = 99999f;
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_INTRO)
		{
			if (introPassFlag) 
			{
				ClosePopout ();
				introPopoutFlag2 = true;
				introPassFlag = false;
			}
			if (introPopoutFlag) 
			{
				ShowPopout ("Pics/TaskR/ST_3D", 0, 0, 99999);
				introPopoutFlag = false;
			}

			if (introPopoutFlag2) 
			{
				introPopoutFlag2 = false;
				ShowPopout("Pics/TaskR/cross", 0, 0, pointTime);
				waitTime = pointTime;
			}

			waitTime -= Utils.GetDeltaTime ();
			if (waitTime < 0) 
			{
				currentExpStatus = EXP5_STATUS.EXP_PRACTICE;
				ResetParaForNext ();
				popoutPic.gameObject.SetActive (false);
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PAUSE)
		{

		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PRACTICE)
		{
			if (ansResFlag) 
			{
				ansResFlag = false;
				string path;
				if (CheckSameShape (currentLeftPicName, currentRightPicName))
					path = RespondPicPath (PIC_STATUS.PIC_RIGHT, tempPressCon);
				else
					path = RespondPicPath (PIC_STATUS.PIC_WRONG, tempPressCon);
					
				ShowPopout (path, 0, -200, 3f);
				ansResShowCount = 3f;
			}
			ansResShowCount -= Utils.GetDeltaTime ();
			if (ansResShowCount > 0)
				return;
			if (gotoRunTipFlag) 
			{
				gotoRunTipFlag = false;
				ClearUILeaveBackground ();
				toRunPopout = PopoutWithText ("请按空格键进入正式任务", 99999, 0, 0);
			}
			if (gotoRunFlag) 
			{
				gotoRunFlag = false;
				toRunPopout.SetActive (false);
				currentExpStatus = EXP5_STATUS.EXP_RUN;
				ResetParaForNext ();
			}
			if (checkGotoRunFlag)
				return;
			if (pointShowFlag) 
			{
				pointShowFlag = false;
				leftPic.gameObject.SetActive (false);
				rightPic.gameObject.SetActive (false);
				ShowPopout ("Pics/TaskR/cross", 0, 0, pointTime);
				return;
			}
			pointTimeCount2 -= Utils.GetDeltaTime ();
			if (pointTimeCount2 > 0)
				return;
			if (showFlag) 
			{
				showFlag = false;
				ansGetFlag = true;
				ShowNextPractice ();
				practiceShowCount--;
			}
			if (ansCheckFlag) 
			{
				ansCheckFlag = false;
				RecordAns ();
				if (practiceShowCount <= 0) 
				{
					ansResFlag = true;
					checkGotoRunFlag = true;
					gotoRunTipFlag = true;
				}
				else
				{
					//TODO: reset time count-down etc...
					ResetParaForNext();
					ansResFlag = true;
				}
				return;
			}

			timeCountdown -= Utils.GetDeltaTime ();
			if (timeCountdown > 0 && timeCountdown < lastTime) 
			{
				ShowCountDown (timeCountdown);
			}
			else if (timeCountdown < 0) 
			{
				RecordAns ();
				if (practiceShowCount <= 0) 
				{
					ansResFlag = true;
					checkGotoRunFlag = true;
					gotoRunTipFlag = true;
				}
				else
				{
					//TODO: reset time count-down etc...
					ResetParaForNext();
					ansResShowCount = ansResTime;
					ansResFlag = true;
				}
			}

		}
		else if(currentExpStatus == EXP5_STATUS.EXP_RUN)
		{
			if (ansResFlag) 
			{
				ansResFlag = false;
				PopoutWithText ("限定时间内没有作答", ansResTime, 0, -100);
				if(CheckSameShape(currentLeftPicName, currentRightPicName))
					PopoutWithText ("左右两幅图片旋转后能够重合，应按F键", ansResTime, 0, -200);
				else
					PopoutWithText ("左右两幅图片旋转后不能重合，应按J键", ansResTime, 0, -200);
			}
			ansResShowCount -= Utils.GetDeltaTime ();
			if (ansResShowCount > 0)
				return;
			if (pointShowFlag) 
			{
				pointShowFlag = false;
				leftPic.gameObject.SetActive (false);
				rightPic.gameObject.SetActive (false);
				ShowPopout ("Pics/TaskR/cross", 0, 0, pointTime);
				return;
			}
			pointTimeCount2 -= Utils.GetDeltaTime ();
			if (pointTimeCount2 > 0)
				return;
			if (showFlag) 
			{
				showFlag = false;
				ansGetFlag = true;
				ShowNext ();
			}
			if (ansCheckFlag) 
			{
				ansCheckFlag = false;
				RecordAns ();
				if (IsFinished ()) 
				{
					EndExp ();
				}
				else
				{
					//TODO: reset time count-down etc...
					ResetParaForNext();
				}
				return;
			}

			timeCountdown -= Utils.GetDeltaTime ();
			if (timeCountdown > 0 && timeCountdown < lastTime) 
			{
				ShowCountDown (timeCountdown);
			}
			else if (timeCountdown < 0) 
			{
				RecordAns ();
				if (IsFinished ()) 
				{
					EndExp ();
				}
				else
				{
					//TODO: reset time count-down etc...
					ResetParaForNext();
					ansResShowCount = ansResTime;
					ansResFlag = true;
				}
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_OVER)
		{
			ClearUILeaveBackground ();
			if (overPicFlag) 
			{
				overPicFlag = false;
				PopoutWithText ("该任务完成, 按任意键结束", 99999, 0, 100f);
			}
		}
	}

	bool introPassFlag = false;
	bool introPassFlag2 = false;
	bool isFPressed = false;
	bool isJPressed = false;
	bool checkGotoRunFlag = false;
	public void ProcessInput()
	{
		if(currentExpStatus == EXP5_STATUS.EXP_INIT)
		{
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_INTRO)
		{
			if (Input.GetButtonDown ("Button8") && !introPopoutFlag)
			{
				introPassFlag = true;
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PAUSE)
		{
			
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PRACTICE)
		{
			if (ansGetFlag) 
			{
				if (Input.GetButtonDown ("ButtonF")) 
				{
					Debug.Log ("yeah!");
					answerVal = true;
					ansCheckFlag = true;
					ansGetFlag = false;
					isFPressed = true;
				} 
				else if (Input.GetButtonDown ("ButtonJ"))
				{
					Debug.Log ("No");
					answerVal = false;
					ansCheckFlag = true;
					ansGetFlag = false;
					isJPressed = true;
				}
			}
			if (checkGotoRunFlag) 
			{
				if (Input.GetButtonDown ("Button8")) 
				{
					checkGotoRunFlag = false;
					gotoRunFlag = true;
				}
					
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_RUN)
		{
			if (ansGetFlag) 
			{
				if (Input.GetButtonDown ("ButtonF")) 
				{
					Debug.Log ("yeah!");
					answerVal = true;
					ansCheckFlag = true;
					ansGetFlag = false;
					isFPressed = true;
				} 
				else if (Input.GetButtonDown ("ButtonJ"))
				{
					Debug.Log ("No");
					answerVal = false;
					ansCheckFlag = true;
					ansGetFlag = false;
					isJPressed = true;
				}
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_OVER)
		{
			if (Input.anyKeyDown)
				ExpManager.instance.GotoNextExp ();
		}
	}	

	public bool IsFinished()
	{
		return ran8x.IsFinished () && ran9x.IsFinished () && ran8z.IsFinished () && ran9z.IsFinished ();
	}

	protected bool eightNineFlag;// true: show 8,  false: show 9
	// bool return value, true: still something left, false: all done

	private string currentLeftPicName;
	private string currentRightPicName;
	public bool ShowNext()
	{
		//TODO
		if (IsFinished())
			return true;
		Debug.Log (ran8x.IsFinished () + "_" + ran8z.IsFinished ()  + "_" + ran9x.IsFinished ()  + "_" + ran9z.IsFinished ());

		if (eightNineFlag)
		{
			eightNineFlag = !eightNineFlag;
			bool is8x;
			if (ran8x.IsFinished ())
				is8x = false;
			else if (ran8z.IsFinished ())
				is8x = true;
			else
				is8x = ((int)UnityEngine.Random.Range (0, 2) == 1);
			if (is8x) 
				ChangePicByRanGen (ran8x);
			else
				ChangePicByRanGen (ran8z);
		}
		else
		{
			eightNineFlag = !eightNineFlag;
			bool is9x;
			if (ran9x.IsFinished ())
				is9x = false;
			else if (ran9z.IsFinished ())
				is9x = true;
			else
				is9x = ((int)UnityEngine.Random.Range (0, 2) == 1);
			if (is9x) 
				ChangePicByRanGen (ran9x);
			else
				ChangePicByRanGen (ran9z);
		}
		return false;
	}


	RanGenExp5 ran1xaa;
	RanGenExp5 ran1xab;
	RanGenExp5 ran1xbb;
	RanGenExp5 ran1xba;
	RanGenExp5 ran1zaa;
	RanGenExp5 ran1zab;
	RanGenExp5 ran1zba;
	RanGenExp5 ran1zbb;
	bool corOrWroFlag;
	public bool ShowNextPractice()
	{
		if (corOrWroFlag)
		{
			corOrWroFlag = !corOrWroFlag;
			int sele = (int)UnityEngine.Random.Range (0, 4);
			if (sele == 0)
				ChangePicByRanGen (ran1xaa);
			else if (sele == 1)
				ChangePicByRanGen (ran1xbb);
			else if (sele == 2)
				ChangePicByRanGen (ran1zaa);
			else if (sele == 3)
				ChangePicByRanGen (ran1zbb);
		}
		else
		{
			corOrWroFlag = !corOrWroFlag;
			int sele = (int)UnityEngine.Random.Range (0, 4);
			if (sele == 0)
				ChangePicByRanGen (ran1xab);
			else if (sele == 1)
				ChangePicByRanGen (ran1xba);
			else if (sele == 2)
				ChangePicByRanGen (ran1zab);
			else if (sele == 3)
				ChangePicByRanGen (ran1zba);
		}
		return false;
	}

	int tempTimeFloor;
	GameObject countObject;
	public void ShowCountDown(float time)
	{
		int tmNow = Mathf.FloorToInt (time);
		if (tmNow < tempTimeFloor) 
		{
			tempTimeFloor--;
			if (countObject != null)
				Destroy (countObject);
			countObject = PopoutWithText ("还剩" + tmNow + "秒", 99999, 0, 100f);
		}
	}

	public void ResetParaForNext()
	{
		if (!isFPressed && !isJPressed)
			tempPressCon = ANSWER_STATUS.ANS_TIMEOUT;
		else if (isFPressed && !isJPressed)
			tempPressCon = ANSWER_STATUS.ANS_RIGHT;
		else
			tempPressCon = ANSWER_STATUS.ANS_WRONG;
		ansCheckFlag = false;
		pointShowFlag = true;
		isFPressed = false;
		isJPressed = false;
		showFlag = true;
		timeCountdown = viewTime;
		pointTimeCount2 = pointTime;
		ansResFlag = false;
		tempTimeFloor = Mathf.CeilToInt (lastTime);
		Destroy (countObject);
	
	}

	public override void ClearUI ()
	{
		ClearUILeaveBackground ();
		background.gameObject.SetActive (false);
	}

	public void ClearUILeaveBackground()
	{
		leftPic.gameObject.SetActive (false);
		rightPic.gameObject.SetActive (false);
	}

	void ChangePicByRanGen(RanGenExp5 ran)
	{
		List<string> nextStr = Utils.SplitString(ran.GetNextRanVal(), ",");
		currentLeftPicName = nextStr [0];
		currentRightPicName = nextStr [1];
		ChangeLeftPic ("Pics/TaskR/" + currentLeftPicName);
		ChangeRightPic ("Pics/TaskR/" + currentRightPicName);
	}

	public bool CheckSameShape(string leftPicName, string rightPicName)
	{
		if (leftPicName == null || rightPicName == null || leftPicName.Length == 0 || rightPicName.Length == 0)
			return false;
		return (leftPicName [leftPicName.Length - 1] == rightPicName [rightPicName.Length - 1]);
	}

	bool overPicFlag = false;
	public override void EndExp ()
	{
		base.EndExp ();
		currentExpStatus = EXP5_STATUS.EXP_OVER;
		overPicFlag = true;
	}

	public void RecordAns()
	{
        //TODO
        //SaveData(int taskno, string leftimg, string rightimg, int iscoincide, int keypress, int isright, double reacttime)
        /*序号：练习阶段为1,2,34，正式实验阶段为1,2,3…62,63,64。
左侧图片：左侧图片的文件名称
右侧图片：右侧图片的文件名称
能否重合：左侧图片和右侧图片能否经过旋转后重合（0能，1不能 ）
反应按键：按键为F则存储为0，按键为J则存储为1，超时则存储为-1
是否正确：“能否重合”=“按键情况”则存储为1，否则存储为0
反应时：图片呈现到被试按键的时间（单位毫秒）保留小数点后一位小数（例如2000.0），如果超时则存储为-1
*/  
        int iscoincide;
        int keypress = -1;
        int isright;

        taskno++;
        iscoincide = (CheckSameShape(currentLeftPicName, currentRightPicName) == true ? 0 : 1);
        if (isFPressed)
        {
            keypress = 0;
        }
        else if (isJPressed)
        {
            keypress = 1;
        }
        isright = ((iscoincide == keypress) ? 1 : 0);

    }
}

