using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

	public int mRow = 2;

	public int mColumn = 2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DrawMap ();
	}

	void DrawMap()
	{
		//Debug.DrawLine (new Vector3 (0.0f, 0.0f, 0.0f), new Vector3 (mRow, 0.0f, mColumn), Color.red);

	}

	void OnDrawGizmosSelected()
	{
		for (int i = 0; i <= mRow; i++) 
		{
			for(int j = 0; j <= mColumn; j++)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(new Vector3(i,0.0f,j),0.05f);
			}
		}
	}
}
