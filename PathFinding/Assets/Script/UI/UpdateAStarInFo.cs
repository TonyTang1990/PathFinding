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
		mSearchTimeText.text = GameManager.mGameInstance.AttackingSoldierSeeker.TimeTaken.ToString();
		mNodesSearchedText.text = GameManager.mGameInstance.AttackingSoldierSeeker.NodesSearched.ToString();
		mEdgesSearchedText.text = GameManager.mGameInstance.AttackingSoldierSeeker.EdgesSearched.ToString();
		mDistance.text = GameManager.mGameInstance.AttackingSoldierSeeker.CostToTarget.ToString();
	}
}
