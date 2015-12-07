using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager UIMInstance = null;
	
	public GameObject mAStarPanel;
	
	private UpdateAStarInFo mUpdateAStarInfo;
	
	public InputField mSourceRowField;
	
	public InputField mSourceColumnField;

	public InputField mTargetRowField;

	public InputField mTargetColumnField;

	public Button mSearchButton;

	void Awake()
	{
		if (UIMInstance == null) {
			UIMInstance = this;
		} else if (UIMInstance != this) {
			Destroy (gameObject);
		}
		mUpdateAStarInfo = mAStarPanel.GetComponent<UpdateAStarInFo> ();
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

		Utility.ParserStringToInt (mSourceRowField.text, out sourcerow);
		Utility.ParserStringToInt (mSourceColumnField.text, out sourcecolumn);
		Utility.ParserStringToInt (mTargetRowField.text, out targetrow);
		Utility.ParserStringToInt (mTargetColumnField.text, out targetcolumn);

		MapManager.MMInstance.UpdateSearchInfo (sourcerow, sourcecolumn, targetrow, targetcolumn);
	
		MapManager.MMInstance.Search ();
	}
}
