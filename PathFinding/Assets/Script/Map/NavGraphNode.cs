using UnityEngine;
using System.Collections;

public class NavGraphNode : GraphNode {
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
