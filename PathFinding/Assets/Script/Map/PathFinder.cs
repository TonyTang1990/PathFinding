using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions;

public class PathFinder : MonoBehaviour {
	
	public int mRow = 2;
	
	public int mColumn = 2;

	private List<int> mTerrainType;

	public SparseGraph<NavGraphNode, GraphEdge> NavGraph {
		get {
			return mNavGraph;
		}
	}
	private SparseGraph<NavGraphNode, GraphEdge> mNavGraph;

	public bool mBDrawMap = true;

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

	void Awake()
	{
		mNavGraph = null;
		mTotalNodes = 0;
		mTotalEdges = 0;
		
		TimerCounter.CreateInstance().Restart("CreateGraph");
		CreateGraph ();
		TimerCounter.CreateInstance ().End ();
	}

	public void CreateGraph(/*int row, int column, float nodedistance, List<NavGraphNode> nodelist*/)
	{
		mNavGraph = new SparseGraph<NavGraphNode, GraphEdge> (mRow * mColumn);
		
		mNavGraph.BDrawMap = mBDrawMap;

		Vector3 nodeposition = new Vector3 ();
		int nextindex = 0;
		//SparseGraph nodes data
		for (int rw = 0; rw < mRow; rw++) {
			for (int col = 0; col < mColumn; col++) {
				nodeposition = new Vector3 (rw, 0.0f, col);
				nextindex = mNavGraph.NextFreeNodeIndex;
				mNavGraph.AddNode (new NavGraphNode (nextindex, nodeposition, 0.0f, false));
			}
		}

		//SparseGraph edges data
		for (int rw = 0; rw < mRow; rw++) 
		{
			for (int col = 0; col < mColumn; col++) 
			{
				CreateAllNeighboursToGridNode(rw, col, mRow, mColumn);
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
					float fromnodeweight = mNavGraph.Nodes[row * totalcolumn + col].Weight;
					float tonodeweight = mNavGraph.Nodes[noderow * totalcolumn + nodecol].Weight;
					dist = dist + fromnodeweight + tonodeweight;

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



	private bool IsValidIndex(int index)
	{
		if (index >= 0 && index < mTotalNodes) {
			return true;
		} else {
			return false;
		}
	}

	public void UpdateNodeEdgesInfo(int index,float value)
	{
		Assert.IsTrue (index >= 0 && index < mNavGraph.NextFreeNodeIndex);
		//Update Nodes weight first
		mNavGraph.Nodes [index].Weight += value;

		//Update edge info that starts from Node[index]
		GraphEdge e;
        for (int i = 0; i < mNavGraph.mEdgesList[index].Count; i++) {
            e =  mNavGraph.mEdgesList[index][i];
			e.Cost += value;
		}
		//Update edge info that ends with Node[index]
		int fromindex = 0;
		GraphEdge edge = new GraphEdge ();
		for (int i = -1; i <= 1; i++) {
			for(int j = -1; j <= 1; j++)
			{
				fromindex = index + j + i * mColumn;
				if(IsValidIndex(fromindex) && fromindex != index)
				{
					edge = mNavGraph.mEdgesList[fromindex].Find( x => x.To == index);
					if(edge != null)
					{
						edge.Cost += value;
					}
				}
			}
		}
	}

	public void UpdateNodeWallStatus(int index,bool iswall)
	{
		Assert.IsTrue (index >= 0 && index < mNavGraph.NextFreeNodeIndex);
		mNavGraph.Nodes [index].IsWall = iswall;
	}

	public void UpdateNodeWallJumpableStatus(int index,bool iswalljumpable)
	{
		Assert.IsTrue (index >= 0 && index < mNavGraph.NextFreeNodeIndex);
		if (mNavGraph.Nodes [index].IsWall) {
			mNavGraph.Nodes [index].IsJumpable = iswalljumpable;
		}
	}

	private void UpdateAlgorithm()
	{

	}
}
