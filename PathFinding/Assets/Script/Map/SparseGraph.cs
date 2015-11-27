using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SparseGraph<T1,T2> where T1 : GraphNode where T2 : GraphEdge
{
	public SparseGraph()
	{
		mNextNodeIndex = 0;
	}

	public int AddNode(T1 node)
	{
		mNodes.Add(node);
		mNextNodeIndex++;
		return mNextNodeIndex;
	}

	public void RemoveNode(int node)
	{

	}

	public void AddEdge(T2 edge)
	{
		mEdges.Add (edge);
	}

	public void RemmoveEdge(int from, int to)
	{

	}

	public T2 GetEdge(int from, int to)
	{
		return mEdges[0];
	}

	public int NumNodes()
	{
		return mNodes.Count;
	}

	public int NumEdges()
	{
		return mEdges.Count;
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
		mEdges.Clear ();
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

	public List<T2> Edges {
		get {
			return mEdges;
		}
		set {
			mEdges = value;
		}
	}
	private List<T2> mEdges;

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


}
