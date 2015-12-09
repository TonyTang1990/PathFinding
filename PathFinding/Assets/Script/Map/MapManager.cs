using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

	public static MapManager MMInstance = null;

	public int mRow = 2;

	public int mColumn = 2;

	public PathFinder PathFinder
	{
		get
		{
			return mPathFinder;
		}
	}
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

		UIManager.UIMInstance.UpdateAstarInfo ();
	}

	void LoadMap()
	{
		TimerCounter.CreateInstance().Restart("CreateGraph");
		mPathFinder.CreteGraph (mRow, mColumn);
		TimerCounter.CreateInstance ().End ();
	}

	// Update is called once per frame
	void Update () {
		//DrawMap ();
	}

	void DrawMap()
	{
		//Debug.DrawLine (new Vector3 (0.0f, 0.0f, 0.0f), new Vector3 (mRow, 0.0f, mColumn), Color.red);
	}

	public void Search()
	{
		mPathFinder.CreatePathAStar ();

		UIManager.UIMInstance.UpdateAstarInfo();
	}
	
	public void UpdateSearchInfo(int sourcerow, int sourcecolumn, int targetrow, int targetcolumn)
	{
		Debug.Log (string.Format ("Source Index = [{0}][{1}]", sourcerow, sourcecolumn));

        Debug.Log (string.Format ("Target Index = [{0}][{1}]", targetrow, targetcolumn));

		mPathFinder.SourceCellIndex = Utility.ConvertRCToIndex (sourcerow, sourcecolumn);

		mPathFinder.TargetCellIndex = Utility.ConvertRCToIndex (targetrow, targetcolumn);

		Debug.Log ("mPathFinder.SourceCellIndex = " + mPathFinder.SourceCellIndex);

		Debug.Log ("mPathFinder.TargetCellIndex = " + mPathFinder.TargetCellIndex);
	}

	void OnDrawGizmos()
	{
		/*
		for (int i = 0; i < mRow; i++) 
		{
			for(int j = 0; j < mColumn; j++)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(new Vector3(i,0.0f,j),0.05f);
			}
		}
		*/
	}
}
