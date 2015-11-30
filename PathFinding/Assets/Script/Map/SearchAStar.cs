using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchAStar {

	public static float Calculate(SparseGraph<NavGraphNode,GraphEdge> g,  int nd1, int nd2)
	{
		return 0.0f;
	}

	public SearchAStar(SparseGraph<NavGraphNode,GraphEdge> graph
	                   ,int source
	                   ,int target)
	{
		mGraph = graph;
		mShortestPathTree = new List<GraphEdge> (graph.NumNodes);
		mSearchFrontier = new List<GraphEdge> (graph.NumEdges);
		CostToTargetNode = new List<float> (graph.NumNodes);
		mISource = source;
		mITarget = target;

		Search ();
	}

	public List<int> GetPathToTarget()
	{
		List<int> path;

		if (mITarget < 0) {
			return path;
		}

		int nd = mITarget;

		path.Add (nd);

		while ((nd != mISource) && (mShortestPathTree[nd] != 0)) 
		{
			nd = mShortestPathTree[nd].From;

			path.Add(nd);
		}

		return path;
	}

	public List<GraphEdge> GetSPT()
	{
		return mShortestPathTree;
	}

	public float GetCostToTarget()
	{
		return mGCosts[mITarget];
	}

	private SparseGraph<NavGraphNode,GraphEdge> mGraph;

	private List<float> mGCosts;

	private List<float> mFCosts;

	public List<GraphEdge> SPT
	{
		get
		{
			return mShortestPathTree;
		}
	}
	private List<GraphEdge> mShortestPathTree;

	public List<float> CostToTargetNode 
	{
		get {
			return mCostToTargetNode;
		}
	}
	private List<float> mCostToTargetNode;

	private List<GraphEdge> mSearchFrontier;

	private int mISource;

	private int mITarget;

	//The A* search algorithm
	private void Search()
	{

	}
}

class Heuristic_Euclid
{
	public static float Calculate(SparseGraph<NavGraphNode,GraphEdge> g, int nd1, int nd2)
	{
		return 0.0f;
	}
}