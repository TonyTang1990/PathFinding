using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

	public static MapManager MMInstance = null;

	public int mRow = 2;

	public int mColumn = 2;

	private PathFinder mPathFinder;

	void Awake()
	{
		if (MMInstance == null) {
			MMInstance = this;
		} else if (MMInstance != this) {
			Destroy (gameObject);
		}

		mPathFinder = gameObject.GetComponent<PathFinder> ();
	}

	void Start()
	{
		LoadMap ();
	}

	void LoadMap()
	{
		mPathFinder.CreteGraph (mRow, mColumn);
	}

	// Update is called once per frame
	void Update () {
		//DrawMap ();
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
