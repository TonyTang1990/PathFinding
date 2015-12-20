using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public static MapManager MMInstance = null;

	public int mRow = 2;

	public int mColumn = 2;

	public float mNodeDistance = 1.0f;

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

	public List<GameObject> NodeWeightListObject {
		get {
			return mNodeWeightListObject;
		}
	}
	private List<GameObject> mNodeWeightListObject;

	public List<NavGraphNode> NodeWeightList {
		get {
			return mNodeWeightList;
		}
	}
	private List<NavGraphNode> mNodeWeightList;

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
		LoadNodeWeights ();

		TimerCounter.CreateInstance().Restart("CreateGraph");
		mPathFinder.CreteGraph (mRow, mColumn, mNodeDistance, mNodeWeightList);
		TimerCounter.CreateInstance ().End ();
	}

	private void LoadNodeWeights()
	{
		mNodeWeightListObject = new List<GameObject> (mRow * mColumn);
		mNodeWeightList = new List<NavGraphNode> (mRow * mColumn);

		GameObject tempobject = new GameObject ();
		NavGraphNode tempnode = new NavGraphNode ();

		Vector3 nodeposition = new Vector3 ();
		int nextindex = 0;
		//SparseGraph nodes data
		for (int rw = 0; rw < mRow; rw++) {
			for (int col = 0; col < mColumn; col++) {
				nodeposition = new Vector3 (rw * mNodeDistance, 0.0f, col * mNodeDistance);
				tempobject = Instantiate (mNodeWeight, nodeposition, Quaternion.Euler (new Vector3 (90.0f, 45.0f, 0.0f))) as GameObject;
				tempobject.name = nextindex.ToString();
				tempnode = tempobject.GetComponent<NavGraphNode>();
				tempnode.Weight = 0.0f;
				tempnode.Index = nextindex;
				tempnode.Position = nodeposition;
				mNodeWeightListObject.Add (tempobject);
				mNodeWeightList.Add(tempnode);
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
	
	public void UpdateSearchInfo(int sourcerow, int sourcecolumn, int targetrow, int targetcolumn, float strickdistance)
	{
		Debug.Log (string.Format ("Source Index = [{0}][{1}]", sourcerow, sourcecolumn));

        Debug.Log (string.Format ("Target Index = [{0}][{1}]", targetrow, targetcolumn));

		mPathFinder.SourceCellIndex = Utility.ConvertRCToIndex (sourcerow, sourcecolumn);

		mPathFinder.TargetCellIndex = Utility.ConvertRCToIndex (targetrow, targetcolumn);

		mPathFinder.StrickDistance = strickdistance;

		Debug.Log ("mPathFinder.SourceCellIndex = " + mPathFinder.SourceCellIndex);

		Debug.Log ("mPathFinder.TargetCellIndex = " + mPathFinder.TargetCellIndex);

		Debug.Log ("mPathFinder.StrickDistance = " + mPathFinder.StrickDistance);
	}

	public void UpdateNodeWeight(float value)
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			mNodeWeightListObject [index].GetComponent<TextMesh> ().text = CurrentSelectedNode.Weight.ToString ();
			//Update Edge info
			mPathFinder.UpdateNodeEdgesInfo(index, value);
		}
	}

}
