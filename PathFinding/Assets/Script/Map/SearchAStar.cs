using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class SearchAStar {

	private SearchAStar()
	{

	}

	public SearchAStar(SparseGraph<NavGraphNode,GraphEdge> graph
	                   ,int source
	                   ,int target
	                   ,bool isignorewall
	                   ,float strickdistance
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
		mOriginalTarget = target;
		
		mNodesSearched = 0;
		
		mEdgesSearched = 0;
		
		Assert.IsTrue (hcostpercentage >= 0);
		mHCostPercentage = hcostpercentage;
		
		mBDrawExplorePath = drawexplorepath;
		
		mExplorePathRemainTime = explorepathremaintime;
		
		mPathToTarget = new List<int> ();

		mMovementPathToTarget = new List<Vector3> ();

		mIsWallInPathToTarget = false;

		mWallInPathToTargetIndex = -1;

		mIsIgnoreWall = isignorewall;

		Search (strickdistance,isignorewall);

		GeneratePathToTargetInfo ();
	}

	public List<int> PathToTarget {
		get {
			return mPathToTarget;
		}
	}
	private List<int> mPathToTarget;

	public List<Vector3> MovementPathToTarget {
		get {
			return mMovementPathToTarget;
		}
	}
	private List<Vector3> mMovementPathToTarget;

	public bool IsWallInPathToTarget {
		get {
			return mIsWallInPathToTarget;
		}
		set {
			mIsWallInPathToTarget = value;
		}
	}
	private bool mIsWallInPathToTarget;

	public int WallInPathToTargetIndex
	{
		get
		{
			return mWallInPathToTargetIndex;
		}
		set
		{
			mWallInPathToTargetIndex = value;
		}
	}
	private int mWallInPathToTargetIndex;

	private bool mIsIgnoreWall;

	private void GeneratePathToTargetInfo()
	{
		mPathToTarget.Clear ();
		mMovementPathToTarget.Clear ();

		if (mITarget < 0) {
			return;
		}

		int nd = mITarget;

		mPathToTarget.Add (nd);

		mMovementPathToTarget.Add (mGraph.Nodes [nd].Position);

		while ((nd != mISource) && (mShortestPathTree[nd] != null) && mShortestPathTree[nd].IsValidEdge()) 
		{
			//Debug.DrawLine(mGraph.Nodes[mShortestPathTree[nd].From].Position,mGraph.Nodes[nd].Position,Color.green, Mathf.Infinity);

		    if(!mIsIgnoreWall)
			{
				if(mGraph.Nodes[nd].IsWall && !mGraph.Nodes[nd].IsJumpable)
				{
					mIsWallInPathToTarget = true;
					mWallInPathToTargetIndex = nd;
				}
			}

			nd = mShortestPathTree[nd].From;

			mPathToTarget.Add(nd);

			mMovementPathToTarget.Add(mGraph.Nodes[nd].Position);
		}
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

	public int ITarget
	{
		get
		{
			return mITarget;
		}
	}
	private int mITarget;

	public int OriginalTarget {
		get {
			return mOriginalTarget;
		}
	}
	private int mOriginalTarget;

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
		float numbernodes = (float)mGraph.NumNodes();
		PriorityQueue<int,float> pq = new PriorityQueue<int, float>((int)Mathf.Sqrt(numbernodes));

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

	//The A* search algorithm with strickdistance
	private void Search(float strickdistance)
	{
		float numbernodes = (float)mGraph.NumNodes();
		PriorityQueue<int,float> pq = new PriorityQueue<int, float>((int)Mathf.Sqrt(numbernodes));

		float currentnodetotargetdistance = Mathf.Infinity;

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

			currentnodetotargetdistance = Heuristic_Euclid.Calculate(mGraph,mITarget,nextclosestnode);

			if(nextclosestnode == mITarget || currentnodetotargetdistance <= strickdistance)
			{
				mITarget = nextclosestnode;
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

	//The A* search algorithm with strickdistance with wall consideration
	private void Search(float strickdistance, bool isignorewall)
	{
		float numbernodes = (float)mGraph.NumNodes();
		PriorityQueue<int,float> pq = new PriorityQueue<int, float>((int)Mathf.Sqrt(numbernodes));
		
		float currentnodetotargetdistance = Mathf.Infinity;
		
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
			
			currentnodetotargetdistance = Heuristic_Euclid.Calculate(mGraph,mITarget,nextclosestnode);
			
			if(nextclosestnode == mITarget || (currentnodetotargetdistance <= strickdistance && !mGraph.Nodes[nextclosestnode].IsWall))
			{
				mITarget = nextclosestnode;
				return;
			}
			
			//Now to test all the edges attached to this node
			List<GraphEdge> edgelist = mGraph.EdgesList[nextclosestnode];
			
			foreach(GraphEdge edge in edgelist)
			{
				//calculate the heuristic cost from this node to the target (H)
				float hcost = Heuristic_Euclid.Calculate(mGraph,mITarget,edge.To) * mHCostPercentage;
				
				//calculate the 'real' cost to this node from the source (G)
				float gcost = 0.0f;
				if(isignorewall)
				{
					gcost = mGCosts[nextclosestnode] + edge.Cost;

					if(mGraph.Nodes[edge.From].IsWall)
					{
						gcost -= mGraph.Nodes[edge.From].Weight;
					}
					if(mGraph.Nodes[edge.To].IsWall)
					{
						gcost -= mGraph.Nodes[edge.To].Weight;
					}
				}
				else
				{
					gcost = mGCosts[nextclosestnode] + edge.Cost;
					if(mGraph.Nodes[edge.From].IsJumpable)
					{
						gcost -= mGraph.Nodes[edge.From].Weight;
					}
					if(mGraph.Nodes[edge.To].IsJumpable)
					{
						gcost -= mGraph.Nodes[edge.To].Weight;
					}
				}

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