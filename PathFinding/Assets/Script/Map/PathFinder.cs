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
		mNavGraph = null;
		mPath = new List<int> ();
		mSubTree = new List<GraphEdge> ();

		//A Star Info
		mTotalNodes = 0;
		mTotalEdges = 0;
		mTimeTaken = 0;
		mNodesSearched = 0;
		mEdgesSearched = 0;
	}

	public void CreteGraph(int row, int column)
	{
		mNavGraph = new SparseGraph<NavGraphNode, GraphEdge> ();
		mPath.Clear ();
		mSubTree.Clear ();
		mTimeTaken = 0;

		//SparseGraph nodes data
		for (int rw = 0; rw < row; rw++)
		{
			for(int col = 0; col < column; col++)
			{
				mNavGraph.AddNode(new NavGraphNode(mNavGraph.GetnextFreeNodeIndex,new Vector3(rw,0.0f,col)));
			}
		}

		//SparseGraph edges data
		for (int rw = 0; rw < row; rw++) 
		{
			for (int col = 0; col < column; col++) 
			{
				CreateAllNeighboursToGridNode(rw, col, row, column);
			}
		}

		mTotalNodes = mNavGraph.NumNodes();
		mTotalEdges = mNavGraph.NumEdges ();
	}

	private void CreateAllNeighboursToGridNode(int row, int col, int totalrow, int totalcolumn)
	{
		int noderow = 0;
		int nodecol = 0;
		for (int i=-1; i<2; ++i) 
		{
			for (int j=-1; j<2; ++j) 
			{
				noderow =  row + i;
				nodecol = col + j;
				//Skip if equal to this node
				if((i == 0) && (j == 0))
				{
					continue;
				}

				//Check to see if this is a valid neighbour
				if(ValidNeighbour(noderow, nodecol, totalrow, totalcolumn))
				{
					Vector3 posnode = mNavGraph.Nodes[row * totalcolumn + col].Position;
					Vector3 posneighbour = mNavGraph.Nodes[noderow * totalcolumn + nodecol].Position;

					float dist = Vector3.Distance(posnode, posneighbour);

					GraphEdge newedge = new GraphEdge(row * totalcolumn + col, noderow * totalcolumn + nodecol, dist);
					mNavGraph.AddEdge(newedge);
				}
			}
		}
	}

	private bool ValidNeighbour(int x, int y, int row, int col)
	{
		return !(x < 0 || y < 0 || x >= row || y >= col);
	}

	public void CreatePathAStar()
	{
		float time_start = Time.realtimeSinceStartup;

		SearchAStar astarsearch = new SearchAStar (mNavGraph, mSourceCellIndex, mTargetCellIndex);

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
	public float TimeTaken {
		get {
			return mTimeTaken;
		}
		set {
			mTimeTaken = value;
		}
	}
	private float mTimeTaken;

	//A Star Info
	public int TotalNodes {
		get {
			return mTotalNodes;
		}
		set {
			mTotalNodes = value;
		}
	}
	private int mTotalNodes;

	public int TotalEdges {
		get {
			return mTotalEdges;
		}
		set {
			mTotalEdges = value;
		}
	}
	private int mTotalEdges;

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
	private int mSourceCellIndex;

	public int TargetCellIndex {
		get {
			return mTargetCellIndex;
		}
		set {
			mTargetCellIndex = value;
		}
	}
	private int mTargetCellIndex;

	private void UpdateAlgorithm()
	{

	}
}
