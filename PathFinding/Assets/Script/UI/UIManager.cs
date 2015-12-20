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

	}

	public void UpdateAstarInfo()
	{
		mUpdateAStarInfo.UpdateToalNodesAndEdges ();
		mUpdateAStarInfo.UpdateSearchInfo ();
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
		float weight = ++MapManager.MMInstance.CurrentSelectedNode.Weight;
		mWeightText.text = weight.ToString ();
		TimerCounter.CreateInstance ().Restart ("UpdateNodeWeightIncrease");
		MapManager.MMInstance.UpdateNodeWeight (1.0f);
		TimerCounter.CreateInstance ().End ();
	}

	public void DecreaseWeight()
	{
		float weight = --MapManager.MMInstance.CurrentSelectedNode.Weight;
		if (MapManager.MMInstance.CurrentSelectedNode.Weight <= 0) {
			MapManager.MMInstance.CurrentSelectedNode.Weight = 0;
			weight = 0.0f;
		} else {
			MapManager.MMInstance.UpdateNodeWeight (-1.0f);
		}
		mWeightText.text = weight.ToString ();
	}
}
