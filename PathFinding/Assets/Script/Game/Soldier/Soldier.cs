using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

[Serializable]
public enum SoldierType
{
	E_HELLEPHANT = 0,
	E_ZOMBUNNY = 1,
	E_DEFAULT
}

/*
[Serializable]
public enum SoldierState
{
	E_IDLE = 0,
	E_MOVING = 1,
	E_ATTACKING = 2,
	E_DEAD
}
*/
[Serializable]
public enum PreferAttackType
{
	E_NULLATTACABLE = 0,
	E_ATTACKABLE = 1,
	E_ALL
}

[Serializable]
public class Soldier : MonoBehaviour, GameObjectType {

	//public SoldierType mST;
	
	//public SoldierState mSS;

	public float mSpeed;

	public PreferAttackType mPreferAttackType;

	public float mAttackInterval;
	
	public float mAttackDistance;

	public float mSHP;
	
	public GameObject mBullet;

	public float DistanceToTarget {
		get {
			return mDistanceToTarget;
		}
		set {
			mDistanceToTarget = value;
		}
	}
	private float mDistanceToTarget = 0.0f;

	public float AttackTimer {
		get {
			return mAttackTimer;
		}
		set {
			mAttackTimer = value;
		}
	}
	protected float mAttackTimer = 0.0f;

	public Building AttackTarget {
		get {
			return mAttackingObject;
		}
		set {
			mAttackingObject = value;
		}
	}
	protected Building mAttackingObject;

	public Building OldAttackObject {
		get {
			return mOldAttackingObject;
		}
	}
	protected Building mOldAttackingObject;

	public Hashtable CurrentCalculatingPaths {
		get {
			return mCurrentCalculatingPaths;
		}
		set {
			mCurrentCalculatingPaths = value;
		}
	}
	private Hashtable mCurrentCalculatingPaths = null;

	public Building ShortestPathTarget
	{
		get
		{
			return mShortestPathObject;
		}
		set
		{
			mShortestPathObject = value;
		}
	}
	private Building mShortestPathObject;

	private Building mOriginalPathObject;

	public Vector3[] AttackablePostions
	{
		get
		{
			return mAttackablePositions;
		}
	}
	private Vector3[] mAttackablePositions; 

	public Animator Anim {
		get {
			return mAnim;
		}
	}
	protected Animator mAnim;

	[HideInInspector] public SoldierState mSCurrentState;

	[HideInInspector] public SoldierAttackState mSAttackState;

	[HideInInspector] public SoldierDeadState mSDeadState;

	[HideInInspector] public SoldierMoveState mSMoveState;

	public bool IsDead{
		get
		{
			return mIsDead;
		}
		set
		{
			mIsDead = value;
		}
	}
	protected bool mIsDead = false;

	private TextMesh mHPText;

	public Seeker Seeker {
		get {
			return mSeeker;
		}
	}
	protected Seeker mSeeker;

	public List<Vector3> AStarPath {
		get {
			return mAStarPath;
		}
	}
	protected List<Vector3> mAStarPath;

	public List<int> AStarPathIndexList {
		get {
			return mAStarPathIndexList;
		}
	}
	private List<int> mAStarPathIndexList;

	public float ShortestTargetPathLength {
		get {
			return mShortestTargetPathLength;
		}
		set
		{
			mShortestTargetPathLength = value;
		}
	}
	private float mShortestTargetPathLength;

	public float NearestTargetPathLength {
		get {
			return mNearestTargetPathLength;
		}
		set
		{
			mNearestTargetPathLength = value;
		}
	}
	private float mNearestTargetPathLength = Mathf.Infinity;

	private Vector3 mFinalMovePosition;
/*
	public Path ShortestPath {
		get {
			return mShortestPath;
		}
	}
	protected Path mShortestPath;
*/
	//private Path[] mLastPaths;

	//A Star Search Info
	public SearchAStar ShortestPath {
		get {
			return mShortestPath;
		}
	}
	private SearchAStar mShortestPath;

	private List<Pair<int, SearchAStar>>/*SearchAStar[]*/ mPathsInfo;

	/* Number of paths completed so far */
	//private int mNumCompleted = 0;

	public int CurrentWayPoint {
		get {
			return mCurrentWayPoint;
		}
	}
	protected int mCurrentWayPoint = 0;

	public float mNextWaypointDistance = 0.2f;

	//private CharacterController mController;

	public float NearestTargetDistance {
		get {
			return mNearestTargetDistance;
		}
		set {
			mNearestTargetDistance = value;
		}
	}
	private float mNearestTargetDistance = 0.0f;

	public float NearestReachableTargetDIstance
	{
		get
		{
			return mNearestReachableTargetDistance;
		}
		set
		{
			mNearestReachableTargetDistance = value;
		}
	}
	private float mNearestReachableTargetDistance = 0.0f;

	public ObjectType GameType
	{
		get
		{
			return mGameType;
		}
		set
		{
			mGameType = value;
		}
	}
	private ObjectType mGameType;

	private GameObject mDetectionRangeCollider;
	
	private SoldierDetectRange mDetectionRange;

	public float mDetectionDistance;

	private bool mBMakeNewDecision = false;

	public virtual void Awake()
	{
		Utility.Log("Soldier's Position = " + gameObject.transform.position);
		/*
		if (mST == SoldierType.E_ZOMBUNNY) {
			mAnim = GetComponent<Animator>();
		}
		*/

		mHPText = gameObject.transform.Find ("HealthText").gameObject.GetComponent<TextMesh> ();
		if (mHPText == null) {
			Utility.Log("mHPText == null");
		}
		mHPText.text = "HP: " + mSHP;

		mAnim = GetComponent<Animator>();

		mSeeker = GetComponent<Seeker> ();

		//mController = GetComponent<CharacterController> ();

		mSAttackState = new SoldierAttackState (this);

		mSMoveState = new SoldierMoveState (this);

		mSDeadState = new SoldierDeadState (this);

		mAttackablePositions = new Vector3[8];
	
		mGameType = ObjectType.EOT_SOLDIER;
		Utility.Log ("Soldier::Awake() mGameType = " + mGameType);

		mDetectionRangeCollider = gameObject.transform.Find ("AttackRangeCollider").gameObject;
		mDetectionRangeCollider.GetComponent<SphereCollider> ().radius = mDetectionDistance;
		Utility.Log ("DetectionDistance = " + mDetectionDistance);
		mDetectionRange = mDetectionRangeCollider.GetComponent<SoldierDetectRange> ();

		mAStarPath = null;

		mAStarPathIndexList = null;

		mFinalMovePosition = new Vector3();

		mShortestPathObject = null;

		mOriginalPathObject = null;
	}

	public void Start()
	{
		mSCurrentState = mSMoveState;

		StartCoroutine (RemoveDestroyedObjectInRange());
	}

	public virtual void Update ()
	{
		if (gameObject) {
			mHPText.text = "HP: " + mSHP;
			mSCurrentState.UpdateState();
		}
	}

	public void TakeDamage (float damage)
	{
		if (mSHP > damage) {
			mSHP -= damage;
		} else {
			mSHP = 0;
			mIsDead = true;
		}
	}

	//AniamtionEvent call
	public void StartSinking ()
	{
		Invoke ("DestroyItself",3.0f);
	}

	public virtual void DestroyItself()
	{
		Destroy (gameObject);
	}

	private bool ShouldChangeAttackTarget()
	{
		if(mBMakeNewDecision)
		{
			mBMakeNewDecision = false;
			return !mBMakeNewDecision;
		}
		else
		{
			if(mOriginalPathObject != null && mOriginalPathObject.mBI.IsDestroyed)
			{
				return true;
			}
			else
			{
				if (mAttackingObject != null)
				{
					if (!mAttackingObject.mBI.IsDestroyed) {
						return false;
					}
					else
					{
						return true;
					}
				} else {
					return true;
				}
			}
		}
	}

	private Building ObtainAttackObjectInDetectionRange()
	{
		return FindShortestPathObject(mDetectionRange.RangeTargetList);
	}

	public void MakeDecision()
	{
		if (gameObject != null) {
			mOldAttackingObject = mAttackingObject;
			if(ShouldChangeAttackTarget())
			{
				//mAttackingObject = null;
				Utility.Log ("mDetectionRange.RangeTargetList.Count = " + mDetectionRange.RangeTargetList.Count);
				mAttackingObject = ObtainAttackObjectInDetectionRange();
				//CalculateAllPathsInfo(MapManager.mMapInstance.NullWallBuildingsInfoInGame);
				//Otherwise chose one as attack target in whole map
				if(mAttackingObject == null)
				{
					Utility.Log("Chose Target From Whole Map");
					mAttackingObject= MapManager.MMInstance.ObtainAttackObject (this);
				}

				if(mAttackingObject != mOldAttackingObject && mAttackingObject!=null)
				{
					//Change the first point to gameobject location to avoid soldier move away from target
					mAStarPath = mShortestPath.MovementPathToTarget;

					mAStarPathIndexList = mShortestPath.PathToTarget;

					mCurrentWayPoint = mAStarPath.Count - 1;

					mAStarPath[mCurrentWayPoint] = transform.position;

					//if the attacking object is Wall,
					//we let soldier listenning for wall break event to make new decision once has wall breaked
					if(mAttackingObject.mBI.getBuildingType() == BuildingType.E_WALL)
					{
						EventManager.mEMInstance.StartListening("WALL_BREAK",WallBreakDelegate);
					}
					else
					{
						EventManager.mEMInstance.StopListening("WALL_BREAK",WallBreakDelegate);
					}
				}
			}
		}
	}

	private IEnumerator RemoveDestroyedObjectInRange()
	{
		//if there are any object has been destroyed in detect range, we let soldier made decision again
		//This takes O(n) times, so we do not do this frequently
		while (true) {
			if(gameObject != null && !IsDead)
			{
				Building temp = null;
				List<int> indextoremove = new List<int> ();
			
				foreach (DictionaryEntry entry in mDetectionRange.RangeTargetList) {
					temp = entry.Value as Building;
					if (temp.mBI.IsDestroyed) {
						indextoremove.Add (temp.mBI.mIndex);
					}
				}
			
				foreach (int inedx in indextoremove) {
					mDetectionRange.RangeTargetList.Remove (inedx);
				}	
				yield return new WaitForSeconds(3.0f);
			}
			else
			{
				break;
			}
		}
	}

	private void WallBreakDelegate()
    {
		Utility.Log ("WallBreakDelegate() called");
		mBMakeNewDecision = true;
	}

	public Building FindShortestPathObject(/*List<Building>*/ Hashtable calculatingpaths)
	{
		mCurrentCalculatingPaths = calculatingpaths;
		Utility.Log ("mCurrentCalculatingPaths.Count = " + mCurrentCalculatingPaths.Count);

		if (mCurrentCalculatingPaths.Count == 0) {
			return null;
		}

		//Reset mpathsInfo to null before we caculate for all paths again
		mPathsInfo = null;
	
		//Create a new lastPaths array if necessary (can reuse the old one?)
		int validbuildingnumbers = 0; //= MapManager.mMapInstance.NullWallBuildingNumber;
		//int nullwallbuildingnumbers = mCurrentCalculatingPaths.Count;
		foreach (DictionaryEntry entry in mCurrentCalculatingPaths) {
			Building nwbd = entry.Value as Building;
			if(nwbd.mBI.IsDestroyed != true && nwbd.mBI.getBuildingType() != BuildingType.E_WALL)
			{
				validbuildingnumbers++;
			}
		}

		if (validbuildingnumbers == 0) {
			return null;
		}

		if (mPathsInfo == null) {
			mPathsInfo = new List<Pair<int, SearchAStar>>(validbuildingnumbers);/*new SearchAStar[validbuildingnumbers];*/
		}

		Building tempbd = null;
		int pathindex = 0;
		Vector2 soldierindex;
		Vector2 bdindex;
		//for(int i = 0; i < nullwallbuildingnumbers; i++)
		//{
		foreach (DictionaryEntry entry in mCurrentCalculatingPaths) {
			Building bd = entry.Value as Building;
			if( bd.mBI.mBT == BuildingType.E_WALL)
			{
				//Debug.Log("IsDestroyed = " + bd.mBI.IsDestroyed);
				continue;
			}
			else
			{
				Utility.Log ("FindShortestPathObject() called");
				tempbd = bd;
				if(tempbd.mBI.IsDestroyed!=true)
				{
					Utility.Log ("transform.position = " + transform.position);
					Utility.Log ("bd.transform.position = " + tempbd.transform.position);
					//If soldier has movement path, we use it last movememnt node as his position
					//to avoid inaccuracy position conversion
					if(mAStarPath != null)
					{
						soldierindex = Utility.ConvertFloatPositionToRC(mFinalMovePosition);
					}
					else
					{
						soldierindex = Utility.ConvertFloatPositionToRC(transform.position);
					}
					int index = Utility.ConvertRCToIndex((int)(soldierindex.x),(int)(soldierindex.y));
					if(MapManager.MMInstance.NodeTerrainList[index].IsWall)
					{
						Debug.Log("index = " + index);
						Debug.Log("Soldier stays at Wall Position");
					}
					//soldierindex = 
					bdindex = Utility.ConvertIndexToRC(tempbd.mBI.mIndex); /*Utility.ConvertFloatPositionToRC(tempbd.transform.position)*/;
					Utility.Log ("soldierindex = " + soldierindex);
					Utility.Log ("bdindex = " + bdindex);

					mSeeker.UpdateSearchInfo((int)(soldierindex.x),(int)(soldierindex.y),(int)(bdindex.x),(int)(bdindex.y),mAttackDistance);
					mSeeker.CreatePathAStar();
					SearchAStar path = mSeeker.mAstarSearch;
					mPathsInfo.Add(new Pair<int, SearchAStar> (tempbd.mBI.mIndex,path));
					//ABPath p = ABPath.Construct (transform.position, bd.transform.position, OnPathInfoComplete);
					//mLastPaths[pathindex] = p;
					//AstarPath.StartPath (p);
				}
				pathindex++;
			}
		}

		mShortestPathObject = FindShortestPathTarget ();


		mCurrentCalculatingPaths = null;

		return mShortestPathObject;
		//}
	}

	private Building FindShortestPathTarget()
	{
		mShortestPath = null;

		int index = -1;
		float distance = Mathf.Infinity;
		foreach (Pair<int, SearchAStar> pair in mPathsInfo) {
			if(pair.Value.GetCostToTarget() < distance)
			{
				index = pair.Key;
				distance = pair.Value.GetCostToTarget();
				mShortestPath = pair.Value;
			}
		}

		if (mShortestPath != null && mShortestPath.IsWallInPathToTarget) {
			Utility.Log("mShortestPath.mWallInPathToTargetIndex = " + mShortestPath.WallInPathToTargetIndex);
			//int wallindex = -1;
			//wallindex = MapManager.MMInstance.BuildingsInfoInGame.FindIndex(x => x.mBI.mIndex == mShortestPath.WallInPathToTargetIndex);
			//wallindex = mCurrentCalculatingPaths.FindIndex(x => x.mBI.mIndex == mShortestPath.WallInPathToTargetIndex);
			//Debug.Log("wallindex = " + wallindex);
			//Assert.IsTrue(wallindex != -1);
			//mShortestPathObject = mCurrentCalculatingPaths[wallindex];
			//mShortestPathObject = MapManager.MMInstance.BuildingsInfoInGame[wallindex];
			mShortestPathObject = MapManager.MMInstance.BuildingsInfoInGame[mShortestPath.WallInPathToTargetIndex] as Building;
			mOriginalPathObject = MapManager.MMInstance.BuildingsInfoInGame[mShortestPath.OriginalTarget] as Building;
		}
		else
		{
			mShortestPathObject = mCurrentCalculatingPaths[index] as Building;
			mOriginalPathObject = MapManager.MMInstance.BuildingsInfoInGame[mShortestPath.OriginalTarget] as Building;
		}

		return mShortestPathObject;
	}
	
	public bool IsTargetInAttackRange()
	{
		if (mAttackingObject != null) 
		{
			mDistanceToTarget = Vector3.Distance (transform.position, MapManager.MMInstance.NodeTerrainList[mAttackingObject.mBI.mIndex].Position /*mAttackingObject.mBI.Position*/);
			if (mDistanceToTarget > mAttackDistance) 
			{
				return false;
			}
			else
			{
				return true;
			}
		} 
		else
		{
			return false;
		}
	}

	public void Attack()
	{
		mAttackTimer += Time.deltaTime;
		if (mAttackTimer >= mAttackInterval) {
			mAttackTimer = 0.0f;
			GameObject bl = MonoBehaviour.Instantiate (mBullet, transform.position, Quaternion.identity) as GameObject;
			bl.GetComponent<Bullet> ().AttackTarget = mAttackingObject;
		}
	}

	public void Move()
	{
		if (mAttackingObject != null) 
		{
			if (mAStarPath == null) {
				//We have no path to move after yet
				return;
			}


			Vector3 dir = new Vector3();
			Vector3 newposition = new Vector3();

			//Never reach the final point(to avoid soldier reach wall position), so give soldier at least 1 strick distance
			if (mCurrentWayPoint < 0) {
				//Move forward to target object to make sure it under soldier attack range
				//Direction to the next waypoint
				dir = (MapManager.MMInstance.NodeTerrainList[mAttackingObject.mBI.mIndex].Position - transform.position).normalized;
				dir.y = 0.0f;
				
				transform.LookAt (MapManager.MMInstance.NodeTerrainList[mAttackingObject.mBI.mIndex].Position);

				//Here we set speed to 0.5f to avoid soldier move too far from destination node
				newposition = transform.position + dir * 0.2f * Time.deltaTime;
				transform.position = newposition;

				Utility.Log ("End Of Path Reached");

				if(MapManager.MMInstance.NodeTerrainList[mAStarPathIndexList[0]].IsWall)
				{
					Debug.Log("mCurrentWayPoint < 0 MapManager.MMInstance.NodeTerrainList[mAStarPathIndexList[mCurrentWayPoint]].IsWall");
					mFinalMovePosition = mFinalMovePosition;
				}
				else
				{
					mFinalMovePosition = mAStarPath[0];
				}

				return;
			}
			else
			{
				if(mAStarPath.Count <= 1)
				{
					Debug.Log("mAStarPath.Count <= 1");
				}
				//Direction to the next waypoint
				dir = (mAStarPath[mCurrentWayPoint] - transform.position).normalized;
				dir.y = 0.0f;
				
				transform.LookAt (mAStarPath [mCurrentWayPoint]);
				
				newposition = transform.position + dir * mSpeed * Time.deltaTime;
				transform.position = newposition;

				//Check if we are close enough to the next waypoint
				if (Vector3.Distance (transform.position, mAStarPath[mCurrentWayPoint]) < mNextWaypointDistance) {
					
					Vector2 soldierindex = Utility.ConvertFloatPositionToRC( mAStarPath[mCurrentWayPoint]);

					int index = Utility.ConvertRCToIndex((int)(soldierindex.x),(int)(soldierindex.y));

					if(MapManager.MMInstance.NodeTerrainList[index].IsWall)
					{
						Debug.Log("soldierindex.x = " + soldierindex.x);
						Debug.Log("soldierindex.y = " + soldierindex.y);

						Debug.Log("index = " + index);
						Debug.Log("mAStarPathIndexList[mCurrentWayPoint] = " + mAStarPathIndexList[mCurrentWayPoint]);
						Debug.Log("mAStarPathIndexList[mCurrentWayPoint-1] = " + mAStarPathIndexList[mCurrentWayPoint - 1]);

						Debug.Log("mCurrentWayPoint !< 0 MapManager.MMInstance.NodeTerrainList[mAStarPathIndexList[mCurrentWayPoint]].IsWall");
						mFinalMovePosition = mFinalMovePosition;
					}
					else
					{
						mFinalMovePosition = mAStarPath[mCurrentWayPoint];
					}
					mCurrentWayPoint--;
					return;
				}
			}
		}
		/*
		if (mAttackingObject != null) {
			if (mAStarPath == null) {
				//We have no path to move after yet
				return;
			}
			if (mCurrentWayPoint >= mAStarPath.vectorPath.Count) {
				Debug.Log ("End Of Path Reached");
				return;
			}
			//Direction to the next waypoint
			Vector3 dir = (mAStarPath.vectorPath [mCurrentWayPoint] - transform.position).normalized;

			transform.LookAt (mAStarPath.vectorPath [mCurrentWayPoint]);
		*/
			/*
		if (mController != null) {
			mController.SimpleMove (dir);
		}
		*/
		/*
			Vector3 newposition = transform.position + dir * mSpeed * Time.deltaTime;
			transform.position = newposition;

			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance (transform.position, mAStarPath.vectorPath [mCurrentWayPoint]) < mNextWaypointDistance) {
				mCurrentWayPoint++;
				return;
			}
		}
		*/
	}
	
	public void CalculatePath()
	{
		if(mAttackingObject != null)
		{
			Utility.Log ("CalculatePath() called");
			//mAStarPath = mSeeker();
			//StartCoroutine(WaitForPathCalculation());
		}
	}

	/*
	private IEnumerator WaitForPathCalculation()
	{
		Debug.Log ("mAttackingObject.transform.position = " + mAttackingObject.transform.position);
		mAStarPath = mSeeker.StartPath(transform.position, mAttackingObject.transform.position, OnPathComplete);
		yield return StartCoroutine (mAStarPath.WaitForPath ());
		mNearestTargetPathLength = mAStarPath.GetTotalLength ();
		Debug.Log ("mNearestTargetPathLength = " + mNearestTargetPathLength);
		mAStarPath.Claim (this);
	}
	*/
/*	
	private void OnPathComplete(Path path)
	{
		if (!path.error) {
			//mAStarPath = path;
			//if(mAStarPath==null)
			//{
				//Debug.Log ("mAStarPath == null");
			//}
			mCurrentWayPoint = 0;
			float targetpositiondistance = Vector3.Distance (mAttackingObject.transform.position, path.vectorPath [path.vectorPath.Count - 1]);
			Debug.Log ("targetpositiondistance = " + targetpositiondistance);
		} else {
			Debug.Log ("Oh noes, the target was not reachable: "+path.errorLog);
			mAStarPath = null;
		}
	}
*/
	//private void OnPathInfoComplete(Path p)
	//{
		//if (!p.error) {
			/*float temppathlength = p.GetTotalLength ();
			if (temppathlength < mShortestTargetPathLength) {
				mShortestTargetPathLength = temppathlength;
				//mShortestPathObject = bd;
				Debug.Log ("mShortestTargetPathLength = " + mShortestTargetPathLength);
			}
			mCurrentWayPoint = 0;
			*/
			//Make sure this path is not an old one
			//for (int i=0;i<mLastPaths.Length;i++) {
				//if (mLastPaths[i] == p) {
					//mNumCompleted++;
					
					//if (mNumCompleted >= mLastPaths.Length) {
						//CompleteSearchClosest ();
					//}
					//return;
				//}
			//}
		//} else {
			//Debug.Log ("Oh noes, the target was not reachable: "+p.errorLog);
			//mShortestPath = null;
		//}
	//}

	/* Called when all paths have completed calculation */
	public void CompleteSearchClosest () {
		
		//Find the shortest path
		/*
		Path shortest = null;
		float shortestLength = float.PositiveInfinity;
		
		//Loop through the paths
		for (int i=0;i<mLastPaths.Length;i++) {
			//Get the total length of the path, will return infinity if the path had an error
			float length = mLastPaths[i].GetTotalLength();
			
			Debug.Log ("length = "+ length);

			if (shortest == null || length < mShortestTargetPathLength) {
				shortest = mLastPaths[i];
				mShortestTargetPathLength = length;
				mShortestPathObject = mCurrentCalculatingPaths[i];				
			}
		}
		Debug.Log ("mShortestTargetPathLength = "+ mShortestTargetPathLength);
		mShortestPath = shortest;
		*/
	}
}

