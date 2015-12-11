using UnityEngine;
using System.Collections;

public class NavGraphNode : MonoBehaviour, GraphNode {

	public NavGraphNode()
	{

	}

	public NavGraphNode(int index,Vector3 pos, float weight)
	{
		Index = index;
		mPosition = pos;
		mWeight = weight;
	}
	
	public int Index
	{
		get
		{
			return mIndex;
		}
		set
		{
			mIndex = value;
		}
	}
	private int mIndex;

	public Vector3 Position
	{
		get
		{
			return mPosition;
		}
		set
		{
			mPosition = value;
		}
	}
	private Vector3 mPosition;

	public float Weight
	{
		get
		{
			return mWeight;
		}
		set
		{
			mWeight = value;
		}
	}
	private float mWeight;
}
