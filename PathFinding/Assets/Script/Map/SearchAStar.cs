using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class SearchAStar {

	public SearchAStar(SparseGraph<NavGraphNode,GraphEdge> graph
	                   ,int source
	                   ,int target
	                   ,float hcostpercentage
	                   ,bool drawexplorepath
	                   ,float explorepathremaintime)
	{
		mGraph = graph;
		mGCosts = new List<float> (graph.NumNodes ());
		mFCosts = new List<Pair<int,float>> (graph.NumNodes ());
		mShortestPathTree = new List<GraphEdge> (graph.NumNodes());
		mSearchFrontier = new List<GraphEdge> (graph.NumNodes());
		CostToTargetNode = new List<float> (graph.NumNodes());
		//Init G cost and F cost and Cost value
		for (int i = 0; i < graph.NumNodes(); i++) {
			mGCosts.Add (0.0f);
			mFCosts.Add(new Pair<int,float>(i,0.0f));
			mShortestPathTree.Add(new GraphEdge());
			mSearchFrontier.Add(new GraphEdge());
			CostToTargetNode.Add(0.0f);
		}
		mISource = source;
		mITarget = target;

		mNodesSearched = 0;

		mEdgesSearched = 0;

		Assert.IsTrue (hcostpercentage >= 0);
		mHCostPercentage = hcostpercentage;

		mBDrawExplorePath = drawexplorepath;

		mExplorePathRemainTime = explorepathremaintime;

		Search ();
	}

	public List<int> GetPathToTarget()
	{
		List<int> path = new List<int>();

		if (mITarget < 0) {
			return path;
		}

		int nd = mITarget;

		path.Add (nd);

		while ((nd != mISource) && (mShortestPathTree[nd] != null) && mShortestPathTree[nd].IsValidEdge()) 
		{
			//Debug.DrawLine(mGraph.Nodes[mShortestPathTree[nd].From].Position,mGraph.Nodes[nd].Position,Color.green, Mathf.Infinity);

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

	private List<Pair<int,float>> mFCosts;

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
		set
		{
			mCostToTargetNode = value;
		}
	}
	private List<float> mCostToTargetNode;

	private List<GraphEdge> mSearchFrontier;

	private int mISource;

	private int mITarget;

	public int NodesSearched
	{
		get
		{
			return mNodesSearched;
		}
	}
	private int mNodesSearched;

	public int EdgesSearched
	{
		get
		{
			return mEdgesSearched;
		}
	}
	private int mEdgesSearched;

	private float mHCostPercentage;

	private bool mBDrawExplorePath;

	private float mExplorePathRemainTime;

	//The A* search algorithm
	private void Search()
	{
		PriorityQueue<int,float> pq = new PriorityQueue<int, float>(MapManager.MMInstance.mRow);

		pq.Push (mFCosts [mISource]);

		mSearchFrontier [mISource] = new GraphEdge (mISource, mISource, 0.0f);

		while (!pq.Empty()) {
			//Get lowest cost node from the queue
			int nextclosestnode = pq.Pop().Key;

			mNodesSearched++;

			//move this node from the frontier to the spanning tree
			if(mSearchFrontier[nextclosestnode] != null && mSearchFrontier[nextclosestnode].IsValidEdge())
			{
				mShortestPathTree[nextclosestnode] = mSearchFrontier[nextclosestnode];
			}
			//If the target has been found exit
			if(nextclosestnode == mITarget)
			{
				return;
			}

			//Now to test all the edges attached to this node
			List<GraphEdge> edgelist = mGraph.EdgesList[nextclosestnode];

			foreach(GraphEdge edge in edgelist)
			{
				//calculate the heuristic cost from this node to the target (H)
				float hcost = Heuristic_Euclid.Calculate(mGraph,mITarget,edge.To) * mHCostPercentage;

				//calculate the 'real' cost to this node from the source (G)
				float gcost = mGCosts[nextclosestnode] + edge.Cost;

				//if the node has not been added to the frontier, add it and update the G and F costs
				if(mSearchFrontier[edge.To] != null && !mSearchFrontier[edge.To].IsValidEdge())
				{
					mFCosts[edge.To].Value = gcost + hcost;
					mGCosts[edge.To] = gcost;

					pq.Push(mFCosts[edge.To]);

					mSearchFrontier[edge.To] = edge;

					mEdgesSearched++;

					if(mBDrawExplorePath)
					{
						Debug.DrawLine(mGraph.Nodes[edge.From].Position,mGraph.Nodes[edge.To].Position,Color.yellow, mExplorePathRemainTime);
					}
				}

				//if this node is already on the frontier but the cost to get here
				//is cheaper than has been found previously, update the node
				//cost and frontier accordingly
				else if(gcost < mGCosts[edge.To])
				{
					mFCosts[edge.To].Value = gcost + hcost;
					mGCosts[edge.To] = gcost;

					//Due to some node's f cost has been changed
					//we should reoder the priority queue to make sure we pop up the lowest fcost node first
					//compare the fcost will make sure we search the path in the right direction
					//h cost is the key to search in the right direction
					pq.ChangePriority(edge.To);
				
					mSearchFrontier[edge.To] = edge;

					mEdgesSearched++;
				}
			}
		}
	}
}

class Heuristic_Euclid
{
	public static float Calculate(SparseGraph<NavGraphNode,GraphEdge> g, int nd1, int nd2)
	{
		//Manhattan distance heuritic
		//Vector2 v1 = Utility.ConvertIndexToRC (nd1);
		//Vector2 v2 = Utility.ConvertIndexToRC (nd2);
		//float dis = v1.x - v2.x + v1.y - v2.y;
		//Debug.Log("dis = " + dis);
		//return dis;
		//Caculation distance takes much time
		return Vector3.Distance(g.Nodes[nd1].Position, g.Nodes[nd2].Position);
	}
}