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
public class Soldier : MonoBehaviour, GameObjectType
{

    //public SoldierType mST;

    //public SoldierState mSS;

    public float mSpeed;

    public PreferAttackType mPreferAttackType;

    public float mAttackInterval;

    public float mAttackDistance;

    public float mSHP;

    public bool mUpdateHP = false;

    public GameObject mBullet;

    protected Vector3 mBulletSpawnPoint;

    public float DistanceToTarget
    {
        get
        {
            return mDistanceToTarget;
        }
        set
        {
            mDistanceToTarget = value;
        }
    }
    private float mDistanceToTarget = 0.0f;

    public float AttackTimer
    {
        get
        {
            return mAttackTimer;
        }
        set
        {
            mAttackTimer = value;
        }
    }
    protected float mAttackTimer = 0.0f;

    public Building AttackTarget
    {
        get
        {
            return mAttackingObject;
        }
        set
        {
            mAttackingObject = value;
        }
    }
    protected Building mAttackingObject;

    public Building OldAttackObject
    {
        get
        {
            return mOldAttackingObject;
        }
    }
    protected Building mOldAttackingObject;

    public Hashtable CurrentCalculatingPaths
    {
        get
        {
            return mCurrentCalculatingPaths;
        }
        set
        {
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

    public Animator Anim
    {
        get
        {
            return mAnim;
        }
    }
    protected Animator mAnim;

    public SoldierState SCurrentState
    {
        set
        {
            if (mSCurrentState != null)
            {
                mSCurrentState.ExitState();
            }
            mSCurrentState = value;
            mSCurrentState.EnterState();
        }
    }
    [HideInInspector]
    private SoldierState mSCurrentState;

    [HideInInspector]
    public SoldierAttackState mSAttackState;

    [HideInInspector]
    public SoldierDeadState mSDeadState;

    [HideInInspector]
    public SoldierMoveState mSMoveState;

    public bool IsDead
    {
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

    public Seeker Seeker
    {
        get
        {
            return mSeeker;
        }
    }
    protected Seeker mSeeker;

    public List<Vector3> AStarPath
    {
        get
        {
            return mAStarPath;
        }
    }
    protected List<Vector3> mAStarPath;

    public List<int> AStarPathIndexList
    {
        get
        {
            return mAStarPathIndexList;
        }
    }
    private List<int> mAStarPathIndexList;

    public float ShortestTargetPathLength
    {
        get
        {
            return mShortestTargetPathLength;
        }
        set
        {
            mShortestTargetPathLength = value;
        }
    }
    private float mShortestTargetPathLength;

    public float NearestTargetPathLength
    {
        get
        {
            return mNearestTargetPathLength;
        }
        set
        {
            mNearestTargetPathLength = value;
        }
    }
    private float mNearestTargetPathLength = Mathf.Infinity;

    private Vector3 mFinalMovePosition;

    //A Star Search Info
    /*
	public SearchAStar ShortestPath {
		get {
			return mShortestPath;
		}
	}
	private SearchAStar mShortestPath;
    */

    public SearchAStar.PathInfo ShortestPath
    {
        get
        {
            return mShortestPath;
        }
    }
    private SearchAStar.PathInfo mShortestPath;

    private List<Pair<int, SearchAStar.PathInfo>> mPathsInfo;

    public int CurrentWayPoint
    {
        get
        {
            return mCurrentWayPoint;
        }
    }
    protected int mCurrentWayPoint = 0;

    public float mNextWaypointDistance = 0.2f;

    public float NearestTargetDistance
    {
        get
        {
            return mNearestTargetDistance;
        }
        set
        {
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

    private Vector3 mDir;

    private Vector3 mNewposition;

    public virtual void Awake()
    {
        Utility.Log("Soldier's Position = " + gameObject.transform.position);

        mHPText = gameObject.transform.Find("HealthText").gameObject.GetComponent<TextMesh>();
        if (mHPText == null)
        {
            Utility.Log("mHPText == null");
        }
        mHPText.text = "HP: " + mSHP;

        mAnim = GetComponent<Animator>();

        mSeeker = GetComponent<Seeker>();

        mSAttackState = new SoldierAttackState(this);

        mSMoveState = new SoldierMoveState(this);

        mSDeadState = new SoldierDeadState(this);

        mAttackablePositions = new Vector3[8];

        mGameType = ObjectType.EOT_SOLDIER;
        Utility.Log("Soldier::Awake() mGameType = " + mGameType);

        mDetectionRangeCollider = gameObject.transform.Find("AttackRangeCollider").gameObject;
        mDetectionRangeCollider.GetComponent<SphereCollider>().radius = mDetectionDistance;
        Utility.Log("DetectionDistance = " + mDetectionDistance);
        mDetectionRange = mDetectionRangeCollider.GetComponent<SoldierDetectRange>();

        mAStarPath = null;

        mAStarPathIndexList = null;

        mFinalMovePosition = new Vector3();

        mShortestPathObject = null;

        mPathsInfo = new List<Pair<int, SearchAStar.PathInfo>>(5);

        mOriginalPathObject = null;

        mDir = new Vector3();
        
        mNewposition = new Vector3();
    }

    public void Start()
    {
        mSCurrentState = mSMoveState;

        StartCoroutine(RemoveDestroyedObjectInRange());
    }

    public virtual void Update()
    {
        if (gameObject)
        {
            mSCurrentState.UpdateState();
        }
    }

    public virtual void FixedUpdate()
    {
        if (gameObject && mUpdateHP)
        {
            mHPText.text = "HP: " + mSHP;
            mUpdateHP = false;
        }
    }

    public virtual void UpdateChildPosition()
    {
        mBulletSpawnPoint = gameObject.transform.Find("BulletSpawnPoint").gameObject.transform.position;
    }

    public void TakeDamage(float damage)
    {
        if (mSHP > damage)
        {
            mSHP -= damage;
        }
        else
        {
            mSHP = 0;
            mIsDead = true;
            //Remove attacker list for building before Soldier die
            if (mAttackingObject != null)
            {
                mAttackingObject.RemoveAttacker(this);
            }
        }
        mUpdateHP = true;
    }

    //AniamtionEvent call
    public void StartSinking()
    {
        Invoke("DestroyItself", 3.0f);
    }

    public virtual void DestroyItself()
    {
        Destroy(gameObject);
    }

    private bool ShouldChangeAttackTarget()
    {
        if (IsLocatedAtWallPosition())
        {
            return false;
        }

        if (mBMakeNewDecision)
        {
            mBMakeNewDecision = false;
            return !mBMakeNewDecision;
        }
        else
        {
            if (mOriginalPathObject != null && mOriginalPathObject.mBI.IsDestroyed)
            {
                return true;
            }
            else
            {
                if (mAttackingObject != null)
                {
                    if (!mAttackingObject.mBI.IsDestroyed)
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
                    return true;
                }
            }
        }
    }

    private bool IsLocatedAtWallPosition()
    {
        Vector2 soldierindex = Utility.ConvertFloatPositionToRC(mFinalMovePosition);

        int index = Utility.ConvertRCToIndex((int)(soldierindex.x), (int)(soldierindex.y));

        if (MapManager.MMInstance.NodeTerrainList[index].IsWall)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Building ObtainAttackObjectInDetectionRange()
    {
        return FindShortestPathObject(mDetectionRange.RangeTargetList);
    }

    public void MakeDecision()
    {
        if (gameObject != null)
        {
            mOldAttackingObject = mAttackingObject;
            if (ShouldChangeAttackTarget())
            {
                if (mOldAttackingObject != null)
                {
                    mOldAttackingObject.RemoveAttacker(this);
                }
                Utility.Log("mDetectionRange.RangeTargetList.Count = " + mDetectionRange.RangeTargetList.Count);
                mAttackingObject = ObtainAttackObjectInDetectionRange();

                //Otherwise chose one as attack target in whole map
                if (mAttackingObject == null)
                {
                    Utility.Log("Chose Target From Whole Map");
                    mAttackingObject = MapManager.MMInstance.ObtainAttackObject(this);
                }

                if (mAttackingObject != mOldAttackingObject && mAttackingObject != null)
                {
                    //Change the first point to gameobject location to avoid soldier move away from target
                    mAStarPath = mShortestPath.MovementPathToTarget;

                    mAStarPathIndexList = mShortestPath.PathToTarget;

                    mCurrentWayPoint = mAStarPath.Count - 1;

                    mAStarPath[mCurrentWayPoint] = transform.position;

                    //Append soldier to attackerlist for building
                    mAttackingObject.AppendAttacker(this);

                    //if the attacking object is Wall,
                    //we let soldier listenning for wall break event to make new decision once has wall breaked
                    /*
                     * Inform all soldiers that is listenning to WALL_BREAK will cause fps drop
                     * Only inform soldiers that are in valid wall notification range
                    if(mAttackingObject.mBI.getBuildingType() == BuildingType.E_WALL)
                    {
                        EventManager.mEMInstance.StartListening("WALL_BREAK",WallBreakDelegate);
                    }
                    else
                    {
                        EventManager.mEMInstance.StopListening("WALL_BREAK",WallBreakDelegate);
                    }
                    */
                }
            }
        }
    }

    private IEnumerator RemoveDestroyedObjectInRange()
    {
        //if there are any object has been destroyed in detect range, we let soldier made decision again
        //This takes O(n) times, so we do not do this frequently
        while (true)
        {
            if (gameObject != null && !IsDead)
            {
                Building temp = null;
                List<int> indextoremove = new List<int>();
                IDictionaryEnumerator enu = mDetectionRange.RangeTargetList.GetEnumerator();
                DictionaryEntry entry;

                while (enu.MoveNext())
                {
                    entry = (DictionaryEntry)enu.Current;
                    temp = entry.Value as Building;
                    if (temp.mBI.IsDestroyed)
                    {
                        indextoremove.Add(temp.mBI.mIndex);
                    }
                }

                for (int index = 0; index < indextoremove.Count; index++)
                {
                    mDetectionRange.RangeTargetList.Remove(index);
                }
                yield return new WaitForSeconds(30.0f);
            }
            else
            {
                break;
            }
        }
    }

    public void WallBreakDelegate()
    {
        Utility.Log("WallBreakDelegate() called");
        mBMakeNewDecision = true;
    }

    public void JumpSpellDelegate()
    {
        Utility.Log("JumpSpellDelegate() called");
        mBMakeNewDecision = true;
    }

    public Building FindShortestPathObject(/*List<Building>*/ Hashtable calculatingpaths)
    {
        mCurrentCalculatingPaths = calculatingpaths;
        Utility.Log("mCurrentCalculatingPaths.Count = " + mCurrentCalculatingPaths.Count);

        if (mCurrentCalculatingPaths.Count == 0)
        {
            return null;
        }

        //Create a new lastPaths array if necessary (can reuse the old one?)
        int validbuildingnumbers = 0;
        IDictionaryEnumerator enu = mCurrentCalculatingPaths.GetEnumerator();
        DictionaryEntry entry;
        Building nwbd;
        while (enu.MoveNext())
        {
            entry = (DictionaryEntry)enu.Current;
            nwbd = entry.Value as Building;
            if (nwbd.mBI.IsDestroyed != true && nwbd.mBI.getBuildingType() != BuildingType.E_WALL)
            {
                validbuildingnumbers++;
            }
        }

        if (validbuildingnumbers == 0)
        {
            return null;
        }

        if (mPathsInfo != null)
        {
            mPathsInfo.Clear();
        }

        Building tempbd = null;
        int pathindex = 0;
        Vector2 soldierindex = new Vector2();
        Vector2 bdindex = new Vector2();

        enu = mCurrentCalculatingPaths.GetEnumerator();
        while (enu.MoveNext())
        {
            entry = (DictionaryEntry)enu.Current;
            Building bd = entry.Value as Building;
            if (bd.mBI.mBT == BuildingType.E_WALL)
            {
                //Debug.Log("IsDestroyed = " + bd.mBI.IsDestroyed);
                continue;
            }
            else
            {
                Utility.Log("FindShortestPathObject() called");
                tempbd = bd;
                if (tempbd.mBI.IsDestroyed != true)
                {
                    //If soldier has movement path, we use it last movememnt node as his position
                    //to avoid inaccuracy position conversion
                    if (mAStarPath != null)
                    {
                        //soldierindex = Utility.ConvertFloatPositionToRC(mFinalMovePosition);
                        Utility.ConvertFloatPositionToRC(mFinalMovePosition,ref soldierindex);
                    }
                    else
                    {
                        //soldierindex = Utility.ConvertFloatPositionToRC(transform.position);
                        Utility.ConvertFloatPositionToRC(transform.position,ref soldierindex);
                    }

                    //soldierindex = 
                    //bdindex = Utility.ConvertIndexToRC(tempbd.mBI.mIndex); /*Utility.ConvertFloatPositionToRC(tempbd.transform.position)*/;
                    Utility.ConvertIndexToRC(tempbd.mBI.mIndex,ref bdindex);
                    Utility.Log("soldierindex = " + soldierindex);
                    Utility.Log("bdindex = " + bdindex);

                    mSeeker.UpdateSearchInfo((int)(soldierindex.x), (int)(soldierindex.y), (int)(bdindex.x), (int)(bdindex.y), mAttackDistance);
                    mSeeker.CreatePathAStar();
                    SearchAStar.PathInfo pathinfo = mSeeker.mAstarSearch.AStarPathInfo.DeepCopy();
                    mPathsInfo.Add(new Pair<int, SearchAStar.PathInfo>(tempbd.mBI.mIndex, pathinfo));
                }
                pathindex++;
            }
        }

        mShortestPathObject = FindShortestPathTarget();

        //mCurrentCalculatingPaths = null;

        return mShortestPathObject;
    }

    private Building FindShortestPathTarget()
    {
        /*mShortestPath = null;*/

        int index = -1;
        float distance = Mathf.Infinity;
        Pair<int, SearchAStar.PathInfo> pair;
        for (int i = 0; i < mPathsInfo.Count; i++)
        {
            pair = mPathsInfo[i];
            if (/*pair.Value.GetCostToTarget()*/ pair.Value.CostToTarget < distance)
            {
                index = pair.Key;
                distance = /*pair.Value.GetCostToTarget()*/pair.Value.CostToTarget;
                mShortestPath = pair.Value;
            }
        }

        if (/*mShortestPath != null &&*/ mShortestPath.IsWallInPathToTarget)
        {
            Utility.Log("mShortestPath.mWallInPathToTargetIndex = " + mShortestPath.WallInPathToTargetIndex);
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
            mDistanceToTarget = Vector3.Distance(transform.position, MapManager.MMInstance.NodeTerrainList[mAttackingObject.mBI.mIndex].Position /*mAttackingObject.mBI.Position*/);
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
        if (mAttackTimer >= mAttackInterval)
        {
            mAttackTimer = 0.0f;
            //GameObject bl = MonoBehaviour.Instantiate (mBullet, mBulletSpawnPoint, Quaternion.identity) as GameObject;
            GameObject bl = ObjectPoolManager.mObjectPoolManagerInstance.GetSoldierBulletObject();
            bl.transform.position = mBulletSpawnPoint;
            bl.GetComponent<Bullet>().AttackTarget = mAttackingObject;
        }
    }

    public void Move()
    {
        if (mAttackingObject != null)
        {
            if (mAStarPath == null)
            {
                //We have no path to move after yet
                return;
            }

            //Never reach the final point(to avoid soldier reach wall position), so give soldier at least 1 strick distance
            if (mCurrentWayPoint < 0)
            {
                //Due to we set mAStarPath[mCurrentWayPoint] = transform.position;
                //We must make sure mAStarPath[mCurrentWayPoint].Position is not located at wall position
                Vector2 soldierindex = Utility.ConvertFloatPositionToRC(mAStarPath[0]);

                int index = Utility.ConvertRCToIndex((int)(soldierindex.x), (int)(soldierindex.y));

                //Move forward to target object to make sure it under soldier attack range
                //Direction to the next waypoint
                mDir = (MapManager.MMInstance.NodeTerrainList[mAttackingObject.mBI.mIndex].Position - transform.position).normalized;
                mDir.y = 0.0f;

                transform.LookAt(MapManager.MMInstance.NodeTerrainList[mAttackingObject.mBI.mIndex].Position);

                Wall wa;
                if (MapManager.MMInstance.NodeTerrainList[index].IsWall)
                {
                    float distance = 0.0f;
                    distance = Vector3.Distance(transform.position, mAStarPath[mCurrentWayPoint]);
                    wa = MapManager.MMInstance.BuildingsInfoInGame[index] as Wall;
                    if (wa != null && wa.LatestJumpSpell != null && wa.LatestJumpSpell.TimeRemain * mSpeed / 2 < distance)
                    {
                        transform.position = transform.position;
                    }
                    else
                    {
                        //Avoid walk through the wall directly
                        if (!wa.CanJump() && AttackTarget != wa)
                        {
                            transform.position = transform.position;
                            mBMakeNewDecision = true;
                            return;
                        }
                        //Here we set speed to 0.5f to avoid soldier move too far from destination node
                        mNewposition = transform.position + mDir * 0.2f * Time.deltaTime;
                        transform.position = mNewposition;
                    }
                }
                else
                {
                    //Here we set speed to 0.5f to avoid soldier move too far from destination node
                    mNewposition = transform.position + mDir * 0.2f * Time.deltaTime;
                    transform.position = mNewposition;
                }
                //Here we set speed to 0.5f to avoid soldier move too far from destination node
                //mNewposition = transform.position + mDir * 0.2f * Time.deltaTime;
                //transform.position = mNewposition;

                Utility.Log("End Of Path Reached");

                if (MapManager.MMInstance.NodeTerrainList[index].IsWall)
                {
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
                //Due to we set mAStarPath[mCurrentWayPoint] = transform.position;
                //We must make sure mAStarPath[mCurrentWayPoint].Position is not located at wall position
                Vector2 soldierindex = Utility.ConvertFloatPositionToRC(mAStarPath[mCurrentWayPoint]);

                int index = Utility.ConvertRCToIndex((int)(soldierindex.x), (int)(soldierindex.y));

                //Direction to the next waypoint
                mDir = (mAStarPath[mCurrentWayPoint] - transform.position).normalized;
                mDir.y = 0.0f;

                transform.LookAt(mAStarPath[mCurrentWayPoint]);

                Wall wa;
                if (MapManager.MMInstance.NodeTerrainList[index].IsWall)
                {
                    float distance = 0.0f;
                    distance = Vector3.Distance(transform.position, mAStarPath[mCurrentWayPoint]);
                    wa = MapManager.MMInstance.BuildingsInfoInGame[index] as Wall;
                    if (wa.LatestJumpSpell != null && wa.LatestJumpSpell.TimeRemain * mSpeed / 2 < distance)
                    {
                        transform.position = transform.position;
                    }
                    else
                    {
                        //Avoid walk through the wall directly
                        if (wa != null && !wa.CanJump() && AttackTarget != wa)
                        {
                            transform.position = transform.position;
                            mBMakeNewDecision = true;
                            return;
                        }
                        mNewposition = transform.position + mDir * mSpeed * Time.deltaTime;
                        transform.position = mNewposition;
                    }
                }
                else
                {
                    mNewposition = transform.position + mDir * mSpeed * Time.deltaTime;
                    transform.position = mNewposition;
                }

                //Check if we are close enough to the next waypoint
                if (Vector3.Distance(transform.position, mAStarPath[mCurrentWayPoint]) < mNextWaypointDistance)
                {
                    if (MapManager.MMInstance.NodeTerrainList[index].IsWall)
                    {
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
    }
}