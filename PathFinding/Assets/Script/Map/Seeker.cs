using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Seeker : MonoBehaviour {
    
	public float mSpeed = 1.0f;

	public float mNextWaypointDistance = 0.2f;

	private PathFinder mPathFinder;

	//this list will store any path returned from a graph search
	private List<int> mPath;
	
	public List<Vector3> MovementPath {
		get {
			return mMovementPath;
		}
	}
	private List<Vector3> mMovementPath;
	
	//this list of edges is used to store any subtree returned from any of the graph algorithms
	private List<GraphEdge> mSubTree;
	
	public float CostToTarget {
		get {
			return mCostToTarget;
		}
	}
	private float mCostToTarget;
	
	//Holds the time taken for the currently used algorithm to complete
	public float TimeTaken {
		get {
			return mTimeTaken;
		}
		set {
			mTimeTaken = value;
		}
	}
	private float mTimeTaken;
	
	public int NodesSearched {
		get {
			return mNodesSearched;
		}
		set {
			mNodesSearched = value;
		}
	}
	private int mNodesSearched;
	
	public int EdgesSearched {
		get {
			return mEdgesSearched;
		}
		set {
			mEdgesSearched = value;
		}
	}
	private int mEdgesSearched;
	
	public int SourceCellIndex {
		get {
			return mSourceCellIndex;
		}
		set {
			mSourceCellIndex = value;
		}
	}
	private int mSourceCellIndex = 0;
	
	public int TargetCellIndex {
		get {
			return mTargetCellIndex;
		}
		set {
			mTargetCellIndex = value;
		}
	}
	private int mTargetCellIndex = 0;
	
	public float StrickDistance {
		get {
			return mStrickDistance;
		}
		set {
			mStrickDistance = value;
		}
	}
	private float mStrickDistance = 0.0f;

	public bool mIgnoreWall = false;

	public float mHCostPercentage = 1.0f;
	
	public bool mBDrawExplorePath = true;
	
	public float mExplorePathRemainTime = 2.0f;

	private bool mIsMoving;

	public int CurrentWayPoint {
		get {
			return mCurrentWayPoint;
		}
	}
	protected int mCurrentWayPoint = 0;

	private SparseGraph<NavGraphNode, GraphEdge> mNavGraph;

	public SearchAStar mAstarSearch;

	void Awake()
	{
		mIsMoving = false;

		mCostToTarget = 0;
		mTimeTaken = 0;
		mPath = new List<int> ();
		mMovementPath = new List<Vector3> ();
		mSubTree = new List<GraphEdge> ();
		
		//A Star Info
		mTimeTaken = 0;
		mNodesSearched = 0;
		mEdgesSearched = 0;

		mPath.Clear ();
		mMovementPath.Clear ();
		mSubTree.Clear ();
		mTimeTaken = 0;

		mAstarSearch = null;
	}

	// Use this for initialization
	void Start () {
		mPathFinder = FindObjectOfType (typeof(PathFinder)) as PathFinder;
		Debug.Assert (mPathFinder != null);
		mNavGraph = mPathFinder.NavGraph;
		Debug.Assert (mNavGraph != null);
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (mIsMoving) {
			RealMove();
		}
		*/
		//Vector3 newposition = 
	}

	public void CreatePathAStar()
	{
		TimerCounter.CreateInstance ().Restart ("AStarSearch");
		
		//SearchAStar astarsearch = new SearchAStar (mNavGraph, mSourceCellIndex, mTargetCellIndex, mStrickDistance, mHCostPercentage, mBDrawExplorePath, mExplorePathRemainTime);
		mAstarSearch = new SearchAStar (mNavGraph, mSourceCellIndex, mTargetCellIndex, mIgnoreWall, mStrickDistance, mHCostPercentage, mBDrawExplorePath, mExplorePathRemainTime);

		TimerCounter.CreateInstance ().End ();

		Debug.Log ("mAstarSearch.ITarget = " + mAstarSearch.ITarget);
		Debug.Log ("mAstarSearch.IsWallInPathToTarget = " + mAstarSearch.IsWallInPathToTarget);
		Debug.Log ("mAstarSearch.WallInPathToTargetIndex = " + mAstarSearch.WallInPathToTargetIndex);

		mTimeTaken = TimerCounter.CreateInstance ().TimeSpend;
		
		mPath = mAstarSearch.PathToTarget;
		
		mMovementPath = mAstarSearch.MovementPathToTarget;
		
		mSubTree = mAstarSearch.GetSPT ();
		
		mCostToTarget = mAstarSearch.GetCostToTarget ();
		
		mNodesSearched = mAstarSearch.NodesSearched;
		
		mEdgesSearched = mAstarSearch.EdgesSearched;

		DrawMovementPath ();
	}

	void OnDrawGizmos()
	{
		/*
		if (mSubTree != null && mPath.Count != 0) {
			Gizmos.color = Color.green;
			int nd = mPath[0];
			
			while ((nd != mSourceCellIndex) && (mSubTree[nd] != null) && mSubTree[nd].IsValidEdge()) 
			{
				Gizmos.DrawLine(mNavGraph.Nodes[mSubTree[nd].From].Position,mNavGraph.Nodes[nd].Position);
				nd = mSubTree[nd].From;
			}
		}
		*/
	}

	public void Move()
	{
		//mIsMoving = true;
		mCurrentWayPoint = mMovementPath.Count - 1;

		DrawMovementPath ();
	}

	private void DrawMovementPath()
	{
		if (mSubTree != null && mPath.Count != 0) {
			int nd = mPath[0];
			
			while ((nd != mSourceCellIndex) && (mSubTree[nd] != null) && mSubTree[nd].IsValidEdge()) 
			{
				Debug.DrawLine(mNavGraph.Nodes[mSubTree[nd].From].Position,mNavGraph.Nodes[nd].Position,Color.green, mCostToTarget / mSpeed);
				nd = mSubTree[nd].From;
			}
		}
	}

	private void RealMove()
	{
		if (mMovementPath == null) {
			//We have no path to move after yet
			return;
		}
		if (mCurrentWayPoint < 0) {
			Debug.Log ("End Of Path Reached");
			mIsMoving = false;
			return;
		}
		//Direction to the next waypoint
		Vector3 dir = (mMovementPath[mCurrentWayPoint] - transform.position).normalized;
		
		transform.LookAt (mMovementPath [mCurrentWayPoint]);

		Vector3 newposition = transform.position + dir * mSpeed * Time.deltaTime;
		transform.position = newposition;
		
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position, mMovementPath[mCurrentWayPoint]) < mNextWaypointDistance) {
			mCurrentWayPoint--;
			return;
		}
	}

	public void UpdateSearchInfo(int sourcerow, int sourcecolumn, int targetrow, int targetcolumn, float strickdistance)
	{
		Debug.Log (string.Format ("Source Index = [{0}][{1}]", sourcerow, sourcecolumn));
		
		Debug.Log (string.Format ("Target Index = [{0}][{1}]", targetrow, targetcolumn));
		
		mSourceCellIndex = Utility.ConvertRCToIndex (sourcerow, sourcecolumn);
		
		mTargetCellIndex = Utility.ConvertRCToIndex (targetrow, targetcolumn);
		
		mStrickDistance = strickdistance;
		
		Debug.Log ("mSourceCellIndex = " + mSourceCellIndex);
		
		Debug.Log ("mTargetCellIndex = " + mTargetCellIndex);
		
		Debug.Log ("mStrickDistance = " + mStrickDistance);
	}
}
