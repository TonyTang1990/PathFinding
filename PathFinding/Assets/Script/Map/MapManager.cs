using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public static MapManager MMInstance = null;

	public TerrainNode CurrentSelectedNode {
		get {
			return mCurrentSelectedNode;
		}
		set {
			mCurrentSelectedNode = value;
		}
	}
	private TerrainNode mCurrentSelectedNode;

	public PathFinder PathFinder
	{
		get
		{
			return mPathFinder;
		}
	}
	private PathFinder mPathFinder;

	public List<GameObject> NodeTerrainListObject {
		get {
			return mNodeTerrainListObject;
		}
	}
	private List<GameObject> mNodeTerrainListObject;

	public List<TerrainNode> NodeTerrainList {
		get {
			return mNodeTerrainList;
		}
	}
	private List<TerrainNode> mNodeTerrainList;

	public GameObject mNodeWeight;

	private int mRow = 1;
	
	private int mColumn = 1;

	private float mNodeDistance = 1.0f;

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
		mRow = mPathFinder.mRow;
		mColumn = mPathFinder.mColumn;
		mNodeDistance = mPathFinder.mNodeDistance;
		LoadNodeWeights ();
	}

	private void LoadNodeWeights()
	{
		mNodeTerrainListObject = new List<GameObject> ( mRow * mColumn);
		mNodeTerrainList = new List<TerrainNode> (mRow * mColumn);

		GameObject tempobject = new GameObject ();
		TerrainNode tempnode;

		Vector3 nodeposition = new Vector3 ();
		int nextindex = 0;
		//SparseGraph nodes data
		for (int rw = 0; rw < mRow; rw++) {
			for (int col = 0; col < mColumn; col++) {
				nodeposition = new Vector3 (rw * mNodeDistance, 0.0f, col * mNodeDistance);
				tempobject = Instantiate (mNodeWeight, nodeposition, Quaternion.Euler (new Vector3 (90.0f, 0.0f, 0.0f))) as GameObject;
				tempobject.name = nextindex.ToString();
				tempnode = tempobject.GetComponent<TerrainNode>();
				tempnode.Weight = 0.0f;
				tempnode.Index = nextindex;
				tempnode.Position = nodeposition;
				mNodeTerrainListObject.Add (tempobject);
				mNodeTerrainList.Add(tempnode);
				nextindex++;
			}
		}
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
		//mPathFinder.CreatePathAStar ();
		GameManager.mGameInstance.AttackingSoldierSeeker.CreatePathAStar ();

		UIManager.UIMInstance.UpdateAstarInfo();
	}
	
	public void UpdateSearchInfo(int sourcerow, int sourcecolumn, int targetrow, int targetcolumn, float strickdistance)
	{
		Debug.Log (string.Format ("Source Index = [{0}][{1}]", sourcerow, sourcecolumn));

        Debug.Log (string.Format ("Target Index = [{0}][{1}]", targetrow, targetcolumn));

		GameManager.mGameInstance.AttackingSoldierSeeker.SourceCellIndex = Utility.ConvertRCToIndex (sourcerow, sourcecolumn);

		GameManager.mGameInstance.AttackingSoldierSeeker.TargetCellIndex = Utility.ConvertRCToIndex (targetrow, targetcolumn);

		GameManager.mGameInstance.AttackingSoldierSeeker.StrickDistance = strickdistance;

		Debug.Log ("GameManager.mGameInstance.AttackingSoldierSeeker.SourceCellIndex = " + GameManager.mGameInstance.AttackingSoldierSeeker.SourceCellIndex);

		Debug.Log ("GameManager.mGameInstance.AttackingSoldierSeeker.TargetCellIndex = " + GameManager.mGameInstance.AttackingSoldierSeeker.TargetCellIndex);

		Debug.Log ("GameManager.mGameInstance.AttackingSoldierSeeker.StrickDistance = " + GameManager.mGameInstance.AttackingSoldierSeeker.StrickDistance);
	}

	public void UpdateNodeWeight(float value)
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			Transform nodeweight = mNodeTerrainListObject [index].transform.FindChild("Node_Weight"); 
			nodeweight.gameObject.GetComponent<TextMesh> ().text = CurrentSelectedNode.Weight.ToString ();
			//Update Edge info
			mPathFinder.UpdateNodeEdgesInfo(index, value);
		}
	}

	public void UpdateNodeWallStatus(bool iswall)
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			mPathFinder.UpdateNodeWallStatus(index, iswall);
		}
	}

	public void UpdateNodeWallJumpableStatus(bool iswalljumpable)
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			mPathFinder.UpdateNodeWallJumpableStatus(index, iswalljumpable);
		}
	}
}
