using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seeker : MonoBehaviour {

	public float mSpeed = 1.0f;

	public float mNextWaypointDistance = 0.2f;

	private List<Vector3> mCurrentPath;

	private bool mIsMoving;

	public int CurrentWayPoint {
		get {
			return mCurrentWayPoint;
		}
	}
	protected int mCurrentWayPoint = 0;

	void Awake()
	{
		mCurrentPath = new List<Vector3> ();
		mIsMoving = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (mIsMoving) {
			RealMove();
		}
		//Vector3 newposition = 
	}

	public void Move(List<Vector3> path)
	{
		mCurrentPath = path;
		mIsMoving = true;
		mCurrentWayPoint = mCurrentPath.Count - 1;
	}

	private void RealMove()
	{
		if (mCurrentPath == null) {
			//We have no path to move after yet
			return;
		}
		if (mCurrentWayPoint < 0) {
			Debug.Log ("End Of Path Reached");
			mIsMoving = false;
			return;
		}
		//Direction to the next waypoint
		Vector3 dir = (mCurrentPath[mCurrentWayPoint] - transform.position).normalized;
		
		transform.LookAt (mCurrentPath [mCurrentWayPoint]);

		Vector3 newposition = transform.position + dir * mSpeed * Time.deltaTime;
		transform.position = newposition;
		
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position, mCurrentPath[mCurrentWayPoint]) < mNextWaypointDistance) {
			mCurrentWayPoint--;
			return;
		}
	}
}
