using UnityEngine;
using System.Collections;

public class NodeWeight : MonoBehaviour {

	public NodeWeight()
	{
		
	}
	
	public NodeWeight(int index,Vector3 pos, float weight)
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
