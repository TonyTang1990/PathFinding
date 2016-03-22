using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UIManager : MonoBehaviour {

	public static UIManager UIMInstance = null;
	
	public GameObject mAStarPanel;
	
	private UpdateAStarInFo mUpdateAStarInfo;
	
	public InputField mSourceRowField;
	
	public InputField mSourceColumnField;

	public InputField mTargetRowField;

	public InputField mTargetColumnField;

	public InputField mStrickDistanceField;

	public Button mSearchButton;
	
	public GameObject mNWAdjustPanel;

	public Text mWeightText;

	public Button mIncreaseButton;

	public Button mDecreaseButton;

	private bool mBIsNWAdjustPanelShow;

	public Text mIsWallStatusText;

	public Button mChangeWallStatus;

	public Text mIsWallJumpableStatusText;
	
	public Button mChangeWallJumpableStatus;

	public Text mGameModeText;

	void Awake()
	{
		if (UIMInstance == null) {
			UIMInstance = this;
		} else if (UIMInstance != this) {
			Destroy (gameObject);
		}
		mUpdateAStarInfo = mAStarPanel.GetComponent<UpdateAStarInFo> ();

		mNWAdjustPanel.SetActive (false);

		mBIsNWAdjustPanelShow = false;
	}

	void Start()
	{
		mGameModeText.text = GameManager.mGameInstance.CurrentGameMode.ToString ();
	}

	public void UpdateAstarInfo()
	{
		mUpdateAStarInfo.UpdateToalNodesAndEdges ();
		mUpdateAStarInfo.UpdateSearchInfo ();
	}

    public void UpdateAstarInfoForSoidler(SearchAStar.PathInfo pathinfo)
    {
        mUpdateAStarInfo.UpdateToalNodesAndEdgesForSoldier();
        mUpdateAStarInfo.UpdateSearchInfoForSoldier(pathinfo);
    }

	public void SearchClick()
	{
		int sourcerow = 0;
		int sourcecolumn = 0;
		int targetrow = 0;
		int targetcolumn = 0;
		float strickdistance = 0.0f;

		Utility.ParserStringToInt (mSourceRowField.text, out sourcerow);
		Utility.ParserStringToInt (mSourceColumnField.text, out sourcecolumn);
		Utility.ParserStringToInt (mTargetRowField.text, out targetrow);
		Utility.ParserStringToInt (mTargetColumnField.text, out targetcolumn);
		Utility.ParserStringToFloat (mStrickDistanceField.text, out strickdistance);

		MapManager.MMInstance.UpdateSearchInfo (sourcerow, sourcecolumn, targetrow, targetcolumn, strickdistance);
	
		MapManager.MMInstance.Search ();

		GameManager.mGameInstance.UpdateSoldierPosition ();

		GameManager.mGameInstance.MoveSoldier ();
	}

	
	public void ShowNWAdjustPanel()
	{
		if (!mBIsNWAdjustPanelShow) {
			mNWAdjustPanel.SetActive (true);
			mBIsNWAdjustPanelShow = true;
		}
		mWeightText.text = MapManager.MMInstance.CurrentSelectedNode.Weight.ToString();
		mIsWallStatusText.text = MapManager.MMInstance.CurrentSelectedNode.IsWall.ToString ();
		if (MapManager.MMInstance.CurrentSelectedNode.IsWall) {
			mIsWallJumpableStatusText.text = MapManager.MMInstance.CurrentSelectedNode.IsJumpable.ToString ();
		} else {
			mIsWallJumpableStatusText.text = "false";
		}
	}
	
	public void HideNWAdustPanel()
	{
		if (mBIsNWAdjustPanelShow) {
			mNWAdjustPanel.SetActive (false);
			mBIsNWAdjustPanelShow = false;
		}
	}

	public void IncreaseWeight()
	{
		float weight = MapManager.MMInstance.CurrentSelectedNode.Weight + 1;
		mWeightText.text = weight.ToString ();
		MapManager.MMInstance.UpdateNodeWeight (1.0f);
	}

	public void DecreaseWeight()
	{
		float weight = MapManager.MMInstance.CurrentSelectedNode.Weight - 1;
		if (MapManager.MMInstance.CurrentSelectedNode.Weight < 0) {
			MapManager.MMInstance.CurrentSelectedNode.Weight = 0;
			weight = 0.0f;
		} else {
			MapManager.MMInstance.UpdateNodeWeight (-1.0f);
		}
		mWeightText.text = weight.ToString ();
	}

	public void ChangeWallStatus()
	{
		bool iswall = MapManager.MMInstance.CurrentSelectedNode.IsWall;
		MapManager.MMInstance.CurrentSelectedNode.IsWall = !iswall;
		if (MapManager.MMInstance.CurrentSelectedNode.IsWall) {
			mIsWallStatusText.text = "true";
		}
		else
		{
			mIsWallStatusText.text = "false";
			mIsWallJumpableStatusText.text = "false";
			MapManager.MMInstance.UpdateNodeWallJumpableStatus (false);
		}
		MapManager.MMInstance.UpdateNodeWallStatus (MapManager.MMInstance.CurrentSelectedNode.IsWall);
	}

	public void ChangeWallJumpableStatus()
	{
		if (MapManager.MMInstance.CurrentSelectedNode.IsWall) {
			bool iswalljumpable = MapManager.MMInstance.CurrentSelectedNode.IsJumpable;
			MapManager.MMInstance.CurrentSelectedNode.IsJumpable= !iswalljumpable;
			if (MapManager.MMInstance.CurrentSelectedNode.IsJumpable) {
				mIsWallJumpableStatusText.text = "true";
			} else {
				mIsWallJumpableStatusText.text = "false";
			}
			MapManager.MMInstance.UpdateNodeWallJumpableStatus (MapManager.MMInstance.CurrentSelectedNode.IsJumpable);
		}
	}

	public void ReLoadScene()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void ClearProfile()
	{
		MapManager.MMInstance.ClearMap ();
	}

	public void EnterBuildMode()
	{
		GameManager.mGameInstance.CurrentGameMode = GameMode.E_BUILDINGMODE;
	}

	public void EnterDeleteMode()
	{
		GameManager.mGameInstance.CurrentGameMode = GameMode.E_DELETEMODE;
	}
	
	public void EnterAttackMode()
	{
		GameManager.mGameInstance.CurrentGameMode = GameMode.E_ATTACKMODE;
	}
}
