using UnityEngine;
using System.Collections;

public class NavGraphNode : GraphNode {

	public NavGraphNode()
	{

	}

	public NavGraphNode(int index,Vector3 pos)
	{
		Index = index;
		mPosition = pos;
	}

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
}
