using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {
	public PathFinder()
	{
		mStart = false;
		mFinish = false;
		mCowNum = 0;
		mColumnNum = 0;
		mCostToTarget = 0;
		mTimeTaken = 0;
	}

	public void CreteGraph(int cow, int column)
	{

	}

	public void CreatePathAStar()
	{
		float time_start = Time.realtimeSinceStartup;

		SearchAStar astarsearch = new SearchAStar (mNavGraph, mCowNum, mColumnNum);

		float time_end = Time.realtimeSinceStartup;

		mTimeTaken = time_end - time_start;

		mPath = astarsearch.GetPathToTarget ();

		mSubTree = astarsearch.GetSPT ();

		mCostToTarget = astarsearch.GetCostToTarget ();
	}

	private List<int> mTerrainType;

	//this list will store any path returned from a graph search
	private List<int> mPath;

	private SparseGraph<NavGraphNode, GraphEdge> mNavGraph;

	//this list of edges is used to store any subtree returned from any of the graph algorithms
	private List<GraphEdge> mSubTree;

	private float mCostToTarget;

	private int mCowNum;

	private int mColumnNum;

	//Flags to indicate if the start and finish points has been added
	private bool mStart;

	private bool mFinish;

	//Holds the time taken for the currently used algorithm to complete
	private float mTimeTaken;

	private void UpdateAlgorithm()
	{

	}
}
