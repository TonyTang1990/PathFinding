using UnityEngine;
using System.Collections;

public class TerrainNode : MonoBehaviour {

	private TerrainNode()
	{
		
	}
	
	public TerrainNode(int index,Vector3 pos, float weight)
	{
		Index = index;
		mPosition = pos;
		mWeight = weight;
		mIsJumpable = false;
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

	public bool IsWall
	{
		get
		{
			return mIsWall;
		}
		set
		{
			mIsWall = value;
		}
	}
	private bool mIsWall;

	public bool IsJumpable
	{
		get
		{
			return mIsJumpable;
		}
		set
		{
			mIsJumpable = value;
		}
	}
	private bool mIsJumpable;
}
