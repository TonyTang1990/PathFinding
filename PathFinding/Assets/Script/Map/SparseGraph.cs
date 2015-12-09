using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class SparseGraph<T1,T2> where T1 : NavGraphNode where T2 : GraphEdge
{
	public SparseGraph()
	{
		mNextNodeIndex = 0;
		mNodes = new List<T1> ();
		mEdgesList = new List<List<T2>> ();
		mBDrawMap = true;
	}

	public SparseGraph(int nodesize)
	{
		mNextNodeIndex = 0;
		mNodes = new List<T1> (nodesize);
		mEdgesList = new List<List<T2>> (nodesize);
    	mBDrawMap = true;
    }

	public int AddNode(T1 node)
	{
		mNodes.Add(node);
		mEdgesList.Add (new List<T2> ());
		mNextNodeIndex++;
		return mNextNodeIndex;
	}

	public void RemoveNode(int node)
	{
		Assert.IsTrue (node >= 0 && node < mNextNodeIndex);
		mNodes.RemoveAt (node);
	}

	public void AddEdge(T2 edge)
	{
		Assert.IsTrue (edge.From < mNextNodeIndex && edge.To < mNextNodeIndex);
		if (UniqueEdge (edge.From, edge.To)) {
			mEdgesList[edge.From].Add (edge);
			if(mBDrawMap == true)
			{
				Debug.DrawLine(mNodes[edge.From].Position, mNodes[edge.To].Position, Color.red, Mathf.Infinity);
			}
		}
	}

	private bool UniqueEdge(int from, int to)
	{
		foreach(T2 t in mEdgesList[from])
		{
			if(t.To == to)
			{
				return false;
			}
		}
		return true;
	}

	public void RemmoveEdge(int from, int to)
	{

	}

	public T2 GetEdge(int from, int to)
	{
		Assert.IsTrue (from < mNextNodeIndex && to < mNextNodeIndex);
		foreach (T2 t in mEdgesList[from]) {
			if(t.To == to)
			{
				return t;
			}
		}
		Assert.IsTrue (false);
		return null;
	}

	public int NumNodes()
	{
		return mNodes.Count;
	}

	public int NumEdges()
	{
		int total = 0;
		foreach (List<T2> t in mEdgesList) {
			total += t.Count;
		}
		return total;
	}

	public bool IsEmpty()
	{
		return mNodes.Count == 0;
	}

	public int NumActiveNodes()
	{
		return 0;
	}

	public void Clear()
	{
		mNodes.Clear ();
		mEdgesList.Clear ();
	}

	public int GetnextFreeNodeIndex
	{
		get
		{
			return mNextNodeIndex;
		}
	}
	private int mNextNodeIndex;

	public List<T1> Nodes
	{
		get
		{
			return mNodes;
		}
		set
		{
			mNodes = value;
		}
	}
	private List<T1> mNodes;

	public List<List<T2>> EdgesList
	{
		get
		{
			return mEdgesList;
		}
		set
		{
			mEdgesList = value;
		}
	}
	public List<List<T2>> mEdgesList;

	public bool BDrawMap {
		get {
			return mBDrawMap;
		}
		set {
			mBDrawMap = value;
		}
	}
	private bool mBDrawMap;
}
