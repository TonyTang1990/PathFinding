using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class SearchAStar {

    public class PathInfo
    {
        public PathInfo()
        {
            ResetPathInfo();
        }

        public PathInfo DeepCopy()
        {
            PathInfo pi = (PathInfo)this.MemberwiseClone();
            pi.PathToTarget = new List<int>(mPathToTarget);
            pi.MovementPathToTarget = new List<Vector3>(mMovementPathToTarget);

            return pi;
        }

        public void ResetPathInfo()
        {
            mIsWallInPathToTarget = false;

            mWallInPathToTargetIndex = -1;

            if (mPathToTarget != null)
            {
                mPathToTarget.Clear();
            }
            else
            {
                mPathToTarget = new List<int>();
            }

            if (mMovementPathToTarget != null)
            {
                mMovementPathToTarget.Clear();
            }
            else
            {
                mMovementPathToTarget = new List<Vector3>();
            }

            mNodesSearched = 0;

            mEdgesSearched = 0;

            mCostToTarget = 0.0f;
        }

        public List<int> PathToTarget
        {
            get
            {
                return mPathToTarget;
            }
            set
            {
                mPathToTarget = value;
            }
        }
        private List<int> mPathToTarget;

        public List<Vector3> MovementPathToTarget
        {
            get
            {
                return mMovementPathToTarget;
            }
            set
            {
                mMovementPathToTarget = value;
            }
        }
        private List<Vector3> mMovementPathToTarget;

        public bool IsWallInPathToTarget
        {
            get
            {
                return mIsWallInPathToTarget;
            }
            set
            {
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

        public float CostToTarget
        {
            get
            {
                return mCostToTarget;
            }
            set
            {
                mCostToTarget = value;
            }
        }
        private float mCostToTarget;

        public int ITarget
        {
            set
            {
                mITarget = value;
            }
            get
            {
                return mITarget;
            }
        }
        private int mITarget;

        public int OriginalTarget
        {
            get
            {
                return mOriginalTarget;
            }
            set
            {
                mOriginalTarget = value;
            }
        }
        private int mOriginalTarget;

        public int NodesSearched
        {
            get
            {
                return mNodesSearched;
            }
            set
            {
                mNodesSearched = value;
            }
        }
        private int mNodesSearched;

        public int EdgesSearched
        {
            get
            {
                return mEdgesSearched;
            }
            set
            {
                mEdgesSearched = value;
            }
        }
        private int mEdgesSearched;

        //this list of edges is used to store any subtree returned from any of the graph algorithms
        /*
        public List<GraphEdge> SubTree
        {
            get
            {
                return mSubTree;
            }
            set
            {
                mSubTree = value;
            }
        }
        private List<GraphEdge> mSubTree;
        */
    }

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
        mPQ = new PriorityQueue<int, float>((int)Mathf.Sqrt(mGraph.NumNodes()));
		mGCosts = new List<float> (graph.NumNodes ());
		mFCosts = new List<Pair<int,float>> (graph.NumNodes ());
		mShortestPathTree = new List<GraphEdge> (graph.NumNodes());
		mSearchFrontier = new List<GraphEdge> (graph.NumNodes());
		//Init G cost and F cost and Cost value
		for (int i = 0; i < graph.NumNodes(); i++) {
			mGCosts.Add (0.0f);
			mFCosts.Add(new Pair<int,float>(i,0.0f));
			mShortestPathTree.Add(new GraphEdge());
			mSearchFrontier.Add(new GraphEdge());
		}
		mISource = source;
		mITarget = target;
		mOriginalTarget = target;
		
		Assert.IsTrue (hcostpercentage >= 0);
		mHCostPercentage = hcostpercentage;
		
		mBDrawExplorePath = drawexplorepath;
		
		mExplorePathRemainTime = explorepathremaintime;

        mAStarPathInfo = new PathInfo();	

		mIsIgnoreWall = isignorewall;

        mStrickDistance = strickdistance;

        //Search(mStrickDistance, mIsIgnoreWall);

        //GeneratePathToTargetInfo();
	}

    public void UpdateSearch(int sourceindex, int targetindex, float strickdistance)
    {
        Assert.IsTrue(mISource >= 0 && mISource < mGraph.NumNodes());
        Assert.IsTrue(mITarget >= 0 && mITarget < mGraph.NumNodes());

        AstarReset(sourceindex, targetindex, strickdistance);

        Search(mStrickDistance, mIsIgnoreWall);

        GeneratePathToTargetInfo();
    }

    private void AstarReset(int sourceindex, int targetindex, float strickdistance)
    {
        mISource = sourceindex;

        mITarget = targetindex;

        mOriginalTarget = targetindex;

        mStrickDistance = strickdistance;

        for (int i = 0; i < mGraph.NumNodes(); i++)
        {
            mGCosts[i] = 0.0f;
            mFCosts[i].Value = 0.0f;
            mShortestPathTree[i].Reset();
            mSearchFrontier[i].Reset();
        }

        mAStarPathInfo.ResetPathInfo();
    }

	private bool mIsIgnoreWall;

    public float StrickDistance
    {
        set
        {
            mStrickDistance = value;
        }
    }
    private float mStrickDistance;

    public PathInfo AStarPathInfo
    {
        get
        {
            return mAStarPathInfo;
        }
        set
        {
            mAStarPathInfo = value;
        }
    }
    private PathInfo mAStarPathInfo;

	private void GeneratePathToTargetInfo()
	{
		mAStarPathInfo.PathToTarget.Clear ();
        mAStarPathInfo.MovementPathToTarget.Clear();

		if (mITarget < 0) {
			return;
		}

		int nd = mITarget;

        mAStarPathInfo.PathToTarget.Add(nd);

        mAStarPathInfo.MovementPathToTarget.Add(mGraph.Nodes[nd].Position);

		while ((nd != mISource) && (mShortestPathTree[nd] != null) && mShortestPathTree[nd].IsValidEdge()) 
		{
			//Debug.DrawLine(mGraph.Nodes[mShortestPathTree[nd].From].Position,mGraph.Nodes[nd].Position,Color.green, Mathf.Infinity);

		    if(!mIsIgnoreWall)
			{
				if(mGraph.Nodes[nd].IsWall && !mGraph.Nodes[nd].IsJumpable)
				{
                    mAStarPathInfo.IsWallInPathToTarget = true;
                    mAStarPathInfo.WallInPathToTargetIndex = nd;
				}
			}

			nd = mShortestPathTree[nd].From;

            mAStarPathInfo.PathToTarget.Add(nd);

            mAStarPathInfo.MovementPathToTarget.Add(mGraph.Nodes[nd].Position);
		}

        mAStarPathInfo.CostToTarget = GetCostToTarget();

        mAStarPathInfo.ITarget = mITarget;

        mAStarPathInfo.OriginalTarget = mOriginalTarget;
	}

	public List<GraphEdge> GetSPT()
	{
		return mShortestPathTree;
	}

	private float GetCostToTarget()
	{
		return mGCosts[mITarget];
	}

	private SparseGraph<NavGraphNode,GraphEdge> mGraph;
    
    private PriorityQueue<int, float> mPQ;

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

    /*
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
    */
	private List<GraphEdge> mSearchFrontier;

    public int ISource
    {
        set
        {
            mISource = value;
        }
    }
	private int mISource;

	public int ITarget
	{
        set
        {
            mITarget = value;
        }
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
        set
        {
            mOriginalTarget = value;
        }
	}
	private int mOriginalTarget;

	private float mHCostPercentage;

	private bool mBDrawExplorePath;

	private float mExplorePathRemainTime;

	//The A* search algorithm
	private void Search()
	{
        mPQ.Clear();

        mPQ.Push(mFCosts[mISource]);

		//mSearchFrontier [mISource] = new GraphEdge (mISource, mISource, 0.0f);
        mSearchFrontier[mISource].From = mISource;
        mSearchFrontier[mISource].To = mISource;
        mSearchFrontier[mISource].Cost = 0.0f;

        while (!mPQ.Empty())
        {
			//Get lowest cost node from the queue
            int nextclosestnode = mPQ.Pop().Key;

            mAStarPathInfo.NodesSearched++;

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
            GraphEdge edge;
			for(int i = 0; i < edgelist.Count; i++)
			{
                edge = edgelist[i];
				//calculate the heuristic cost from this node to the target (H)
				float hcost = Heuristic_Euclid.Calculate(mGraph,mITarget,edge.To) * mHCostPercentage;

				//calculate the 'real' cost to this node from the source (G)
				float gcost = mGCosts[nextclosestnode] + edge.Cost;

				//if the node has not been added to the frontier, add it and update the G and F costs
				if(mSearchFrontier[edge.To] != null && !mSearchFrontier[edge.To].IsValidEdge())
				{
					mFCosts[edge.To].Value = gcost + hcost;
					mGCosts[edge.To] = gcost;

                    mPQ.Push(mFCosts[edge.To]);

					mSearchFrontier[edge.To] = edge;

                    mAStarPathInfo.EdgesSearched++;

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
                    mPQ.ChangePriority(edge.To);
				
					mSearchFrontier[edge.To] = edge;

                    mAStarPathInfo.EdgesSearched++;
				}
			}
		}
	}

	//The A* search algorithm with strickdistance
	private void Search(float strickdistance)
	{
		float currentnodetotargetdistance = Mathf.Infinity;

        mPQ.Clear();

        mPQ.Push(mFCosts[mISource]);
		
		//mSearchFrontier [mISource] = new GraphEdge (mISource, mISource, 0.0f);
        mSearchFrontier[mISource].From = mISource;
        mSearchFrontier[mISource].To = mISource;
        mSearchFrontier[mISource].Cost = 0.0f;

        while (!mPQ.Empty())
        {
			//Get lowest cost node from the queue
            int nextclosestnode = mPQ.Pop().Key;

            mAStarPathInfo.NodesSearched++;
			
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
			GraphEdge edge;
            for (int i = 0; i < edgelist.Count; i++ )
            {
                edge = edgelist[i];
                //calculate the heuristic cost from this node to the target (H)
                float hcost = Heuristic_Euclid.Calculate(mGraph, mITarget, edge.To) * mHCostPercentage;

                //calculate the 'real' cost to this node from the source (G)
                float gcost = mGCosts[nextclosestnode] + edge.Cost;

                //if the node has not been added to the frontier, add it and update the G and F costs
                if (mSearchFrontier[edge.To] != null && !mSearchFrontier[edge.To].IsValidEdge())
                {
                    mFCosts[edge.To].Value = gcost + hcost;
                    mGCosts[edge.To] = gcost;

                    mPQ.Push(mFCosts[edge.To]);

                    mSearchFrontier[edge.To] = edge;

                    mAStarPathInfo.EdgesSearched++;

                    if (mBDrawExplorePath)
                    {
                        Debug.DrawLine(mGraph.Nodes[edge.From].Position, mGraph.Nodes[edge.To].Position, Color.yellow, mExplorePathRemainTime);
                    }
                }

                //if this node is already on the frontier but the cost to get here
                //is cheaper than has been found previously, update the node
                //cost and frontier accordingly
                else if (gcost < mGCosts[edge.To])
                {
                    mFCosts[edge.To].Value = gcost + hcost;
                    mGCosts[edge.To] = gcost;

                    //Due to some node's f cost has been changed
                    //we should reoder the priority queue to make sure we pop up the lowest fcost node first
                    //compare the fcost will make sure we search the path in the right direction
                    //h cost is the key to search in the right direction
                    mPQ.ChangePriority(edge.To);

                    mSearchFrontier[edge.To] = edge;

                    mAStarPathInfo.EdgesSearched++;
                }
            }
		}
	}

	//The A* search algorithm with strickdistance with wall consideration
	private void Search(float strickdistance, bool isignorewall)
	{
		float currentnodetotargetdistance = Mathf.Infinity;

        mPQ.Clear();

        mPQ.Push(mFCosts[mISource]);
		
		//mSearchFrontier [mISource] = new GraphEdge (mISource, mISource, 0.0f);
        mSearchFrontier[mISource].From = mISource;
        mSearchFrontier[mISource].To = mISource;
        mSearchFrontier[mISource].Cost = 0.0f;

        while (!mPQ.Empty())
        {
			//Get lowest cost node from the queue
            int nextclosestnode = mPQ.Pop().Key;

            mAStarPathInfo.NodesSearched++;
			
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
			GraphEdge edge = new GraphEdge();
            for (int i = 0; i < edgelist.Count; i++ )
            {
                //Avoid pass refrence
                //edge = new GraphEdge(edgelist[i]);
                edge.From = edgelist[i].From;
                edge.To = edgelist[i].To;
                edge.Cost = edgelist[i].Cost;
                //calculate the heuristic cost from this node to the target (H)
                float hcost = Heuristic_Euclid.Calculate(mGraph, mITarget, edge.To) * mHCostPercentage;

                //calculate the 'real' cost to this node from the source (G)
                float gcost = 0.0f;
                if (isignorewall)
                {
                    gcost = mGCosts[nextclosestnode] + edge.Cost;

                    if (mGraph.Nodes[edge.From].IsWall)
                    {
                        gcost -= mGraph.Nodes[edge.From].Weight;
                    }
                    if (mGraph.Nodes[edge.To].IsWall)
                    {
                        gcost -= mGraph.Nodes[edge.To].Weight;
                    }
                }
                else
                {
                    gcost = mGCosts[nextclosestnode] + edge.Cost;
                    if (mGraph.Nodes[edge.From].IsJumpable)
                    {
                        gcost -= mGraph.Nodes[edge.From].Weight;
                    }
                    if (mGraph.Nodes[edge.To].IsJumpable)
                    {
                        gcost -= mGraph.Nodes[edge.To].Weight;
                    }
                }

                //if the node has not been added to the frontier, add it and update the G and F costs
                if (mSearchFrontier[edge.To] != null && !mSearchFrontier[edge.To].IsValidEdge())
                {
                    mFCosts[edge.To].Value = gcost + hcost;
                    mGCosts[edge.To] = gcost;

                    mPQ.Push(mFCosts[edge.To]);

                    mSearchFrontier[edge.To] = edge;

                    mAStarPathInfo.EdgesSearched++;

                    if (mBDrawExplorePath)
                    {
                        Debug.DrawLine(mGraph.Nodes[edge.From].Position, mGraph.Nodes[edge.To].Position, Color.yellow, mExplorePathRemainTime);
                    }
                }

                //if this node is already on the frontier but the cost to get here
                //is cheaper than has been found previously, update the node
                //cost and frontier accordingly
                else if (gcost < mGCosts[edge.To])
                {
                    mFCosts[edge.To].Value = gcost + hcost;
                    mGCosts[edge.To] = gcost;

                    //Due to some node's f cost has been changed
                    //we should reoder the priority queue to make sure we pop up the lowest fcost node first
                    //compare the fcost will make sure we search the path in the right direction
                    //h cost is the key to search in the right direction
                    mPQ.ChangePriority(edge.To);

                    mSearchFrontier[edge.To] = edge;

                    mAStarPathInfo.EdgesSearched++;
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