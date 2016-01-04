using UnityEngine;
using System.Collections;

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

	void Awake()
	{
		if (mGameInstance == null) {
			mGameInstance = this;
		} else if (mGameInstance != this) {
			Destroy(gameObject);
		}

		mTargetSoldierSeeker = mTargetSoldier.GetComponent<Seeker> ();

		mAttackingSoldierSeeker = mAttackingSoldier.GetComponent<Seeker> ();
	}

	// Use this for initialization
	void Start () {
		
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
