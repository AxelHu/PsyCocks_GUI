using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Exp3 : ExpObject
{
	//------------------ setting parameters
	Color backgroundColor;
	float runTime;
	int repeatTime;

	public TRAIL_SHAPE trailShape;
	public enum TRAIL_SHAPE
	{
		CIRCLE,
		EIGHT,
		ELLIPSE
	}

	public bool moveDirection;// true -- original false -- reversed
	public bool pauseEnabled;
	public float pauseTime;
	public int pauseFrequency;
	public MOVE_PATTERN movePattern;

	public enum MOVE_PATTERN
	{
		TRANS,
		TRAS_ROLL
	}

	public SPEED_MODE speedMode;
	public enum SPEED_MODE
	{
		CONSTANT,
		ACC
	}
	// for translation
	public bool controllerDirection;
	public float constantSpeed;
	public float accStartSpeed;
	public float accMinSpeed;
	public float accMaxSpeed;
	public float accValueMin;
	public float accValueMax;
	//for trans + roll
	public float rollMinSpeed;
	public float rollMaxSpeed;
	public float rollMinRange;
	public float rollMaxRange;

	public float errDistance;
	public float errAngle;

	public float aimerSpeed;
	//--------------------parameters used in exp3
	public EXP3_STATUS currentExpStatus;
	public string leftJoystickButtonName = "Horizontal";
	public string rightJoystickButtonName;

	public int repeatCount;
	public bool roundInitFlag = false;

	public bool roundStartPauseFlag = true;
	public bool checkRoundStartPauseFlag = false;
	public bool roundFinishFlag = false;
	public bool pauseTipFlag = false;
	public bool checkPauseTipFlag = false;
	public bool overPicFlag = false;
	public bool checkMovementFlag = false;

	public bool horiMoveFlag = false;
	public bool vertiMoveFlag = false;
	public float horiVal;
	public float vertiVal;

	public GameObject aimer;
	public GameObject target;

	public float aimerMoveX;
	public float aimerMoveY;

	public float targetMoveX;
	public float targetMoveY;


	public float circleRad = 3f;
	public float elliRadY = 2f;
	public float elliRadX = 4f;
	public float eightRad = 2f;

	public float totalTimePaused;
	public GameObject pauseTip;
	public bool targetPauseFlag;
	public float totalTimeExp3;
	public float totalTimeThisRun;
	//------------------
	// for side task
	public bool feedback;
	public bool mainTask;

	public bool buttonDownFlag;
	public bool checkGreenLightFlag;

	public float estiTimeThisRound;


	public float textTipTime = 2f;
	public float roundRestTime = 2f;

	public float planeRate;
	public float heliRate;
	public int plaHelPos;
	public int plaHelPic;
	public float timeBetween;
	public float timeBetweenMin;
	public float timeBetweenMax;

	public float showTime;
	public float waitForInputTime;

    protected int pointnum = 0;
    protected int eventnum = 0;

    protected System.DateTime startTime;
    protected System.DateTime sureTime;

    protected Vector2 lastObjPoint = new Vector2(3f, 0);
    protected Vector2 lastPostPoint = new Vector2(0, 0);
    protected double lastRotateAngle = 0;

    private string saveTime;
    private string savePath;
    private string saveTraceFilename;
    private string saveEventFilename;


    public GameObject textObject;

	public override void Init ()
	{
		base.Init ();
		InitPara ();
		InitPrefab ();
		InitRan ();

        if (config.config3.mainTest)
        {
            InitTraceOutput();
        }
        InitEventOutput();

    }

    void InitTraceOutput()
    {
        List<string> outputlist = new List<string>();
        outputlist = new List<string> { "PointNum", "PointTime", "ObjPointX", "ObjPointY", "PostPointX", "PostPointY", "Distance", "Hit", "ObjSpeedX", "ObjSpeedY", "PostSpeedX", "PostSpeedY" };

        saveTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        savePath = Utils.MakeDirectoy("Data\\" + ExpManager.tester.Id + "-" + ExpManager.tester.Name);
        saveTraceFilename = "T3-Trace-" + config.sortId + "-任务3-双任务模式突发事件反应时测试-" + ExpManager.tester.Id + "-" + ExpManager.tester.Name + "-" + ExpManager.tester.Count + "-" + saveTime + ".csv";
        Utils.DoFileOutputLine(savePath, saveTraceFilename, outputlist);
    }

    void InitEventOutput()
    {
        List<string> outputlist = new List<string>();
        outputlist = new List<string> { "EventNum", "EventType", "StartTime", "SureTime", "Event_RT", "ButtonNo", "Event_Acc" };

        saveTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        savePath = Utils.MakeDirectoy("Data\\" + ExpManager.tester.Id + "-" + ExpManager.tester.Name);
        saveEventFilename = "T3-Event-" + config.sortId + "-任务3-双任务模式突发事件反应时测试-" + ExpManager.tester.Id + "-" + ExpManager.tester.Name + "-" + ExpManager.tester.Count + "-" + saveTime + ".csv";
        Utils.DoFileOutputLine(savePath, saveEventFilename, outputlist);
    }


    public void InitPara()
	{
		if (config.config3.backgroundColor == "灰黑")
			backgroundColor = new Color (48f / 255f, 48f / 255f, 48f / 255f);
		else if (config.config3.backgroundColor == "蓝黑")
			backgroundColor = new Color (20f / 255f, 20f / 255f, 70f / 255f);
		else
			backgroundColor = new Color (48f / 255f, 48f / 255f, 48f / 255f);

		//runTime = config.config3.testTime;
		//repeatTime = config.config3.testNum;
		if (config.config3.moveTrail == "圆形")
			trailShape = TRAIL_SHAPE.CIRCLE;
		else if (config.config3.moveTrail == "椭圆形")
			trailShape = TRAIL_SHAPE.ELLIPSE;
		else
			trailShape = TRAIL_SHAPE.EIGHT;
		if (config.config3.direction == 0)
			moveDirection = true;
		else
			moveDirection = false;
		if (config.config3.ctrlDirection == 0)
			controllerDirection = true;
		else
			controllerDirection = false;
		//if (config.config2.moveMode == 0)
		//	movePattern = MOVE_PATTERN.TRANS;
		//else
		//	movePattern = MOVE_PATTERN.TRAS_ROLL;

		//pauseEnabled = config.config2.pause;
		//pauseFrequency = config.config2.pauseRate;
		if(pauseEnabled)
			pauseTime = 60f / pauseFrequency;
		if (config.config3.speedMode == 0)
			speedMode = SPEED_MODE.CONSTANT;
		else
			speedMode = SPEED_MODE.ACC;
		constantSpeed = (float)config.config3.speed / 100f;
		accStartSpeed = (float)config.config3.speed / 100f;
		accMinSpeed = (float)config.config3.minSpeed / 100f;
		accMaxSpeed = (float)config.config3.maxSpeed / 100f;
		accValueMin = (float)config.config3.minASpeed / 100f;
		accValueMax = (float)config.config3.maxASpeed / 100f;

		//aimerSpeed = (float)hdConfig.Speed;
		aimerSpeed = 150f/100f;
		errDistance = 50f;
		showIntroFlag = true;
		repeatCount = 0;
		// side task
		repeatTime = config.config3.testNum;
		planeRate = (float)config.config3.plane;
		heliRate = (float)config.config3.copter;
		timeBetweenMin = config.config3.minTimeSpace;
		timeBetweenMax = config.config3.maxTimeSpace;
		showTime = config.config3.viewTime;
		waitForInputTime = config.config3.waitTime;
		feedback = config.config3.feedback;
	}	

	public void InitPrefab()
	{
		InitAimer ();// layer z = 5
		InitTarget ();
		InitPlaHel ();
		UnityEngine.Object ob = Resources.Load ("Prefabs/Text");
		textObject = GameObject.Instantiate (ob) as GameObject;
		textObject.transform.position = new Vector3(0, 0, 90);
		textObject.SetActive (false);
		background.GetComponentInChildren<Renderer> ().material.color = backgroundColor;
	}


	public RanGen ranEx3;
	List<int> ranPos;
	List<int> ranPic;
	int ranPicCount;
	int ranPosCount;
	public void InitRan()
	{
		ranPic = new List<int> (repeatTime);
		ranPos = new List<int> ();
		ranPicCount = repeatTime;
		ranPosCount = repeatTime;
		for (int i = 0; i < Mathf.Floor(repeatTime * planeRate / (planeRate + heliRate)); i++) 
			ranPic.Add (0);
		for (int i = 0; i < repeatTime - Mathf.Floor (repeatTime * planeRate / (planeRate + heliRate)); i++)
			ranPic.Add (1);
		foreach (int i in ranPic)
			Debug.Log (i);

		int length = repeatTime / 4 + 1;
		for (int i = 0; i < length; i++) 
		{
			ranPos.Add (0);
			ranPos.Add (1);
			ranPos.Add (2);
			ranPos.Add (3);
		}

		int removeNum = 4 - repeatTime % 4;
		if (removeNum == 1) 
		{
			ranPos.RemoveAt (Random.Range (0, 4));
		}
		else if (removeNum == 3) 
		{
			int keep = Random.Range (0, 4);
			for (int i = 0; i < 4; i++) 
			{
				if (i != keep)
					ranPos.Remove (i);
			}
		}
		else if(removeNum == 2)
		{
			int keep1 = Random.Range (0, 4);
			int keep2;
			while ((keep2 = Random.Range (0, 4)) == keep1) {
			}
			for (int i = 0; i < 4; i++) 
			{
				if (i != keep1 && i != keep2)
					ranPos.Remove (i);
			}
		}

	}

	public override void UpdateExpLogic ()
	{
		base.UpdateExpLogic ();
		ProcessInput ();
		ProcessLogic ();
	}

	public bool button8Down;
	public bool button6Donw;
	void ProcessInput()
	{
		if(currentExpStatus == EXP3_STATUS.EXP_INIT)
		{
		}
		else if(currentExpStatus == EXP3_STATUS.EXP_INTRO)
		{
			if(Input.GetButtonDown("Button8"))
			{
				currentExpStatus = EXP3_STATUS.EXP_RUN;
				popoutPic.gameObject.SetActive(false);
				ResetParaForNextRun ();
				ShowAimerAndTarget ();
				checkMovementFlag = true;
			}
		}
		else if(currentExpStatus == EXP3_STATUS.EXP_PAUSE)
		{
		}
		else if(currentExpStatus == EXP3_STATUS.EXP_PRACTICE)
		{

		}
		else if(currentExpStatus == EXP3_STATUS.EXP_RUN)
		{
			if (checkPauseTipFlag) 
			{
				if (Input.anyKeyDown)
				{
					pauseTipFlag = false;
					checkPauseTipFlag = false;
					GameObject.Destroy (pauseTip);
					ShowAimerAndTarget ();
				}
			}
			if (checkRoundStartPauseFlag) 
			{
				if (Input.GetButton ("Horizontal")) 
				{
					roundStartPauseFlag = false;
					checkRoundStartPauseFlag = false;
					checkMovementFlag = true;
				}
			}
			if(checkMovementFlag)
			{
				if(Input.GetButton ("Horizontal"))
				{
					horiVal = Input.GetAxisRaw ("Horizontal");
					//Debug.Log ("Hori_" + Input.GetButtonDown ("Horizontal") + "_val:" + horiVal);
					horiMoveFlag = true;
				}
				if (Input.GetButton ("Vertical")) 
				{
					vertiVal = Input.GetAxisRaw ("Vertical");
					//Debug.Log ("Verti_" + Input.GetButtonDown ("Vertical") + "_val:" + vertiVal);
					vertiMoveFlag = true;
				}
			}
			if (checkGreenLightFlag)
			{
				if (Input.GetButtonDown ("Button8")) 
				{
					buttonDownFlag = true;
					button8Down = true;
                    sureTime = System.DateTime.Now;
                }
				else if(Input.GetButtonDown("Button6"))
				{
					buttonDownFlag = true;
					button6Donw = true;
                    sureTime = System.DateTime.Now;
                }
					
			}
		}
		else if(currentExpStatus == EXP3_STATUS.EXP_OVER)
		{
			if (Input.anyKeyDown)
				ExpManager.instance.GotoNextExp ();
		}
	}

	void ProcessLogic()
	{
		if(currentExpStatus == EXP3_STATUS.EXP_INIT)
		{
			currentExpStatus = EXP3_STATUS.EXP_INTRO;
		}
		else if(currentExpStatus == EXP3_STATUS.EXP_INTRO)
		{
			if(showIntroFlag)
			{
				showIntroFlag = false;
				ShowIntro();
			}
		}
		else if(currentExpStatus == EXP3_STATUS.EXP_PAUSE)
		{

		}
		else if(currentExpStatus == EXP3_STATUS.EXP_RUN)
		{
			if (checkPauseTipFlag) 
			{
				return;
			}

			if (roundStartPauseFlag && totalTimeThisRun > timeBetween) 
			{
				roundStartPauseFlag = false;
                //ChangeTextTip ("估计" + estiTimeThisRound + "秒", textTipTime);
                startTime = System.DateTime.Now;
				ShowThisRun();
				checkGreenLightFlag = true;
			}
			totalTimeExp3 += Utils.GetDeltaTime ();
			totalTimeThisRun += Utils.GetDeltaTime ();
			if (totalTimeThisRun > timeBetween + showTime && roundInitFlag)
			{
				roundInitFlag = false;
				//greenLight.SetActive (true);
				HidePlaHel();
			}
			AimerMove ();
			TargetMove ();
			CheckIfLocked ();
			ResetParaForNextFrame ();
			if (totalTimeThisRun > timeBetween + waitForInputTime + showTime || buttonDownFlag)
				roundFinishFlag = true;
			if (roundFinishFlag) 
			{
                //roundFinishFlag = false;
                //checkMovementFlag = false;
                RecordAns();
                HidePlaHel ();
				if (buttonDownFlag)
				{
					if (feedback) 
					{
						float time = totalTimeThisRun - timeBetween;
						string res;
						if((button6Donw && (plaHelPic == 1)) || (button8Down && (plaHelPic == 0)))
							res = "正确";
						else
							res = "错误";
						ChangeTextTip (res + "  反应时间\n" + time, textTipTime);
					}
				}
				else
				{
					if(feedback)
						ChangeTextTip ("反应超时", textTipTime);
				}
				//greenLight.SetActive (false);
				if (repeatCount >= repeatTime) 
				{
					EndExp ();
					return;
				} 
				else 
				{
					ResetParaForNextRun ();
					//pauseTipFlag = true;
					//checkPauseTipFlag = true;
					//HideAimerAndTarget ();
					//pauseTip = PopoutWithText ("休息一下，按任意键继续", 99999, 0, 0);
				}
			}
		}
		else if(currentExpStatus == EXP3_STATUS.EXP_OVER)
		{
			if (overPicFlag) 
			{
				overPicFlag = false;
				HideAimerAndTarget ();
				PopoutWithText ("该任务完成, 按任意键结束", 99999, 0, 100f);
			}
		}	
	}

	public bool showIntroFlag = false;
	public void ShowIntro()
	{
        if (config.config3.mainTest)
        {
            if (moveDirection)
                ShowPopout("Pics/Inst/DT_simpleRT_track", 0, 0, 99999);
            else
                ShowPopout("Pics/Inst/DT_simpleRT_track_a", 0, 0, 99999);
        }
        else
        {
            ShowPopout("Pics/Inst/ST_simpleRT", 0, 0, 99999);
        }
        //TODO
	}

	public void ResetParaForNextRun()
	{
		//ShowAimerAndTarget ();
		//aimer.transform.position = new Vector3 (0, 0, aimer.transform.position.z);
		//aimer.transform.rotation = Quaternion.identity;
		//target.transform.position = new Vector3 (-3, 0, aimer.transform.position.z);
		//target.transform.rotation = Quaternion.identity;
		//TODO: initial position
		roundInitFlag = true;
		roundStartPauseFlag = true;
		//checkRoundStartPauseFlag = true;
		roundFinishFlag = false;
		//checkMovementFlag = false;
		//totalTimePaused = 0f;
		targetPauseFlag = false;
		totalTimeThisRun = 0f;
		timeBetween = Random.Range (timeBetweenMin, timeBetweenMax);
		//currentAngle = 0f;
		//if (speedMode == SPEED_MODE.ACC) 
		//{
		//	currentSpeed = accStartSpeed;
		//}
		int temp;
		temp = Random.Range (0, ranPosCount);
		plaHelPos = ranPos[temp];
		ranPos.RemoveAt(temp);
		ranPosCount--;
		temp = Random.Range (0, ranPicCount);
		plaHelPic = ranPic [temp];
		ranPic.RemoveAt(temp);
		foreach (int i in ranPic)
			Debug.Log (i);
		ranPicCount--;
		repeatCount++;
		checkGreenLightFlag = false;
		buttonDownFlag = false;
		button6Donw = false;
		button8Down = false;
	}

	public void ResetParaForNextFrame()
	{
		horiMoveFlag = false;
		vertiMoveFlag = false;
		horiVal = 0f;
		vertiVal = 0f;
	}

	public void AimerMove()
	{
		if (horiMoveFlag) 
		{
			if (horiVal > 0)
				aimer.transform.position += new Vector3 (-aimerSpeed * Utils.GetDeltaTime (), 0, 0);
			else
				aimer.transform.position += new Vector3 (aimerSpeed * Utils.GetDeltaTime (), 0, 0);
			//Debug.Log (aimerSpeed * Utils.GetDeltaTime ());
		}
		if (vertiMoveFlag) 
		{
			if (vertiVal > 0)
				aimer.transform.position += new Vector3 (0, aimerSpeed * Utils.GetDeltaTime (), 0);
			else
				aimer.transform.position += new Vector3 (0, -aimerSpeed * Utils.GetDeltaTime (), 0);
			//Debug.Log (aimerSpeed * Utils.GetDeltaTime ());
		}
	}

	float currentAngle;
	float currentSpeed;
	float currentTargetRollAngle;
	float ecc;
	float omega; // for wrong elli cal
	public void TargetMove()
	{
		if (targetPauseFlag) 
		{
			totalTimePaused += Utils.GetDeltaTime ();
			return;
		}
		if (trailShape == TRAIL_SHAPE.CIRCLE)
		{
			// linear speed = v.anglular * r ------ 
			// linear shift = v.angular * r * time
			if (speedMode == SPEED_MODE.CONSTANT) 
			{
				float dir;
				if (moveDirection)
					dir = -1;
				else
					dir = 1;
				currentAngle = constantSpeed * (totalTimeExp3 - totalTimePaused) / circleRad;
				target.transform.position = new Vector3 (circleRad * Mathf.Cos (dir * currentAngle), circleRad * Mathf.Sin (dir * currentAngle), target.transform.position.z);
			} 
			else 
			{
				float dir;
				if (moveDirection)
					dir = -1;
				else
					dir = 1;
				float accVal = Random.Range (accValueMin, accValueMax);
				currentSpeed += accVal * Utils.GetDeltaTime ();
				Debug.Log (currentSpeed);
				if (currentSpeed > accMaxSpeed)
					currentSpeed = accMaxSpeed;
				else if (currentSpeed < accMinSpeed)
					currentSpeed = accMinSpeed;
				float angleShift = currentSpeed / circleRad * Utils.GetDeltaTime ();
				currentAngle += dir * angleShift;
				target.transform.position = new Vector3 (circleRad * Mathf.Cos (currentAngle), circleRad * Mathf.Sin (currentAngle), target.transform.position.z);
			}
		}
		else if (trailShape == TRAIL_SHAPE.ELLIPSE)
		{
			ecc = Mathf.Sqrt (1f - elliRadY * elliRadY / (elliRadX * elliRadX));

			if (speedMode == SPEED_MODE.CONSTANT) 
			{
				float dir;
				omega = WrongElliCal (currentSpeed, currentAngle);
				if (moveDirection)
					dir = -1;
				else
					dir = 1;
				currentAngle += dir * Utils.GetDeltaTime () * omega;
				target.transform.position = new Vector3 (elliRadX * Mathf.Cos (dir * currentAngle), elliRadY * Mathf.Sin (dir * currentAngle), target.transform.position.z);
			} 
			else 
			{
				float dir;
				if (moveDirection)
					dir = -1;
				else
					dir = 1;
				float accVal = Random.Range (accValueMin, accValueMax);
				currentSpeed += accVal * Utils.GetDeltaTime ();
				if (currentSpeed > accMaxSpeed)
					currentSpeed = accMaxSpeed;
				else if (currentSpeed < accMinSpeed)
					currentSpeed = accMinSpeed;
				omega = WrongElliCal (currentSpeed, currentAngle);
				float angleShift = omega * Utils.GetDeltaTime ();
				currentAngle += dir * angleShift;
				target.transform.position = new Vector3 (elliRadX * Mathf.Cos (currentAngle), elliRadY * Mathf.Sin (currentAngle), target.transform.position.z);
			}
		} 
		else 
		{
			if(speedMode == SPEED_MODE.CONSTANT)
			{
				float dir;
				if (moveDirection)
					dir = -1;
				else
					dir = 1;
				currentAngle = constantSpeed * (totalTimeExp3 - totalTimePaused) / circleRad;
				target.transform.position = new Vector3 ((Mathf.Sign(Mathf.Sin(dir*currentAngle/2f)))*(-eightRad + eightRad * Mathf.Cos (dir * currentAngle)),  eightRad * Mathf.Sin (dir * currentAngle), target.transform.position.z);
			}
			else
			{
				float dir;
				if (moveDirection)
					dir = -1;
				else
					dir = 1;
				float accVal = Random.Range (accValueMin, accValueMax);
				currentSpeed += accVal * Utils.GetDeltaTime ();
				Debug.Log (currentSpeed);
				if (currentSpeed > accMaxSpeed)
					currentSpeed = accMaxSpeed;
				else if (currentSpeed < accMinSpeed)
					currentSpeed = accMinSpeed;
				float angleShift = currentSpeed / circleRad * Utils.GetDeltaTime ();
				currentAngle += dir * angleShift;
				target.transform.position = new Vector3 ((Mathf.Sign(Mathf.Sin(dir*currentAngle/2f)))*(-eightRad + eightRad * Mathf.Cos (dir * currentAngle)),  eightRad * Mathf.Sin (dir * currentAngle), target.transform.position.z);
			}
		}
		if (movePattern == MOVE_PATTERN.TRAS_ROLL) 
		{
			float rollVal = Random.Range (rollMinSpeed, rollMaxSpeed) * Utils.GetDeltaTime ();
			currentTargetRollAngle += rollVal;
			if (currentTargetRollAngle > rollMaxRange)
				currentTargetRollAngle = rollMaxRange;
			else if (currentTargetRollAngle < rollMinRange)
				currentTargetRollAngle = rollMinRange;
			//Debug.Log (currentTargetRollAngle);
			target.transform.rotation = Quaternion.Euler (0, 0, currentTargetRollAngle);
		}
	}

	public bool CheckIfLocked()
	{
		Vector2 v1 = Utils.GetV2FromV3 (aimer.transform.position);
		Vector2 v2 = Utils.GetV2FromV3 (target.transform.position);
		v1 *= 100;
		v2 *= 100;
		//Debug.Log(Vector2.Distance(v1, v2));
		if (Vector2.Distance (v1, v2) < errDistance)
		{
			AimerTurnRed ();
		}
		else
			AimerTurnYellow ();
		return false;
	}
	public enum EXP3_STATUS
	{
		EXP_INIT,
		EXP_PAUSE,
		EXP_INTRO,
		EXP_PRACTICE,
		EXP_RUN,
		EXP_OVER
	}

	public void HideAimerAndTarget()
	{
		target.gameObject.SetActive (false);
		aimer.gameObject.SetActive (false);
	}
	public void ShowAimerAndTarget()
	{
		target.gameObject.SetActive (true);
		aimer.gameObject.SetActive (true);
	}

	public override void EndExp ()
	{
		base.EndExp ();
		currentExpStatus = EXP3_STATUS.EXP_OVER;
		overPicFlag = true;
	}

	public void SaveData()
	{
		//TODO
	}

	public override void ClearUI ()
	{
		base.ClearUI ();
		if (aimer != null)
			GameObject.Destroy (aimer);
		if (target != null)
			GameObject.Destroy (target);
		if (plaHel != null)
			GameObject.Destroy (plaHel);
	}

	public void InitAimer()
	{
		UnityEngine.Object prefab1 = Resources.Load ("Prefabs/Aimer");
		aimer = GameObject.Instantiate (prefab1) as GameObject;
		aimerRedPart = aimer.transform.FindChild ("Red").gameObject;
		aimerYellowPart = aimer.transform.FindChild ("Yellow").gameObject;
		AimerTurnYellow ();
		aimer.SetActive (false);
	}

	public GameObject aimerRedPart;
	public GameObject aimerYellowPart;
	public void AimerTurnRed()
	{
		if (aimer != null) 
		{
			aimerRedPart.SetActive (true);
			aimerYellowPart.SetActive (false);
		}
	}

	public void AimerTurnYellow()
	{
		if (aimer != null) 
		{
			aimerRedPart.SetActive (false);
			aimerYellowPart.SetActive (true);
		}
	}

	public GameObject greenLight;
	public GameObject targetText;

	public GameObject plaHel;
	public GameObject plane;
	public GameObject helicopter;

	public void InitPlaHel()
	{
		UnityEngine.Object prefab = Resources.Load ("Prefabs/PlaAndHeli");
		plaHel = GameObject.Instantiate (prefab) as GameObject;
		plaHel.SetActive (false);
		plane = plaHel.transform.Find ("Plane").gameObject;
		helicopter = plaHel.transform.Find ("Helicopter").gameObject;
		plane.SetActive (false);
		helicopter.SetActive (false);
	}

	public void ShowPlane(/*float posX, float posY*/)
	{
		plaHel.SetActive (true);
		helicopter.SetActive (false);
		plane.SetActive (true);
		//plaHel.transform.position = new Vector3(posX/100f, posY/100f, plaHel.transform.position.z);
	}

	public void ShowHeliCopter(/*float posX, float posY*/)
	{
		plaHel.SetActive (true);
		helicopter.SetActive (true);
		plane.SetActive (false);
		//plaHel.transform.position = new Vector3(posX/100f, posY/100f, plaHel.transform.position.z);
	}

	public void HidePlaHel()
	{
		plane.SetActive (false);
		helicopter.SetActive (false);
	}

	public void ShowThisRun()
	{
		if (plaHelPos == 0) 
		{
			plaHel.transform.position = new Vector3 (5, 3, plaHel.transform.position.z);
		}
		else if (plaHelPos == 1)
		{
			plaHel.transform.position = new Vector3 (-5, 3, plaHel.transform.position.z);
		} 
		else if (plaHelPos == 2)
		{
			plaHel.transform.position = new Vector3 (5, -3, plaHel.transform.position.z);
		} 
		else 
		{
			plaHel.transform.position = new Vector3 (-5, -3, plaHel.transform.position.z);
		}

		if (plaHelPic == 0)
			ShowPlane ();
		else
			ShowHeliCopter();
	}

	public void InitTarget()
	{
		UnityEngine.Object prefab1 = Resources.Load ("Prefabs/TargetWithLight");
		target = GameObject.Instantiate (prefab1) as GameObject;
		target.SetActive (false);
		greenLight = target.transform.Find ("GreenLight").gameObject;
		targetText = target.transform.Find ("Text").gameObject;
		greenLight.SetActive (false);
		targetText.SetActive (false);
	}

	public float WrongElliCal(float v, float alpha)
	{
		float omega = Mathf.Pow (v * v / (Mathf.Pow (elliRadX * Mathf.Sin (alpha), 2f) + Mathf.Pow (elliRadY * Mathf.Cos (alpha), 2f)), 0.5f);
		return omega;
	}

	public void ChangeTextTip(string text, float time)
	{
		TextMesh txm = textObject.GetComponentInChildren<TextMesh> ();
		textObject.SetActive (true);
		txm.text = text;
		StartCoroutine (HideTextTip(time, textObject));
	}
	public IEnumerator HideTextTip(float time, GameObject go)
	{
		yield return new WaitForSeconds (time);
		if (go != null)
			go.SetActive (false);
	}

    public void RecordPoint()
    {
        System.DateTime nowtime;
        Vector2 objPoint = Utils.GetV2FromV3(aimer.transform.position);
        Vector2 postPoint = Utils.GetV2FromV3(target.transform.position);
        Vector2 dObjPoint = (objPoint - lastObjPoint) * 100;
        Vector2 dPostPoint = (postPoint - lastPostPoint) * 100;
        double distance;
        int hit;

        pointnum++;
        nowtime = System.DateTime.Now;
        objPoint *= 100;
        postPoint *= 100;
        distance = Vector2.Distance(objPoint, postPoint);
        hit = ((distance < errDistance) ? 1 : 0);
        dObjPoint /= 40;
        dPostPoint /= 40;
        lastObjPoint = objPoint;
        lastPostPoint = postPoint;
        SaveTraceData(pointnum, nowtime, objPoint, postPoint, distance, hit, dObjPoint, dPostPoint);
    }

    public void SaveTraceData(int pointNum, System.DateTime pointTime, Vector2 objPoint, Vector2 postPoint, double distance, int hit, Vector2 objSpeed, Vector2 postSpeed)
    {
        List<string> savelist = new List<string>();
        savelist.Add(pointNum.ToString());
        savelist.Add(pointTime.ToString("HH:mm:ss"));
        savelist.Add((objPoint.x).ToString("f0"));
        savelist.Add((objPoint.y).ToString("f0"));
        savelist.Add((postPoint.x).ToString("f0"));
        savelist.Add((postPoint.y).ToString("f0"));
        savelist.Add(distance.ToString("f2"));
        savelist.Add(hit.ToString());
        savelist.Add((objSpeed.x).ToString("f0"));
        savelist.Add((objSpeed.y).ToString("f0"));
        savelist.Add((postSpeed.x).ToString("f0"));
        savelist.Add((postSpeed.y).ToString("f0"));

        Utils.DoFileOutputLine(savePath, saveTraceFilename, savelist);
    }

    public void SaveEventData(int eventNum, int eventType, System.DateTime startTime, System.DateTime sureTime, double event_RT, int buttonNo, int event_Acc)
    {
        List<string> savelist = new List<string>();
        savelist.Add(eventNum.ToString());
        savelist.Add(eventType.ToString());
        savelist.Add(startTime.ToString("HH:mm:ss"));
        savelist.Add(sureTime.ToString("HH:mm:ss"));
        savelist.Add(event_RT.ToString("f0"));
        savelist.Add(buttonNo.ToString());
        savelist.Add(event_Acc.ToString());

        Utils.DoFileOutputLine(savePath, saveEventFilename, savelist);
    }


    public void RecordAns()
    {
        int EventType;
        double Event_rt = -1;
        int ButtonNo = -1;
        int Event_acc = 0;

        eventnum++;
        EventType = plaHelPic;

        if (buttonDownFlag)
        {
            Event_rt = (totalTimeThisRun - timeBetween)*1000;
            ButtonNo = (button6Donw ? 1 : 0);
            Event_acc = (EventType == ButtonNo) ? 1 : 0;
        }
        else
        {
            sureTime = new System.DateTime();
        }
        SaveEventData(eventnum, EventType, startTime, sureTime, Event_rt, ButtonNo, Event_acc);
    }

}

