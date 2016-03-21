using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Exp4 : ExpObject
{
	Color ballColor = Color.blue;
	Color backgroundColor;
	Color blockerColor = Color.green;
   
	float ballPixelRad = 10;
	float blockerPixelRad = 50;
	float ballToCenterDistance = 300;


	float ballSpeed1;
	float ballSpeed2;
	float ballSpeed3;
	List<float> ballSpeedList = new List<float>{50, 100, 150};

	bool biasFeedbackFlag = true;

	int repeatTime = 1;
	float waitTimeInBetween = 2f;

    Vector2 destPoint = new Vector2(0,0);

	List<bool> posList = new List<bool>{true, true, true, true};

	enum INIT_POS
	{
		LEFT = 0,
		RIGHT = 1,
		UP = 2,
		DOWN = 3
	}

	public void InitPara()
	{
		ballColor = new Color ((float)config.config4.ballColorR/255f, (float)config.config4.ballColorG/255f, (float)config.config4.ballColorB/255f);
		backgroundColor = new Color ((float)config.config4.backgroundColorR/255f, (float)config.config4.backgroundColorG/255f, (float)config.config4.backgroundColorB/255f);
		blockerColor = new Color ((float)config.config4.shelterColorR/255f, (float)config.config4.shelterColorG/255f, (float)config.config4.shelterColorB/255f);
		ballPixelRad = (float)config.config4.ballRadius;
		blockerPixelRad = (float)config.config4.shelterRadius;
		ballToCenterDistance = (float)config.config4.ballCenterDis;
		ballSpeed1 = config.config4.speed1;
		ballSpeed2 = config.config4.speed2;
		ballSpeed3 = config.config4.speed3;
		ballSpeedList = new List<float>{ ballSpeed1, ballSpeed2, ballSpeed3 };
		biasFeedbackFlag = config.config4.feedback;
		repeatTime = config.config4.repeatNum;
		waitTimeInBetween = config.config4.timeInterval;
		posList = new List<bool>{ config.config4.left, config.config4.right, config.config4.up, config.config4.down };

	}

	//-------------------

	public EXP4_STATUS currentExpStatus;
	protected float waitTime;
	protected int runCount;
	protected int repeatCount;

	protected GameObject blockerPic;
	protected GameObject ballPic;

	protected RanGen ranEx4;

	protected string currentRunSetting;
	protected int currentRepeatNum;
	protected string currentStartPos;
	protected int currentSpeedLvl;

	protected float currentBallSpeed;
	protected Vector3 currentBallSpeedVec;
	protected float ballLayer = 4f;
	protected float blockerLayer = 30f;

	protected string saveTime;
    protected DateTime startTime;
    protected DateTime pressTime;
    protected DateTime disappearTime;
	protected DateTime taskstartTime;
	protected DateTime taskendTime;

	protected string savePath;
	protected string saveFilename;
	protected int taskno = 0;

    BALL_BLOCKER_RELATION ballPosCheck;
	//-------------------

	public override void Init ()
	{
		base.Init ();
		InitPara ();
		InitPrefab ();
		InitRan ();
        InitOutput ();
		taskstartTime = DateTime.Now;

		int tempCount = 0;
		foreach (bool bl in posList) 
		{
			if(bl)
				tempCount++;
		}
		repeatCount = repeatTime * (tempCount) * 3;
		Debug.Log (repeatCount);
	}

    void InitOutput()
    {
        List<string> outputlist = new List<string>();
		outputlist = new List<string> { "测试序号", "速度", "运动方向", "运动开始时间", "消失时间", "按键时间", "可见运动时间", "进入遮挡物到按键时间", "实际时间(毫秒)", "估计时间(毫秒)", "偏差率", "起始坐标", "目标坐标", "按键坐标", "实验开始时间", "实验结束时间", "实验用时" };

		saveTime = DateTime.Now.ToString("yyyyMMddHHmmss");
		savePath = Utils.MakeDirectoy("Data\\" + ExpManager.tester.Id + "-" + ExpManager.tester.Name);
		saveFilename = "T4-" + config.sortId + "-任务4-速度知觉能力测试-" + ExpManager.tester.Id + "-" + ExpManager.tester.Name + "-" + ExpManager.tester.Count + "-" + saveTime + ".csv";
		Utils.DoFileOutputLine(savePath, saveFilename, outputlist);
    }

	public override void UpdateExpLogic ()
	{
		base.UpdateExpLogic ();
		ProcessInput ();
		ProcessLogic ();
	}


	bool roundInitFlag = false;
	bool checkBallFlag = false;
	bool overPicFlag = false;

	void ProcessInput()
	{
		if(currentExpStatus == EXP4_STATUS.EXP_INIT)
		{
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_INTRO)
		{
			if(Input.GetButtonDown("Button8"))
			{
				currentExpStatus = EXP4_STATUS.EXP_RUN;
				popoutPic.gameObject.SetActive(false);
				roundInitFlag = true;
			}
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_PAUSE)
		{

		}
		else if(currentExpStatus == EXP4_STATUS.EXP_PRACTICE)
		{

		}
		else if(currentExpStatus == EXP4_STATUS.EXP_RUN)
		{
			if(Input.GetButtonDown("Button8"))
			{
				checkBallFlag = true;
                pressTime = DateTime.Now;
            }
			if (Input.GetButtonDown ("EscMenu")) 
			{
				EndExp ();
			}
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_OVER)
		{
			if (Input.anyKeyDown)
				ExpManager.instance.GotoNextExp ();
		}
	}

	void ProcessLogic()
	{
		if(currentExpStatus == EXP4_STATUS.EXP_INIT)
		{
			waitTime -= Utils.GetDeltaTime();
			if(waitTime < 0)
			{
				currentExpStatus = EXP4_STATUS.EXP_INTRO;
			}
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_INTRO)
		{
			if(showIntroFlag)
			{
				showIntroFlag = false;
				ShowIntro();
			}
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_PAUSE)
		{

		}
		else if(currentExpStatus == EXP4_STATUS.EXP_PRACTICE)
		{

		}
		else if(currentExpStatus == EXP4_STATUS.EXP_RUN)
		{
			if(waitTimeCount > 0)
			{
				waitTimeCount -= Utils.GetDeltaTime();
				return;
			}
			if(roundInitFlag)
			{
				roundInitFlag = false;
				if(repeatCount <= 0)
				{
					EndExp ();
					return;
				}
				repeatCount --;
				ballPic.SetActive(true);
				blockerPic.SetActive(true);
				SetCurrentRun();
				Debug.Log (currentRunSetting);
				return;
			}
			ballPic.transform.position += currentBallSpeedVec * Utils.GetDeltaTime();

			ballPosCheck =  CheckBallBlockerRelation();
			if(checkBallFlag)
			{
				checkBallFlag = false;
				if(ballPosCheck == BALL_BLOCKER_RELATION.IN)
				{
					ShowCurrentRunRes();
					roundInitFlag = true;
				}
			}
			if(ballPosCheck == BALL_BLOCKER_RELATION.OUT)
			{}
			else if(ballPosCheck == BALL_BLOCKER_RELATION.IN)
			{}
			else if(ballPosCheck == BALL_BLOCKER_RELATION.INTERSECT)
			{}
			else
			{
                pressTime = DateTime.Now;
				ShowCurrentRunRes();
				Debug.Log ("timeOut");
				roundInitFlag = true;
			}
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_OVER)
		{
            ClearUILeaveBackground();
			if (overPicFlag) 
			{
				overPicFlag = false;
				PopoutWithText ("该任务完成, 按任意键结束", 99999, 0, 100f);
			}
		}	
	}

	bool showIntroFlag = true;
	public void ShowIntro()
	{
		ShowPopout ("Pics/Inst/Speed_Sensor", 0, 0, 99999);
	}

	public void InitPrefab()
	{
		InitBlocker ();// layer z = 5
		InitBall ();
		background.GetComponentInChildren<Renderer> ().material.color = backgroundColor;
	}

	public void InitBlocker()
	{
		UnityEngine.Object prefab1 = Resources.Load ("Prefabs/Exp4Blocker");
		blockerPic = GameObject.Instantiate (prefab1) as GameObject;
		GameObject sphere = blockerPic.transform.FindChild("Sphere").gameObject;
		sphere.transform.localScale = new Vector3 (blockerPixelRad / 50f, blockerPixelRad / 50f, blockerPixelRad / 50f);
		MeshRenderer meshRen = sphere.GetComponent<MeshRenderer> ();
		meshRen.material.color = blockerColor;
		blockerPic.transform.position = new Vector3 (0, 0, blockerLayer);
		blockerPic.SetActive (false);
	}

	public void InitBall()
	{
		UnityEngine.Object prefab1 = Resources.Load ("Prefabs/Exp4Ball");
		ballPic = GameObject.Instantiate (prefab1) as GameObject;
		GameObject sphere = ballPic.transform.FindChild("Sphere").gameObject;
		sphere.transform.localScale = new Vector3 (ballPixelRad / 50f, ballPixelRad / 50f, ballPixelRad / 50f);
		MeshRenderer meshRen = sphere.GetComponent<MeshRenderer> ();
		meshRen.material.color = ballColor;
		ballPic.transform.position = new Vector3 (0, 0, ballLayer);
		ballPic.SetActive (false);
	}

	public void InitRan()
	{
		List<string> str1 = new List<string> ();
		for (int i = 0; i < repeatTime; i++) 
		{
			str1.Add("repeat" + i);
		}
		List<string> str2 = new List<string>{"speed1", "speed2", "speed3"};
		List<string> str3 = new List<string> ();
		if (posList [0])
			str3.Add ("left");
		if (posList [1])
			str3.Add ("right");
		if (posList [2])
			str3.Add ("up");
		if (posList [3])
			str3.Add ("down");
		List<List<string>> strLiLi = new List<List<string>>{str1, str2, str3};

		ranEx4 = new RanGen ();
		ranEx4.Init (strLiLi, "_");
	}

	public void SetCurrentRun()
	{
        startTime = DateTime.Now;

		currentRunSetting = ranEx4.GetNextRanVal();
		string[] splitRes = currentRunSetting.Split ('_');
		string str1 = splitRes [0];
		string str2 = splitRes [1];
		string str3 = splitRes [2];
		currentRepeatNum = int.Parse (str1.Substring (6));
		currentSpeedLvl = int.Parse (str2.Substring (str2.Length - 1));
		currentBallSpeed = ballSpeedList [currentSpeedLvl - 1];
		currentStartPos = str3;
		if (str3 == "left") 
		{
			ballPic.transform.position = new Vector3 (ballToCenterDistance / 100f, 0, ballLayer);
			currentBallSpeedVec = new Vector3(-currentBallSpeed/100f, 0, 0);
		}
		else if (str3 == "right")
		{
			ballPic.transform.position = new Vector3 (-ballToCenterDistance / 100f, 0, ballLayer);
			currentBallSpeedVec = new Vector3(currentBallSpeed/100f, 0, 0);
		}
		else if (str3 == "up")
		{
			ballPic.transform.position = new Vector3 (0, ballToCenterDistance / 100f, ballLayer);
			currentBallSpeedVec = new Vector3(0, -currentBallSpeed/100f, 0);
		}
		else if (str3 == "down")
		{
			ballPic.transform.position = new Vector3 (0, -ballToCenterDistance / 100f, ballLayer);
			currentBallSpeedVec = new Vector3(0, currentBallSpeed/100f, 0);
		}

		Debug.Log (currentRepeatNum + "_" + currentSpeedLvl + "_" + currentStartPos);

	}

	public BALL_BLOCKER_RELATION CheckBallBlockerRelation()
	{
		Vector2 v1 = new Vector2 (ballPic.transform.position.x, ballPic.transform.position.y);
		Vector2 v2 = new Vector2 (blockerPic.transform.position.x, blockerPic.transform.position.y);
		float distance = Vector2.Distance (v1, v2);
		if (distance < blockerPixelRad / 100f)
			return BALL_BLOCKER_RELATION.IN;
		else if (distance <= (ballPixelRad + blockerPixelRad) / 100f) 
		{
			if((v1.x > v2.x && currentBallSpeedVec.x < 0)
				||(v1.x < v2.x && currentBallSpeedVec.x > 0)
				||(v1.y > v2.y && currentBallSpeedVec.y < 0)
				||(v1.y < v2.y && currentBallSpeedVec.y > 0))
				return BALL_BLOCKER_RELATION.INTERSECT;
			else
				return BALL_BLOCKER_RELATION.INTERSECT_PASSED;
		}
		else if((v1.x > v2.x && currentBallSpeedVec.x < 0)
			||(v1.x < v2.x && currentBallSpeedVec.x > 0)
			||(v1.y > v2.y && currentBallSpeedVec.y < 0)
			||(v1.y < v2.y && currentBallSpeedVec.y > 0))
			return BALL_BLOCKER_RELATION.OUT;
		else
			return BALL_BLOCKER_RELATION.PASSED;

		return BALL_BLOCKER_RELATION.PASSED;
	}

	public enum BALL_BLOCKER_RELATION
	{
		IN, //ball center enter blocker circle
		INTERSECT, // no enter, but intersect
		INTERSECT_PASSED,//
		OUT,// before intersect and out
		PASSED// crossed and out
	}

	float waitTimeCount;
	public void ShowCurrentRunRes()
	{        
		ballPic.transform.position = new Vector3 (ballPic.transform.position.x, ballPic.transform.position.y, blockerLayer + 20f);
		waitTimeCount = waitTimeInBetween;
		if (ballPosCheck == BALL_BLOCKER_RELATION.IN) 
		{
			if (biasFeedbackFlag)
				PopoutWithText ("偏差率" + (CalCorrectRate()*100).ToString("f2")+"%", waitTimeInBetween, 0, 220f);
		}
		else
		{
            if (biasFeedbackFlag)
                PopoutWithText ("反应超时！", waitTimeInBetween, 0, 100f);
		}

		Vector2 startP = new Vector2(0, 0);
        string movedirection="";
        switch(currentStartPos)
        {
            case "left":
				movedirection = "水平向右";
				startP = new Vector2(-ballToCenterDistance, 0);
                break;
            case "right":
				movedirection = "水平向左";
				startP = new Vector2(ballToCenterDistance, 0);
                break;
            case "up":
				movedirection = "竖直向下";
				startP = new Vector2(0, ballToCenterDistance);
                break;
            case "down":
				movedirection = "竖直向上";
				startP = new Vector2(0, -ballToCenterDistance);
                break;
            default:
                break;
        }        

        float movetime, reacttime, exacttime, estimatetime;
        movetime = (ballToCenterDistance - blockerPixelRad) / currentBallSpeed;
        disappearTime = startTime.Add(new TimeSpan(0, 0, 0, (int)movetime));

        Vector2 p1 = Utils.GetV2FromV3(ballPic.transform.position);
        Vector2 p2 = Utils.GetV2FromV3(blockerPic.transform.position);
        float dis1 = Vector2.Distance(p1, p2);
        reacttime = (blockerPixelRad - dis1) / currentBallSpeed;
        exacttime = reacttime + movetime;
        estimatetime = blockerPixelRad / currentBallSpeed;

		taskno++;
		SaveData (taskno, (int)currentBallSpeed ,movedirection,startTime,disappearTime,pressTime,movetime*1000,reacttime*1000,exacttime*1000,estimatetime*1000, CalCorrectRate(), startP, p1);

	}

	public float CalCorrectRate()
	{
		if (ballPosCheck == BALL_BLOCKER_RELATION.IN) 
		{
			Vector2 p1 = Utils.GetV2FromV3 (ballPic.transform.position);
			Vector2 p2 = Utils.GetV2FromV3 (blockerPic.transform.position);
			Vector2 dp = p1 - p2;
			float dis = 0;
			dis = (dp.x == 0 ? dp.y : dp.x);
            if ((currentStartPos=="up")|| (currentStartPos == "left"))
            {
                dis = -dis;
            }
            return dis / blockerPixelRad * 100f;
		}
		else
			return -1;
	}

    public void SaveData(int taskno, int speed, string movedirection, DateTime starttime, DateTime disappartime, DateTime presstime, double movetime, double reacttime, double exacttime, double estimatetime, double errorratio, Vector2 startpoint, Vector2 presspoint)
	{
        List<string> savelist = new List<string>();
        //savelist.Add();
        savelist.Add(taskno.ToString());
        savelist.Add(speed.ToString("f0"));
        savelist.Add(movedirection);
		savelist.Add(starttime.ToString("HH:mm:ss"));
		savelist.Add(disappartime.ToString("HH:mm:ss"));
		savelist.Add(presstime.ToString("HH:mm:ss"));
        savelist.Add(movetime.ToString("f1"));
        savelist.Add(reacttime.ToString("f1"));
        savelist.Add(exacttime.ToString("f1"));
        savelist.Add(estimatetime.ToString("f1"));
		if (errorratio == -1)
		{
			savelist.Add(errorratio.ToString("f0"));
		}
		else
		{
			savelist.Add(errorratio.ToString("f3"));
		}
		savelist.Add("{" + (startpoint.x).ToString("f0") + ";" + (startpoint.y).ToString("f0") + "}");
		savelist.Add("{" + (destPoint.x * 100).ToString("f0") + ";" + (destPoint.y * 100).ToString("f0") + "}");
		savelist.Add("{" + (presspoint.x * 100).ToString("f0") + ";" + (presspoint.y * 100).ToString("f0") + "}");

		int tempCount = 0;
		foreach (bool bl in posList)
		{
			if (bl)
				tempCount++;
		}
		if (taskno== repeatTime * (tempCount) * 3)
		{
			taskendTime = DateTime.Now;
			TimeSpan ts = taskstartTime.Subtract(taskendTime).Duration();
			savelist.Add(taskstartTime.ToString("HH:mm:ss"));
			savelist.Add(taskendTime.ToString("HH:mm:ss"));
			int duration;
			duration = ts.Minutes * 60 + ts.Seconds;
			savelist.Add(duration.ToString());
		}

		Utils.DoFileOutputLine(savePath, saveFilename, savelist);
    }

	public enum EXP4_STATUS
	{
		EXP_INIT,
		EXP_PAUSE,
		EXP_INTRO,
		EXP_PRACTICE,
		EXP_RUN,
		EXP_OVER
	}
		
	public override void EndExp ()
	{
		base.EndExp ();
		currentExpStatus = EXP4_STATUS.EXP_OVER;
		overPicFlag = true;
	}
		
	public override void ClearUI ()
	{
		base.ClearUI ();
		Destroy (blockerPic);
		Destroy (ballPic);
	}

    public void ClearUILeaveBackground()
    {
        Destroy(blockerPic);
        Destroy(ballPic);
    }
}

