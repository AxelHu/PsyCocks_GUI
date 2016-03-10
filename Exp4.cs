using UnityEngine;
using System.Collections;

public class Exp4 : ExpObject
{
	Color ballColor;
	Color backgroundColor;
	Color blockerColor;

	float ballPixelRad;
	float blockerPixelRad = 50;
	float ballToCenterDistance;


	float ballSpeed1;
	float ballSpeed2;
	float ballSpeed3;

	bool biasFeedbackFlag;

	int repeatTime;
	float waitTimeInBetween;

	enum INIT_POS
	{
		LEFT = 0,
		RIGHT = 1,
		UP = 2,
		DOWN = 3
	}

	//-------------------

	public EXP4_STATUS currentExpStatus;
	protected float waitTime;
	protected int runCount;

	protected GameObject blockerPic;

	public override void Init ()
	{
		base.Init ();
		InitPrefab ();
	}

	public override void UpdateExpLogic ()
	{
		base.UpdateExpLogic ();
		ProcessInput ();
		ProcessLogic ();

	}


	bool roundInitFlag = false;

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

			}
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_OVER)
		{
			
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
			if(roundInitFlag)
			{
				roundInitFlag = false;
				blockerPic.gameObject.SetActive(true);
			}
		}
		else if(currentExpStatus == EXP4_STATUS.EXP_OVER)
		{
			
		}
	}

	bool showIntroFlag = true;
	public void ShowIntro()
	{
		ShowPopout ("Pics/Inst/Speed_Sensor", 0, 0, 9999);
	}

	public void InitPrefab()
	{

	}

	public void InitBlocker()
	{
		Object prefab1 = Resources.Load ("Prefabs/Exp5Pic");
		GameObject blockerPic = GameObject.Instantiate (prefab1) as GameObject;

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
}

