using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager mGameInstance = null;

	public GameObject mTargetSoldier;

	public GameObject mAttackingSoldier;

	void Awake()
	{
		if (mGameInstance == null) {
			mGameInstance = this;
		} else if (mGameInstance != this) {
			Destroy(gameObject);
		}
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
		attackingnodeindex = MapManager.MMInstance.PathFinder.SourceCellIndex;
		targetnodeindex = MapManager.MMInstance.PathFinder.TargetCellIndex;
		mAttackingSoldier.transform.position = MapManager.MMInstance.PathFinder.NavGraph.Nodes [attackingnodeindex].Position;
		mTargetSoldier.transform.position = MapManager.MMInstance.PathFinder.NavGraph.Nodes [targetnodeindex].Position;
	}

	public void MoveSoldier()
	{
		mAttackingSoldier.GetComponent<Seeker> ().Move (MapManager.MMInstance.PathFinder.MovementPath);
	}
}
