using UnityEngine;
using System.Collections;

public enum GameMode
{
	E_ATTACKMODE = 0,
	E_BUILDINGMODE = 1,
	E_DELETEMODE = 2
}

public class GameManager : MonoBehaviour {

	public static GameManager mGameInstance = null;

	public GameObject mTargetSoldier;

	public GameObject mAttackingSoldier;

	public Seeker TargetSoldierSeeker {
		get {
			return mTargetSoldierSeeker;
		}
	}
	private Seeker mTargetSoldierSeeker;

	public Seeker AttackingSoldierSeeker {
		get {
			return mAttackingSoldierSeeker;
		}
	}
	private Seeker mAttackingSoldierSeeker;

	public bool mIsDebugEnable = true;

	public GameMode CurrentGameMode {
		get {
			return mCurrentGameMode;
		}
		set
		{
			mCurrentGameMode = value;
			ApplyGameMode();
		}
	}
	private GameMode mCurrentGameMode;

    private GameObject mSoldierUI;

    private GameObject mBuildingUI;

    private GameObject mSpellUI;

	void Awake()
	{
		if (mGameInstance == null) {
			mGameInstance = this;
		} else if (mGameInstance != this) {
			Destroy(gameObject);
		}

		mTargetSoldierSeeker = mTargetSoldier.GetComponent<Seeker> ();

		mAttackingSoldierSeeker = mAttackingSoldier.GetComponent<Seeker> ();

        mSoldierUI = UIManager.UIMInstance.gameObject.transform.Find("GameUICanvas/SoldierUI").gameObject;
        mBuildingUI = UIManager.UIMInstance.gameObject.transform.Find("GameUICanvas/BuildingUI").gameObject;
        mSpellUI = UIManager.UIMInstance.gameObject.transform.Find("GameUICanvas/SpellUI").gameObject;
	}

	// Use this for initialization
	void Start () {		
		mCurrentGameMode = GameMode.E_BUILDINGMODE;
		ApplyGameMode ();
	}

	private void ApplyGameMode()
	{
		switch (mCurrentGameMode) {
		case GameMode.E_BUILDINGMODE:
            mSoldierUI.SetActive(false);
            mBuildingUI.SetActive(true);
            mSpellUI.SetActive(false);
			break;
		case GameMode.E_DELETEMODE:
            mSoldierUI.SetActive(false);
            mBuildingUI.SetActive(false);
            mSpellUI.SetActive(false);
			break;
		case GameMode.E_ATTACKMODE:
            mSoldierUI.SetActive(true);
            mBuildingUI.SetActive(false);
            mSpellUI.SetActive(true);
			break;
		}

		MapManager.MMInstance.DeselectChoosingStaff ();
		UIManager.UIMInstance.mGameModeText.text = mCurrentGameMode.ToString ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateSoldierPosition()
	{
		int attackingnodeindex = 0;
		int targetnodeindex = 0;
		attackingnodeindex = GameManager.mGameInstance.AttackingSoldierSeeker.SourceCellIndex;
		targetnodeindex = GameManager.mGameInstance.AttackingSoldierSeeker.TargetCellIndex;
		mAttackingSoldier.transform.position = MapManager.MMInstance.PathFinder.NavGraph.Nodes [attackingnodeindex].Position;
		Vector3 newposition = mAttackingSoldier.transform.position;
		newposition.y += 0.2f;
		mAttackingSoldier.transform.position = newposition;

		mTargetSoldier.transform.position = MapManager.MMInstance.PathFinder.NavGraph.Nodes [targetnodeindex].Position;
		newposition = mTargetSoldier.transform.position;
		newposition.y += 0.2f;
		mTargetSoldier.transform.position = newposition;
	}

	public void MoveSoldier()
	{
		mAttackingSoldier.GetComponent<Seeker> ().Move ();
	}
}
