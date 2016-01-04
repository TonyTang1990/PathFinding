using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class TimerCounter{

	private static TimerCounter TCInstance = null;

	private Stopwatch mTimer;

	private string mName;

	public float TimeSpend
	{
		get
		{
			return mTimer.ElapsedMilliseconds;
		}
	}
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
		mTimer = new Stopwatch ();
		mName = "Default";
	}

	public void Start(string name)
	{
		mName = name;
		mTimer.Start ();
	}

	public void Restart(string name)
	{
		mTimer.Reset ();
		mTimer.Start ();
		mName = name;
	}

	public void End()
	{
		mTimer.Stop ();

		mTimeSpend = mTimer.ElapsedMilliseconds;

		Utility.Log (mName + " takes: " + mTimeSpend);
	}
}
