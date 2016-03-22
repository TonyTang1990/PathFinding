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
		mNodesSearchedText.text = GameManager.mGameInstance.AttackingSoldierSeeker.SeekerPathInfo.NodesSearched.ToString();
        mEdgesSearchedText.text = GameManager.mGameInstance.AttackingSoldierSeeker.SeekerPathInfo.EdgesSearched.ToString();
        mDistance.text = GameManager.mGameInstance.AttackingSoldierSeeker.SeekerPathInfo.CostToTarget.ToString();
	}

    public void UpdateToalNodesAndEdgesForSoldier()
    {
        mTotalNodesText.text = MapManager.MMInstance.PathFinder.TotalNodes.ToString();
        mTotalEdgesText.text = MapManager.MMInstance.PathFinder.TotalEdges.ToString();
    }

    public void UpdateSearchInfoForSoldier(SearchAStar.PathInfo pathinfo)
    {
        mSearchTimeText.text = pathinfo.TimeTaken.ToString();
        mNodesSearchedText.text = pathinfo.NodesSearched.ToString();
        mEdgesSearchedText.text = pathinfo.EdgesSearched.ToString();
        mDistance.text = pathinfo.CostToTarget.ToString();
    }
}
