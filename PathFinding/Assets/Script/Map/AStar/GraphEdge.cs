using UnityEngine;
using System.Collections;

enum E_NODE_INDEX
{
	INVALID_NODE = -1
}

public class GraphEdge {
	public GraphEdge()
	{
		mFrom = (int)E_NODE_INDEX.INVALID_NODE;
		mTo = (int)E_NODE_INDEX.INVALID_NODE;
		mCost = 0.0f;
	}

	public GraphEdge(int from, int to, float cost)
	{
		mFrom = from;
		mTo = to;
		mCost = cost;
	}

	public bool IsValidEdge()
	{
		return (mFrom != (int)E_NODE_INDEX.INVALID_NODE || mTo != (int)E_NODE_INDEX.INVALID_NODE);
	}

	public int From {
		get {
			return mFrom;
		}
		set {
			Debug.Assert (value >= 0,"mFrom must great or equal to 0");
			mFrom = value;
		}
	}
	private int mFrom;

	public int To {
		get {
			return mTo;
		}
		set {
			Debug.Assert (value >= 0, "mTo must great or equal to 0");
			mTo = value;
		}
	}
	private int mTo;

	public float Cost
	{
		get
		{
			return mCost;
		}
		set
		{
			Debug.Assert (value >= 0, "mCost must great or equal to 0");
			mCost = value;
		}
	}
	private float mCost;
}
