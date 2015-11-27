using UnityEngine;
using System.Collections;

public class GraphEdge {
	public GraphEdge(int from, int to, float cost)
	{
		mFrom = from;
		mTo = to;
		mCost = cost;
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
