using UnityEngine;
using System.Collections;

public class TimerCounter{

	private static TimerCounter TCInstance = null;

	private string mName;

	private float mStartTime;

	private float mEndTime;

	private float mTimeSpend;

	public static TimerCounter CreateInstance()
	{
		if (TCInstance == null) {
			TCInstance = new TimerCounter();
		}
		return TCInstance;
	}

	public static void DestroyInstance()
	{
		if (TCInstance != null) {
			TCInstance = null;
		}
	}

	private TimerCounter()
	{

	}

	public void Start(string name)
	{
		mName = name;
		mStartTime = Time.realtimeSinceStartup;

	}

	public void End()
	{
		mEndTime = Time.realtimeSinceStartup;

		mTimeSpend = mEndTime - mStartTime;

		Debug.Log (mName + " takes: " + mTimeSpend);
	}
}
