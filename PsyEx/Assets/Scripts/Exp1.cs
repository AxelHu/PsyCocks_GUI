using UnityEngine;
using System.Collections;

public class Exp1 : ExpObject
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
	//--------------------parameters used in exp1
	public EXP1_STATUS currentExpStatus;
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
	public float totalTimeThisRound;
	//------------------
	public override void Init ()
	{
		base.Init ();
		InitPara ();
		InitPrefab ();
		InitRan ();
	}



	public void InitPara()
	{
		if (config.config1.backgroundColor == "灰黑")
			backgroundColor = new Color (48f / 255f, 48f / 255f, 48f / 255f);
		else if (config.config1.backgroundColor == "蓝黑")
			backgroundColor = new Color (20f / 255f, 20f / 255f, 70f / 255f);
		else
			backgroundColor = new Color (48f / 255f, 48f / 255f, 48f / 255f);

		runTime = config.config1.testTime;
		repeatTime = config.config1.testNum;
		if (config.config1.particle == "圆形")
			trailShape = TRAIL_SHAPE.CIRCLE;
		else if (config.config1.particle == "椭圆形")
			trailShape = TRAIL_SHAPE.ELLIPSE;
		else
			trailShape = TRAIL_SHAPE.EIGHT;
		if (config.config1.direction == 0)
			moveDirection = true;
		else
			moveDirection = false;
		if (config.config1.ctrlDirection == 0)
			controllerDirection = true;
		else
			controllerDirection = false;
		if (config.config1.moveMode == 0)
			movePattern = MOVE_PATTERN.TRANS;
		else
			movePattern = MOVE_PATTERN.TRAS_ROLL;

		pauseEnabled = config.config1.pause;
		pauseFrequency = config.config1.pauseRate;
		if(pauseEnabled)
			pauseTime = 60f / pauseFrequency;
		if (config.config1.speedMode == 0)
			speedMode = SPEED_MODE.CONSTANT;
		else
			speedMode = SPEED_MODE.ACC;
		constantSpeed = (float)config.config1.speed / 100f;
		accStartSpeed = (float)config.config1.speed / 100f;
		accMinSpeed = (float)config.config1.minSpeed / 100f;
		accMaxSpeed = (float)config.config1.maxSpeed / 100f;
		accValueMin = (float)config.config1.minASpeed / 100f;
		accValueMax = (float)config.config1.maxASpeed / 100f;
		rollMinSpeed = (float)config.config1.minGTASpeed;
		rollMaxSpeed = (float)config.config1.maxGTASpeed;
		rollMinRange = (float)config.config1.minAngle;
		rollMaxRange = (float)config.config1.maxAngel;

		//aimerSpeed = (float)hdConfig.Speed;
		aimerSpeed = 150f/100f;
		errDistance = 50f;
		showIntroFlag = true;
		repeatCount = 0;
	}	

	public void InitPrefab()
	{
		InitAimer ();// layer z = 5
		InitTarget ();
		background.GetComponentInChildren<Renderer> ().material.color = backgroundColor;
	}

	public void InitRan()
	{
	}

	public override void UpdateExpLogic ()
	{
		base.UpdateExpLogic ();
		ProcessInput ();
		ProcessLogic ();
	}

	void ProcessInput()
	{
		if(currentExpStatus == EXP1_STATUS.EXP_INIT)
		{
		}
		else if(currentExpStatus == EXP1_STATUS.EXP_INTRO)
		{
			if(Input.GetButtonDown("Button8"))
			{
				currentExpStatus = EXP1_STATUS.EXP_RUN;
				popoutPic.gameObject.SetActive(false);
				ResetParaForNextRun ();
			}
		}
		else if(currentExpStatus == EXP1_STATUS.EXP_PAUSE)
		{
		}
		else if(currentExpStatus == EXP1_STATUS.EXP_PRACTICE)
		{

		}
		else if(currentExpStatus == EXP1_STATUS.EXP_RUN)
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
		}
		else if(currentExpStatus == EXP1_STATUS.EXP_OVER)
		{
			if (Input.anyKeyDown)
				ExpManager.instance.GotoNextExp ();
		}
	}

	void ProcessLogic()
	{
		if(currentExpStatus == EXP1_STATUS.EXP_INIT)
		{
			currentExpStatus = EXP1_STATUS.EXP_INTRO;
		}
		else if(currentExpStatus == EXP1_STATUS.EXP_INTRO)
		{
			if(showIntroFlag)
			{
				showIntroFlag = false;
				ShowIntro();
			}
		}
		else if(currentExpStatus == EXP1_STATUS.EXP_PAUSE)
		{

		}
		else if(currentExpStatus == EXP1_STATUS.EXP_RUN)
		{
			if (checkPauseTipFlag) 
			{
				return;
			}

			if (roundStartPauseFlag)
				return;
			totalTimeThisRound += Utils.GetDeltaTime ();
			AimerMove ();
			TargetMove ();
			CheckIfLocked ();
			ResetParaForNextFrame ();
			if (totalTimeThisRound - totalTimePaused > runTime)
				roundFinishFlag = true;
			if (roundFinishFlag) 
			{
				roundFinishFlag = false;
				checkMovementFlag = false;
				SaveData ();
				if (repeatCount >= repeatTime) 
				{
					EndExp ();
					return;
				} 
				else 
				{
					ResetParaForNextRun ();
					pauseTipFlag = true;
					checkPauseTipFlag = true;
					HideAimerAndTarget ();
					pauseTip = PopoutWithText ("休息一下，按任意键继续", 99999, 0, 0);
				}
			}
		}
		else if(currentExpStatus == EXP1_STATUS.EXP_OVER)
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
		if (moveDirection)
			ShowPopout ("Pics/Inst/ST_Tracking", 0, 0, 99999);
		else
			ShowPopout ("Pics/Inst/ST_Tracking", 0, 0, 99999);
		//TODO
	}

	public void ResetParaForNextRun()
	{
		ShowAimerAndTarget ();
		aimer.transform.position = new Vector3 (0, 0, aimer.transform.position.z);
		aimer.transform.rotation = Quaternion.identity;
		target.transform.position = new Vector3 (-3, 0, aimer.transform.position.z);
		target.transform.rotation = Quaternion.identity;
		//TODO: initial position
		roundInitFlag = true;
		roundStartPauseFlag = true;
		checkRoundStartPauseFlag = true;
		roundFinishFlag = false;
		checkMovementFlag = false;
		totalTimePaused = 0f;
		targetPauseFlag = false;
		totalTimeThisRound = 0f;
		currentAngle = 0f;
		if (speedMode == SPEED_MODE.ACC) 
		{
			currentSpeed = accStartSpeed;
		}
		repeatCount++;
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
				currentAngle = constantSpeed * (totalTimeThisRound - totalTimePaused) / circleRad;
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
				currentAngle = constantSpeed * (totalTimeThisRound - totalTimePaused) / circleRad;
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
	public enum EXP1_STATUS
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
		currentExpStatus = EXP1_STATUS.EXP_OVER;
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

	public void InitTarget()
	{
		UnityEngine.Object prefab1 = Resources.Load ("Prefabs/Target");
		target = GameObject.Instantiate (prefab1) as GameObject;
		target.SetActive (false);
	}

	public float WrongElliCal(float v, float alpha)
	{
		float omega = Mathf.Pow (v * v / (Mathf.Pow (elliRadX * Mathf.Sin (alpha), 2f) + Mathf.Pow (elliRadY * Mathf.Cos (alpha), 2f)), 0.5f);
		return omega;
	}
}

