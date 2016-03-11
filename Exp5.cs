using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public float pointTime;//注视点时间
	public float viewTime;//呈现时间
	public float lastTime;//倒数时间

	float resPopoutTime = 3f;
	float introPopoutTime = 5f;
	float timeCountdown; // used to count down and show tips etc.
	bool introPopoutFlag = false;

	float waitTime; // used in several states to manually record time pass

	public override void Init()
	{
		base.Init ();
		InitPrefab ();
		InitRan ();

		currentExpStatus = EXP5_STATUS.EXP_INIT;
		waitTime = 1f;// pretend loading
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
		leftPic.SetPic (path);
	}

	public void ChangeRightPic(string path)
	{
		rightPic.SetPic (path);
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
		Object prefab1 = Resources.Load ("Prefabs/Exp5Pic");
		GameObject go1 = GameObject.Instantiate (prefab1) as GameObject;
		leftPic = go1.GetComponent<UISprite> ();
		PanelData pd1 = new PanelData ();
		pd1.Init (600, 600, 300, 0, 1);
		leftPic.InitWithPic (pd1, null, "Pics/TaskR/1_x_15_a");
		
		// rightPic
		Object prefab2 = Resources.Load ("Prefabs/Exp5Pic");
		GameObject go2 = GameObject.Instantiate (prefab1) as GameObject;
		leftPic = go2.GetComponent<UISprite> ();
		PanelData pd2 = new PanelData ();
		pd2.Init (600, 600, -300, 0, 1);
		leftPic.InitWithPic (pd2, null, "Pics/TaskR/1_x_15_b");
		
		Object prefab3 = Resources.Load ("Prefabs/Exp5Pic");
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
	}

	public void ShowIntro()
	{
		ShowPopout ("Pics/TaskR/ST_3D", 0, 0, 5);
	}

	protected PIC_STATUS currentPicCombStatus;
	protected ANSWER_STATUS currentAnswerStatus;

	bool ansCheckFlag = false;
	bool timeWarningFlag = false;
	bool answerVal;

	public void ShowRes()
	{
		string picPath = RespondPicPath (currentPicCombStatus, currentAnswerStatus);
		ShowPopout (picPath, 0, 0, resPopoutTime);
	}


	public void ProcessLogic()
	{
		if(currentExpStatus == EXP5_STATUS.EXP_INIT)
		{
			waitTime -= Utils.GetDeltaTime ();
			if (waitTime < 0f) 
			{
				currentExpStatus = EXP5_STATUS.EXP_INTRO;
				introPopoutFlag = true;
				waitTime = introPopoutTime = 0.5f;
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_INTRO)
		{
			if (introPopoutFlag) 
			{
				ShowPopout ("Pics/TaskR/ST_3D", 0, 0, introPopoutTime);
				introPopoutFlag = false;
			}
			waitTime -= Utils.GetDeltaTime ();
			if (waitTime < 0) 
			{
				currentExpStatus = EXP5_STATUS.EXP_PRACTICE;
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PAUSE)
		{

		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PRACTICE)
		{
			// TODO
			currentExpStatus = EXP5_STATUS.EXP_RUN;
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_RUN)
		{
			if (ansCheckFlag) 
			{
				ansCheckFlag = false;
				RecordAns ();
				bool isFinished = ShowNext ();
				if (isFinished) 
				{
					//TODO: all finished, goto next exp
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
				ResetParaForNext ();
				RecordAns ();
				bool isFinished = ShowNext ();
				if (isFinished) 
				{
					//TODO: all finished, goto next exp
				}
				else
				{
					//TODO: reset time count-down etc...
					ResetParaForNext();
				}
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_OVER)
		{

		}
	}

	public void ProcessInput()
	{
		if(currentExpStatus == EXP5_STATUS.EXP_INIT)
		{
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_INTRO)
		{

		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PAUSE)
		{
			
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_PRACTICE)
		{

		}
		else if(currentExpStatus == EXP5_STATUS.EXP_RUN)
		{
			if (Input.GetButtonDown ("Yes")) 
			{
				Debug.Log ("yeah!");
				answerVal = true;
				ansCheckFlag = true;
			}
			else if (Input.GetButtonDown ("No")) 
			{
				Debug.Log ("No");
				answerVal = false;
				ansCheckFlag = true;
			}
		}
		else if(currentExpStatus == EXP5_STATUS.EXP_OVER)
		{

		}
	}	

	protected bool eightNineFlag = true;// true: show 8,  false: show 9
	// bool return value, true: still something left, false: all done
	public bool ShowNext()
	{
		//TODO
		if (ran8x.IsFinished () && ran9x.IsFinished () && ran8z.IsFinished () && ran9z.IsFinished ())
			return true;
		if (eightNineFlag)
		{
			eightNineFlag = !eightNineFlag;

		}
		else
		{
			eightNineFlag = !eightNineFlag;
		}
		return false;
	}

	public void ShowCountDown(float time)
	{
		//TODO
	}

	public void ResetParaForNext()
	{
		timeCountdown = viewTime;

	}

	public void RecordAns()
	{
		//TODO
	}
}

