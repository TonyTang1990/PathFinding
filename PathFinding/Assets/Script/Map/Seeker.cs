using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Seeker : MonoBehaviour {
    
	private float mSpeed = 1.0f;

	public float mNextWaypointDistance = 0.2f;

	private PathFinder mPathFinder;

	//this list will store any path returned from a graph search
    public SearchAStar.PathInfo SeekerPathInfo
    {
        get
        {
            return mSeekerPathInfo;
        }
        set
        {
            mSeekerPathInfo = value;
        }
    }
    private SearchAStar.PathInfo mSeekerPathInfo;
    /*
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
	*/

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

	public bool mBDrawMovementPath = false;

	public bool mBDrawExplorePath = false;
	
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

    private List<GraphEdge> mSubTree;

	void Awake()
	{
		mIsMoving = false;

        mSeekerPathInfo = new SearchAStar.PathInfo();

        mTimeTaken = 0;

	}

	// Use this for initialization
	void Start () {
		mPathFinder = FindObjectOfType (typeof(PathFinder)) as PathFinder;
		Debug.Assert (mPathFinder != null);
		mNavGraph = mPathFinder.NavGraph;
        mAstarSearch = new SearchAStar(mNavGraph, mSourceCellIndex, mTargetCellIndex, mIgnoreWall, mStrickDistance, mHCostPercentage, mBDrawExplorePath, mExplorePathRemainTime);
        Debug.Assert(mNavGraph != null);
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
        //mAstarSearch = new SearchAStar(mNavGraph, mSourceCellIndex, mTargetCellIndex, mIgnoreWall, mStrickDistance, mHCostPercentage, mBDrawExplorePath, mExplorePathRemainTime);
        mAstarSearch.UpdateSearch(mSourceCellIndex, mTargetCellIndex, mStrickDistance);

		TimerCounter.CreateInstance ().End ();

		Utility.Log ("mAstarSearch.ITarget = " + mAstarSearch.ITarget);
		Utility.Log ("mAstarSearch.IsWallInPathToTarget = " + mAstarSearch.AStarPathInfo.IsWallInPathToTarget);
        Utility.Log("mAstarSearch.WallInPathToTargetIndex = " + mAstarSearch.AStarPathInfo.WallInPathToTargetIndex);

		mTimeTaken = TimerCounter.CreateInstance ().TimeSpend;

        mSeekerPathInfo = mAstarSearch.AStarPathInfo;

        mSubTree = mAstarSearch.GetSPT();

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
		mCurrentWayPoint = mSeekerPathInfo.MovementPathToTarget.Count - 1;

		DrawMovementPath ();
	}

	private void DrawMovementPath()
	{
		if (mBDrawMovementPath) {
            if (mSubTree != null && mSeekerPathInfo.PathToTarget.Count != 0)
            {
                int nd = mSeekerPathInfo.PathToTarget[0];

                while ((nd != mSourceCellIndex) && (mSubTree[nd] != null) && mSubTree[nd].IsValidEdge())
                {
                    Debug.DrawLine(mNavGraph.Nodes[mSubTree[nd].From].Position, mNavGraph.Nodes[nd].Position, Color.green, mSeekerPathInfo.CostToTarget / mSpeed);
                    nd = mSubTree[nd].From;
				}
			}
		}
	}

	private void RealMove()
	{
		if (mSeekerPathInfo.MovementPathToTarget == null) {
			//We have no path to move after yet
			return;
		}
		if (mCurrentWayPoint < 0) {
			Utility.Log ("End Of Path Reached");
			mIsMoving = false;
			return;
		}
		//Direction to the next waypoint
        Vector3 dir = (mSeekerPathInfo.MovementPathToTarget[mCurrentWayPoint] - transform.position).normalized;

        transform.LookAt(mSeekerPathInfo.MovementPathToTarget[mCurrentWayPoint]);

		Vector3 newposition = transform.position + dir * mSpeed * Time.deltaTime;
		transform.position = newposition;
		
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, mSeekerPathInfo.MovementPathToTarget[mCurrentWayPoint]) < mNextWaypointDistance)
        {
			mCurrentWayPoint--;
			return;
		}
	}

	public void UpdateSearchInfo(int sourcerow, int sourcecolumn, int targetrow, int targetcolumn, float strickdistance)
	{
		Utility.Log (string.Format ("Source Index = [{0}][{1}]", sourcerow, sourcecolumn));
		
		Utility.Log (string.Format ("Target Index = [{0}][{1}]", targetrow, targetcolumn));
		
		mSourceCellIndex = Utility.ConvertRCToIndex (sourcerow, sourcecolumn);
		
		mTargetCellIndex = Utility.ConvertRCToIndex (targetrow, targetcolumn);
		
		mStrickDistance = strickdistance;
		
		Utility.Log ("mSourceCellIndex = " + mSourceCellIndex);
		
		Utility.Log ("mTargetCellIndex = " + mTargetCellIndex);
		
		Utility.Log ("mStrickDistance = " + mStrickDistance);
	}
}
