using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateAStarInFo : MonoBehaviour {
	public Text mTotalNodesText;

	public Text mTotalEdgesText;

	public Text mSearchTimeText;

	public Text mNodesSearchedText;

	public Text mEdgesSearchedText;

	public Text mDistance;

	public void UpdateToalNodesAndEdges()
	{
		mTotalNodesText.text = MapManager.MMInstance.PathFinder.TotalNodes.ToString();
		mTotalEdgesText.text = MapManager.MMInstance.PathFinder.TotalEdges.ToString();
	}

	public void UpdateSearchInfo()
	{
		mSearchTimeText.text = MapManager.MMInstance.PathFinder.TimeTaken.ToString();
		mNodesSearchedText.text = MapManager.MMInstance.PathFinder.NodesSearched.ToString();
		mEdgesSearchedText.text = MapManager.MMInstance.PathFinder.EdgesSearched.ToString();
		mDistance.text = MapManager.MMInstance.PathFinder.CostToTarget.ToString();
	}
}
