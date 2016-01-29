using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions;
using System.Collections.Generic;

[Serializable]
public enum BuildingType
{
	E_WALL = 0,
	E_HOUSE = 1,
	E_DRAWER
}

[Serializable]
public struct OccupiedSize
{
	public int mRow;
	public int mColumn;
}

[Serializable]
public struct BuildingPosition
{
	public float mX;
	public float mY;
	public float mZ;
}

[Serializable]
public class BuildingInfo
{
	public BuildingType mBT;

	public OccupiedSize mSize;

	public float mBHP;

	public Vector3 Position
	{
		get
		{
			return new Vector3(mPosition.mX, mPosition.mY, mPosition.mZ);
		}
		set
		{
			mPosition.mX = value.x;
			mPosition.mY = value.y;
			mPosition.mZ = value.z;
		}
	}
	private BuildingPosition mPosition;

	public int mIndex;

	public bool IsDestroyed
	{
		get
		{
			return mIsDestroyed;
		}
		set
		{
			mIsDestroyed = value;
		}
	}
	private bool mIsDestroyed = false;

	public bool IsBuildedCompleted {
		get {
			return mIsBuildedCompleted;
		}
		set {
			mIsBuildedCompleted = value;
		}
	}
	private bool mIsBuildedCompleted;

	public BuildingType getBuildingType()
	{
		return mBT;
	}
	
	public OccupiedSize getSize()
	{
		return mSize;
	}

}

[Serializable]
public class Building : MonoBehaviour, GameObjectType {

	public BuildingInfo mBI;

    private bool mUpdateHP = false;

	public float mWeight = 0.0f;

	public bool mAttackable = true;

	private TextMesh mHPText;

	private GameObject mBuildingUI;
	
	protected Vector3 mSpawnPoint;

	public BuildingState BCurrentState {
		set {
			if(mBCurrentState != null)
			{
				mBCurrentState.ExitState();
			}
			mBCurrentState = value;
			mBCurrentState.EnterState();
		}
	}
	[HideInInspector] private BuildingState mBCurrentState;
	
	[HideInInspector] public BuildingAttackState mBAttackState;
	
	[HideInInspector] public BuildingIdleState mBIdleState;

	[HideInInspector] public BuildingBuildState mBBuildingState;

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

	public virtual void Awake()
	{
		mHPText = gameObject.transform.Find ("HealthText").gameObject.GetComponent<TextMesh> ();
		if (mHPText == null) {
			Debug.Log("mHPText == null");
		}
		mHPText.text = "HP: " + mBI.mBHP;
		
		mBuildingUI = gameObject.transform.Find ("BuildingUI").gameObject;
		if (mBuildingUI == null) {
			Debug.Log("mBuildingUI == null");
		}
		
		mGameType = ObjectType.EOT_BUILDING;
		//Debug.Log ("Building::Awake() mGameType = " + mGameType);
		
		mBI.IsBuildedCompleted = false;
		
		Assert.IsTrue (mWeight >= 0);

	}

	public virtual void Start()
	{

	}

	public virtual void Update()
	{
		if (gameObject &&  mBCurrentState!= null) {
			mBCurrentState.UpdateState();
		}
	}

	public virtual void FixedUpdate()
	{
        if (gameObject && mUpdateHP)
        {
			mHPText.text = "HP: " + mBI.mBHP;
            mUpdateHP = false;
		}
	}

	public virtual void TakeDamage(float damage)
	{
        if (mBI.mBHP > damage)
        {
			mBI.mBHP -= damage;
		} else {
			Utility.Log("IsDestroyed == true");
			mBI.mBHP = 0;
			mBI.IsDestroyed = true;
			gameObject.SetActive(false);
		}
        mUpdateHP = true;
	}

	public virtual void UpdateChildPosition()
	{

	}

	public virtual void ActiveBuildingUI(bool isactive)
	{
		mBuildingUI.SetActive (isactive);
	}

	public virtual bool CanAttack()
	{
		return false;
	}

	public virtual void Attack()
	{

	}

	public virtual bool IsTargetAvalibleToAttack()
	{
		return false;
	}

	public List<int> GetWeightNodeIndex()
	{
		//We assume all building occupied size mRow = mColumn
		List<int> weightNodeList = new List<int> ();
		int buildingpositionnodeindex = 0;
		switch (mBI.getSize ().mRow) {
		case 1:
			buildingpositionnodeindex = Utility.ConvertRCToIndex ((int)(mBI.Position.x + 1), (int)(mBI.Position.z + 1));
			weightNodeList.Add(buildingpositionnodeindex);
			break;
		case 2:
			break;
		case 3:
			buildingpositionnodeindex = Utility.ConvertRCToIndex ((int)(mBI.Position.x + 2), (int)(mBI.Position.z + 2));
			weightNodeList.Add(buildingpositionnodeindex);
			break;
		case 4:
			for(int i = 2; i <= 3; i++)
			{
				for(int j = 2; j <=3; j++)
				{
					buildingpositionnodeindex = Utility.ConvertRCToIndex ((int)(mBI.Position.x + i), (int)(mBI.Position.z + j));
					weightNodeList.Add(buildingpositionnodeindex);
				}
			}
            break;
         default:
            break;
		}
		return weightNodeList;
	}

	public int GetBuildingIndex()
	{
		//We assume all building occupied size mRow = mColumn
		int BuildingNodeList = 0;
		int buildingpositionnoderow = 0;
		int buildingpositionnodecomlumn = 0;
		buildingpositionnoderow = (int)(mBI.Position.x + 1);
		buildingpositionnodecomlumn = (int)(mBI.Position.z + 1);

		switch (mBI.getSize ().mRow) {
		case 1:
		case 2:
			BuildingNodeList = Utility.ConvertRCToIndex(buildingpositionnoderow, buildingpositionnodecomlumn);
			break;
		case 3:
		case 4:
			BuildingNodeList = Utility.ConvertRCToIndex (buildingpositionnoderow + 1, buildingpositionnodecomlumn + 1);
			break;
		default:
			break;
		}
		return BuildingNodeList;
	}

	public int GetBuildingBottomLeftIndex()
	{
		//We assume all building occupied size mRow = mColumn
		int bottomleftindex = 0;
		Vector2 buildingindexrc = Utility.ConvertIndexToRC (mBI.mIndex);
		int buildingpositionnoderow = (int)(buildingindexrc.x);
		int buildingpositionnodecomlumn = (int)(buildingindexrc.y);

		switch (mBI.getSize ().mRow) {
		case 1:
		case 2:
			bottomleftindex = Utility.ConvertRCToIndex(buildingpositionnoderow, buildingpositionnodecomlumn);
			break;
		case 3:
		case 4:
			bottomleftindex = Utility.ConvertRCToIndex (buildingpositionnoderow - 1, buildingpositionnodecomlumn - 1);
			break;
		default:
			break;
		}
		return bottomleftindex;
	}

	/*
	void OnTriggerEnter(Collider other) {
		Debug.Log ("other.name = " + other.name);
		if (other.tag == "Bullet") {
			float damage = other.gameObject.GetComponent<Bullet> ().mDamage;
			Debug.Log ("damage = " + damage);
			if (mBI.mBHP > damage) {
				mBI.mBHP -= damage;
				Destroy (other.gameObject);
			} else {
				mBI.mBHP = 0;
				mBI.IsDestroyed = true;
				//MapManager.mMapInstance.RemoveObjectFromMap(mBI);
				Destroy (other.gameObject);
				//Destroy (gameObject);
			}
		}
	}
	*/
}
