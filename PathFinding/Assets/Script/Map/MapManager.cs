using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public static MapManager MMInstance = null;

	public int mRow = 2;

	public int mColumn = 2;

	public NavGraphNode CurrentSelectedNode {
		get {
			return mCurrentSelectedNode;
		}
		set {
			mCurrentSelectedNode = value;
		}
	}
	private NavGraphNode mCurrentSelectedNode;

	public PathFinder PathFinder
	{
		get
		{
			return mPathFinder;
		}
	}
	private PathFinder mPathFinder;

	public List<GameObject> NodeWeightList {
		get {
			return mNodeWeightList;
		}
	}
	private List<GameObject> mNodeWeightList;
	
	public GameObject mNodeWeight;

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

		LoadNodeWeights ();
	}

	private void LoadNodeWeights()
	{
		GameObject temp = new GameObject ();
		mNodeWeightList = new List<GameObject> (mRow * mColumn);
		
		Vector3 nodeposition = new Vector3 ();
		int nextindex = 0;
		//SparseGraph nodes data
		for (int rw = 0; rw < mRow; rw++) {
			for (int col = 0; col < mColumn; col++) {
				nodeposition = new Vector3 (rw, 0.0f, col);
				temp = Instantiate (mNodeWeight, nodeposition, Quaternion.Euler (new Vector3 (90.0f, 45.0f, 0.0f))) as GameObject;
				temp.name = nextindex.ToString();
				temp.GetComponent<NavGraphNode> ().Weight = 0.0f;
				temp.GetComponent<NavGraphNode> ().Index = nextindex;
				temp.GetComponent<NavGraphNode> ().Position = nodeposition;
				mNodeWeightList.Add (temp);
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

	public void UpdateNodeWeight()
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			mNodeWeightList [index].GetComponent<TextMesh> ().text = CurrentSelectedNode.Weight.ToString ();
		}
	}

}
