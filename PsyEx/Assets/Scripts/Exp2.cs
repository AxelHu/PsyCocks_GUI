using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Exp2 : ExpObject
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
	//--------------------parameters used in exp2
	public EXP2_STATUS currentExpStatus;
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
	public float totalTimeExp2;
	public float totalTimeThisRun;
	//------------------
	// for side task
	public int loopTime;
	public bool feedback;
	public bool mainTask;
	public int estimateNum;

	public bool buttonDownFlag;
	public bool checkGreenLightFlag;

	public float estiTimeThisRound;
	public string loopNoTag;

    protected int pointnum = 0;
    protected int holdnum = 0;

    protected System.DateTime startTime;
    protected System.DateTime sureTime;

    protected Vector2 lastObjPoint = new Vector2(3f, 0);
    protected Vector2 lastPostPoint = new Vector2(0, 0);
    protected double lastRotateAngle = 0;

    private string saveTime;
    private string savePath;
    private string saveTraceFilename;
    private string saveHoldFilename;

    public float textTipTime = 2f;
	public float roundRestTime = 2f;

	public override void Init ()
	{
		base.Init ();
		InitPara ();
		InitPrefab ();
		InitRan ();

        if (config.config2.mainTest)
        {
            InitTraceOutput();
        }
        InitHoldOutput();

	}

    void InitTraceOutput()
    {
        List<string> outputlist = new List<string>();
        outputlist = new List<string> { "PointNum", "PointTime", "ObjPointX", "ObjPointY", "PostPointX", "PostPointY", "Distance", "Hit", "ObjSpeedX", "ObjSpeedY", "PostSpeedX", "PostSpeedY" };

        saveTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        savePath = Utils.MakeDirectoy("Data\\" + ExpManager.tester.Id + "-" + ExpManager.tester.Name);
        saveTraceFilename = "T2-Trace-" + config.sortId + "-任务2-操作力保持及时间知觉能力测试-" + ExpManager.tester.Id + "-" + ExpManager.tester.Name + "-" + ExpManager.tester.Count + "-" + saveTime + ".csv";
        Utils.DoFileOutputLine(savePath, saveTraceFilename, outputlist);
    }

    void InitHoldOutput()
    {
        List<string> outputlist = new List<string>();
        outputlist = new List<string> { "HoldNum", "HoldTime", "StartTime", "SureTime", "Test_RT", "HoldError", "Error_Ratio" };

        saveTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        savePath = Utils.MakeDirectoy("Data\\" + ExpManager.tester.Id + "-" + ExpManager.tester.Name);
        saveHoldFilename = "T2-Hold-" + config.sortId + "-任务2-操作力保持及时间知觉能力测试-" + ExpManager.tester.Id + "-" + ExpManager.tester.Name + "-" + ExpManager.tester.Count + "-" + saveTime + ".csv";
        Utils.DoFileOutputLine(savePath, saveHoldFilename, outputlist);
    }

    public void InitPara()
	{
		if (config.config2.backgroundColor == "灰黑")
			backgroundColor = new Color (48f / 255f, 48f / 255f, 48f / 255f);
		else if (config.config2.backgroundColor == "蓝黑")
			backgroundColor = new Color (20f / 255f, 20f / 255f, 70f / 255f);
		else
			backgroundColor = new Color (48f / 255f, 48f / 255f, 48f / 255f);

		//runTime = config.config2.testTime;
		//repeatTime = config.config2.testNum;
		if (config.config2.moveTrail == "圆形")
			trailShape = TRAIL_SHAPE.CIRCLE;
		else if (config.config2.moveTrail == "椭圆形")
			trailShape = TRAIL_SHAPE.ELLIPSE;
		else
			trailShape = TRAIL_SHAPE.EIGHT;
		if (config.config2.direction == 0)
			moveDirection = true;
		else
			moveDirection = false;
		if (config.config2.ctrlDirection == 0)
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
		if (config.config2.speedMode == 0)
			speedMode = SPEED_MODE.CONSTANT;
		else
			speedMode = SPEED_MODE.ACC;
		constantSpeed = (float)config.config2.speed / 100f;
		accStartSpeed = (float)config.config2.speed / 100f;
		accMinSpeed = (float)config.config2.minSpeed / 100f;
		accMaxSpeed = (float)config.config2.maxSpeed / 100f;
		accValueMin = (float)config.config2.minASpeed / 100f;
		//accValueMax = (float)config.config2maxASpeed / 100f;
		//rollMinSpeed = (float)config.config2.minGTASpeed;
		//rollMaxSpeed = (float)config.config2.maxGTASpeed;
		//rollMinRange = (float)config.config2.minAngle;
		//rollMaxRange = (float)config.config2.maxAngel;

		//aimerSpeed = (float)hdConfig.Speed;
		aimerSpeed = 150f/100f;
		errDistance = 50f;
		showIntroFlag = true;
		repeatCount = 0;
		// side task
		loopTime = config.config2.loop;
		feedback = config.config2.feedback;
		estimateNum = config.config2.estimateNum;
	}	

	public void InitPrefab()
	{
		InitAimer ();// layer z = 5
		InitTarget ();
		background.GetComponentInChildren<Renderer> ().material.color = backgroundColor;
	}


	public RanGen ranEx1;
	public void InitRan()
	{
		List<string> lo = new List<string> ();
		for (int i = 0; i < loopTime; i++)
			lo.Add ("loop" + i);
		List<string> tm = new List<string> ();
		int ct = estimateNum;
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate1.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate2.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate3.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate4.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate5.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate6.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate7.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate8.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate9.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate10.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate11.ToString ());
			ct--;
		}
		if (ct > 0) 
		{
			tm.Add (config.config2.estimate12.ToString ());
			ct--;
		}
		List<List<string>> strLiLi = new List<List<string>>{ lo, tm };
		ranEx1 = new RanGen ();
		ranEx1.Init (strLiLi, "_");
	}

	public override void UpdateExpLogic ()
	{
		base.UpdateExpLogic ();
		ProcessInput ();
		ProcessLogic ();
	}

	void ProcessInput()
	{
		if(currentExpStatus == EXP2_STATUS.EXP_INIT)
		{
		}
		else if(currentExpStatus == EXP2_STATUS.EXP_INTRO)
		{
			if(Input.GetButtonDown("Button8"))
			{
				currentExpStatus = EXP2_STATUS.EXP_RUN;
				popoutPic.gameObject.SetActive(false);
				ResetParaForNextRun ();
				ShowAimerAndTarget ();
				checkMovementFlag = true;
			}
		}
		else if(currentExpStatus == EXP2_STATUS.EXP_PAUSE)
		{
		}
		else if(currentExpStatus == EXP2_STATUS.EXP_PRACTICE)
		{

		}
		else if(currentExpStatus == EXP2_STATUS.EXP_RUN)
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
                    sureTime = System.DateTime.Now;
					buttonDownFlag = true;
				}
			}
		}
		else if(currentExpStatus == EXP2_STATUS.EXP_OVER)
		{
			if (Input.anyKeyDown)
				ExpManager.instance.GotoNextExp ();
		}
	}

	void ProcessLogic()
	{
		if(currentExpStatus == EXP2_STATUS.EXP_INIT)
		{
			currentExpStatus = EXP2_STATUS.EXP_INTRO;
		}
		else if(currentExpStatus == EXP2_STATUS.EXP_INTRO)
		{
			if(showIntroFlag)
			{
				showIntroFlag = false;
				ShowIntro();
			}
		}
		else if(currentExpStatus == EXP2_STATUS.EXP_PAUSE)
		{

		}
		else if(currentExpStatus == EXP2_STATUS.EXP_RUN)
		{
			if (checkPauseTipFlag) 
			{
				return;
			}

			if (roundStartPauseFlag && totalTimeThisRun > roundRestTime) 
			{
				roundStartPauseFlag = false;
				ActiveText (estiTimeThisRound + "s");
			}
			totalTimeExp2 += Utils.GetDeltaTime ();
			totalTimeThisRun += Utils.GetDeltaTime ();
			if (totalTimeThisRun > 2 * roundRestTime && roundInitFlag)
			{
				roundInitFlag = false;
                GreenLightFlash();
				checkGreenLightFlag = true;
                startTime = System.DateTime.Now;
			}
			AimerMove ();
			TargetMove ();
			CheckIfLocked ();
			ResetParaForNextFrame ();
			if (totalTimeThisRun > 2f * estiTimeThisRound + 5f || buttonDownFlag)
				roundFinishFlag = true;
			if (roundFinishFlag) 
			{
                HideObject(targetText);
				//roundFinishFlag = false;
				//checkMovementFlag = false;
				RecordAns ();
				if (buttonDownFlag)
				{
					if (feedback) 
					{
						float time = totalTimeThisRun -  2 * roundRestTime;
						ChangeTextTip ("知觉时间\n" + (time * 1000).ToString("f0") + "ms", textTipTime);
					}
				}
				else
				{
					if(feedback)
						ChangeTextTip ("反应超时", textTipTime);
				}
				//greenLight.SetActive (false);
				if (repeatCount >= loopTime * estimateNum) 
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
		else if(currentExpStatus == EXP2_STATUS.EXP_OVER)
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
        if (config.config2.mainTest)
        {
            if (moveDirection)
                ShowPopout("Pics/Inst/DT_timing_track", 0, 0, 99999);
            else
                ShowPopout("Pics/Inst/DT_timing_track_a", 0, 0, 99999);
        }
        else
        {
            ShowPopout("Pics/Inst/ST_timing", 0, 0, 99999);
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
		//currentAngle = 0f;
		//if (speedMode == SPEED_MODE.ACC) 
		//{
		//	currentSpeed = accStartSpeed;
		//}
		List<string> runSetting = Utils.SplitString(ranEx1.GetNextRanVal(), "_");
		loopNoTag = runSetting [0];
		estiTimeThisRound = (float)double.Parse (runSetting [1]);
		repeatCount++;
		checkGreenLightFlag = false;
		buttonDownFlag = false;
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
				currentAngle = constantSpeed * (totalTimeExp2 - totalTimePaused) / circleRad;
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
				currentAngle = constantSpeed * (totalTimeExp2 - totalTimePaused) / circleRad;
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
	public enum EXP2_STATUS
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
		currentExpStatus = EXP2_STATUS.EXP_OVER;
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

    public void ActiveText(string text)
    {
        TextMesh txm = targetText.GetComponent<TextMesh> ();
		txm.text = text;
		targetText.SetActive (true);
    }

    public void HideObject(GameObject go)
    {
        if (go != null)
			go.SetActive (false);
    }

	public void ChangeTextTip(string text, float time)
	{
        ActiveText(text);
		StartCoroutine (HideWait (time, targetText));
	}

	public IEnumerator HideWait(float time, GameObject go)
	{
		yield return new WaitForSeconds (time);
        HideObject(go);
	}

    public void GreenLightFlash()
    {
        greenLight.SetActive(true);
        StartCoroutine(HideWait(0.5f, greenLight));
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

    public void SaveTraceData(int pointNum, System.DateTime pointTime, Vector2 objPoint, Vector2 postPoint, double distance, int hit, Vector2 objSpeed, Vector2 postSpeed )
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

    public void SaveHoldData(int holdNum, double holdTime, System.DateTime startTime, System.DateTime sureTime, double test_RT, double holdError, double error_Ratio)
    {
        List<string> savelist = new List<string>();
        savelist.Add(holdNum.ToString());
        savelist.Add(holdTime.ToString("f0"));
        savelist.Add(startTime.ToString("HH:mm:ss"));
        savelist.Add(sureTime.ToString("HH:mm:ss"));
        savelist.Add(test_RT.ToString("f0"));
        savelist.Add(holdError.ToString("f0"));
        savelist.Add(error_Ratio.ToString("f2"));

        Utils.DoFileOutputLine(savePath, saveHoldFilename, savelist);
    }

    public void RecordAns()
    {
        double Test_RT = -1;
        double HoldError = -1;
        double Error_Ratio = -1;

        holdnum++;
        if (buttonDownFlag)
        {
            Test_RT = (totalTimeThisRun - 2 * roundRestTime)*1000;
            HoldError = Test_RT - estiTimeThisRound * 1000;
            Error_Ratio = HoldError / (estiTimeThisRound * 1000);
        }
        else
        {
            sureTime = new System.DateTime();
        }
        SaveHoldData(holdnum, estiTimeThisRound*1000 , startTime, sureTime, Test_RT, HoldError, Error_Ratio);

    }
}

