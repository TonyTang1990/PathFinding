﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Assertions;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class MapManager : MonoBehaviour {

	public static MapManager MMInstance = null;

	//Old Begin
	private Map mMap;
	
	public List<GameObject> mBuildings;
	
	public List<GameObject> mSoldiers;
	
	private List<GameObject> mBuldingsInGame;

	public List<Building> BuildingsInfoInGame {
		get {
			return mBuldingsInfoInGame;
		}
	}
	private List<Building> mBuldingsInfoInGame;
	
	private List<GameObject> mSoldiersInGame;
	
	private List<Soldier> mSoldiersScriptInGame;

	private bool[,] mMapOccupied;
	private GameObject mCurrentSelectedBuilding;

	private Building mSelectedBuilding;

	private SoldierType mCurrentSelectedSoldierType;
	
	public bool isBuildingSelected
	{
		get
		{
			return mIsBuildingSelected;
		}
		set
		{
			mIsBuildingSelected = value;
		}
	}
	private bool mIsBuildingSelected = false;
	
	public bool isSoldierSelected
	{
		get
		{
			return mIsSoldierSelected;
		}
		set
		{
			mIsSoldierSelected = value;
		}
	}
	private bool mIsSoldierSelected = false;
	
	private Vector2 mCurrentOccupiedIndex;
	
	private string mMapSavePath;
	//Old End

	public TerrainNode CurrentSelectedNode {
		get {
			return mCurrentSelectedNode;
		}
		set {
			mCurrentSelectedNode = value;
		}
	}
	private TerrainNode mCurrentSelectedNode;

	public PathFinder PathFinder
	{
		get
		{
			return mPathFinder;
		}
	}
	private PathFinder mPathFinder;

	public List<GameObject> NodeTerrainListObject {
		get {
			return mNodeTerrainListObject;
		}
	}
	private List<GameObject> mNodeTerrainListObject;

	public List<TerrainNode> NodeTerrainList {
		get {
			return mNodeTerrainList;
		}
	}
	private List<TerrainNode> mNodeTerrainList;

	public GameObject mNodeWeight;

	private int mRow = 1;
	
	private int mColumn = 1;

	void Awake()
	{
		if (MMInstance == null) {
			MMInstance = this;
		} else if (MMInstance != this) {
			Destroy (gameObject);
		}

		mMapSavePath = Application.persistentDataPath + "/mapInfo.dat";
		Debug.Log ("mMapSavePath = " + mMapSavePath);
		
		mBuldingsInGame = new List<GameObject>();
		mBuldingsInfoInGame = new List<Building> ();
		mSoldiersInGame = new List<GameObject> ();
		mSoldiersScriptInGame = new List<Soldier>();

		mPathFinder = gameObject.GetComponent<PathFinder> ();
	}

	void Start()
	{
		LoadMap ();

		UIManager.UIMInstance.UpdateAstarInfo ();

		MapSetup ();
	}

	void LoadMap()
	{
		Debug.Log("LoadMap()");
		if (File.Exists (mMapSavePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream fs = File.Open (mMapSavePath, FileMode.Open);
			mMap = (Map)bf.Deserialize (fs);
			fs.Close ();
		} else {
			mMap = new Map ();
			SaveMap();
		}

		mRow = mPathFinder.mRow;
		mColumn = mPathFinder.mColumn;

		LoadNodeWeights ();
	}

	void SaveMap()
	{
		Debug.Log("SaveMap()");
		if (!File.Exists (mMapSavePath)) {
			FileStream fsc = File.Create(mMapSavePath);
			fsc.Close();
		}
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream fs = File.Open (mMapSavePath, FileMode.Open);
		
		bf.Serialize (fs, mMap);
		fs.Close ();
	}

	void MapSetup()
	{
		mMapOccupied = new bool[mRow ,mColumn];

		for (int i = -(mRow/2); i < mRow/2; i++) 
		{
			for(int j = -(mColumn/2); j < mColumn/2; j++)
			{
				mMapOccupied[i + mRow/2,j + mColumn/2] = false;
				
				if(mMap.getMapOccupiedInfo(i + mRow/2,j + mColumn/2) == true)
				{
					Debug.Log(string.Format("mMap.getMapOccupiedInfo[{0}][{1}] = {2}]",i + mRow/2,j + mColumn/2,mMap.getMapOccupiedInfo(i + mRow/2,j + mColumn/2)));
				}
				mMapOccupied[i + mRow/2,j + mColumn/2] = mMap.getMapOccupiedInfo(i + mRow/2,j + mColumn/2);
			}
		}

		Debug.Log("mMap.getBuildings().Capacity = " + mMap.getBuildings().Capacity);
		GameObject bd;
		Building bding;
		List<int> weightnodeindexs;
		Vector3 position;
		foreach( BuildingInfo bdi in mMap.getBuildings())
		{
			Debug.Log("bdi.getBuildingType() = " + bdi.getBuildingType());
			Debug.Log(string.Format("bd.getPosition().x = {0} .y = {1} .z = {2}", bdi.Position.x, bdi.Position.y, bdi.Position.z));
			position = new Vector3(bdi.Position.x,bdi.Position.y,bdi.Position.z);
			//position = new Vector3( mNodeTerrainList[bdi.Index].Position.x, mNodeTerrainList[bdi.Index].Position.y, mNodeTerrainList[bdi.Index].Position.z);
			bd = Instantiate(mBuildings[(int)bdi.getBuildingType()],position,Quaternion.identity) as GameObject;
			bding = bd.GetComponent<Building>();
			bding.mBI.Position = position;
			bding.mBI.mIndex = bdi.mIndex;
			Debug.Log("bding.mBI.mIndex = " + bding.mBI.mIndex);
			mBuldingsInGame.Add(bd);
			mBuldingsInfoInGame.Add(bding/*bd.GetComponent<Building>()*/);

			weightnodeindexs = bding.GetWeightNodeIndex();

			//Update Node weight
			foreach(int index in weightnodeindexs)
			{
				Debug.Log("weightnodeindex = " + index);
				UpdateSpecificNodeWeight(index, bding.mWeight);
			}

			//Update Node info
			if(bding.mBI.getBuildingType() == BuildingType.E_WALL)
			{
				int index = bding.mBI.mIndex; //Utility.ConvertRCToIndex((int)(bding.mBI.Position.x + 1),(int)(bding.mBI.Position.z + 1));
				Debug.Log("WALL Index = " + index);
				mNodeTerrainList[index].IsWall = true;
				UpdateSpecificNodeWallStatus(index,true);
			}

			Debug.Log("bdi.Position" + bdi.Position);
		}
	}

	private void LoadNodeWeights()
	{
		mNodeTerrainListObject = new List<GameObject> ( mRow * mColumn);
		mNodeTerrainList = new List<TerrainNode> (mRow * mColumn);

		GameObject tempobject = new GameObject ();
		TerrainNode tempnode;

		Vector3 nodeposition = new Vector3 ();
		int nextindex = 0;
		//SparseGraph nodes data
		for (int rw = 0; rw < mRow; rw++) {
			for (int col = 0; col < mColumn; col++) {
				nodeposition = new Vector3 (rw, 0.0f, col);
				tempobject = Instantiate (mNodeWeight, nodeposition, Quaternion.Euler (new Vector3 (90.0f, 0.0f, 0.0f))) as GameObject;
				tempobject.name = nextindex.ToString();
				tempnode = tempobject.GetComponent<TerrainNode>();
				tempnode.Weight = 0.0f;
				tempnode.Index = nextindex;
				tempnode.RowColumnInfo = Utility.ConvertIndexToRC(nextindex);
				tempnode.Position = nodeposition;
				mNodeTerrainListObject.Add (tempobject);
				mNodeTerrainList.Add(tempnode);
				nextindex++;
			}
		}
	}
	
	public void setCurrenctSelectedBuilding(int index)
	{
		if (mCurrentSelectedBuilding) {
			Destroy(mCurrentSelectedBuilding);
		}
		mCurrentSelectedBuilding = Instantiate(mBuildings [index],new Vector3(0.0f,100.0f,0.0f),Quaternion.identity) as GameObject;
		mSelectedBuilding = mCurrentSelectedBuilding.GetComponent<Building> ();
		mIsBuildingSelected = true;
		mIsSoldierSelected = false;
	}

	// Update is called once per frame
	void Update () {
		//DrawMap ();
		if (mIsBuildingSelected) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, LayerMask.GetMask ("Terrain"))) {
				if(hit.collider)
				{
					//hit.collider.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0);
					Vector3 tempselectposition = hit.collider.transform.position;
					TerrainNode tempterrainnode = hit.collider.GetComponent<TerrainNode>();
					switch(mSelectedBuilding.mBI.getBuildingType())
					{
					case BuildingType.E_WALL:
						tempselectposition.y += 0.5f;
						break;
					case BuildingType.E_HOUSE:
						tempselectposition.x += 0.5f;
						tempselectposition.z += 0.5f;
						break;
					case BuildingType.E_DRAWER:
						tempselectposition.x += 0.9f;
						tempselectposition.z += 0.9f;
						break;
					}

					mCurrentSelectedBuilding.transform.position = tempselectposition;
					mSelectedBuilding.mBI.Position = tempselectposition;
					mCurrentOccupiedIndex = tempterrainnode.RowColumnInfo;
					mCurrentSelectedNode = mNodeTerrainList[tempterrainnode.Index];
				}
			}
		}
	}

	private bool isValideTerrainIndex(int row, int column)
	{
		bool isrowindexvalide = !( row < 0 || row > PathFinder.mRow);
		bool iscolumnindexvalide = !(column < 0 || column > PathFinder.mColumn);
		bool isvalideindex = (isrowindexvalide && iscolumnindexvalide);
		return isvalideindex;
	}
	
	public bool IsTerrainAvaibleToBuild()
	{
		bool isavaliable = true;
		if (mCurrentSelectedBuilding) {
			//Building buildinginfo = mCurrentSelectedBuilding.GetComponent<Building>();
			for(int i = 0; i < mSelectedBuilding.mBI.getSize().mRow; i++)
			{
				for(int j = 0; j < mSelectedBuilding.mBI.getSize().mColumn; j++)
				{
					if( isValideTerrainIndex((int)mCurrentOccupiedIndex.x + i, (int)mCurrentOccupiedIndex.y + j) )
					{
						if(mMapOccupied[(int)mCurrentOccupiedIndex.x + i - 1, (int)mCurrentOccupiedIndex.y + j - 1])
						{
							Debug.Log(string.Format("mMapOccupied[{0}][{1}] = {2}",(int)mCurrentOccupiedIndex.x + i - 1,(int)mCurrentOccupiedIndex.y + j - 1,mMapOccupied[(int)mCurrentOccupiedIndex.x + i - 1, (int)mCurrentOccupiedIndex.y + j - 1]));
							isavaliable = false;
						}
					}
					else{
						Debug.Log(string.Format("isValideTerrainIndex([{0}],[{1})]",(int)mCurrentOccupiedIndex.x + i,(int)mCurrentOccupiedIndex.y + j));
						Debug.Log("Index invalide");
						isavaliable = false;
						break;
					}
				}
			}
		}
		Debug.Log ("isavaliable = " + isavaliable);
		return isavaliable;
	}

	public void BuildBuilding()
	{
		Debug.Log ("mCurrentOccupiedIndex.x = " + mCurrentOccupiedIndex.x);
		Debug.Log ("mCurrentOccupiedIndex.y = " + mCurrentOccupiedIndex.y);
		
		if (mCurrentSelectedBuilding) {
			//Building buildinginfo = mCurrentSelectedBuilding.GetComponent<Building>() as Building;
			
			for( int i = 0 ; i < mSelectedBuilding.mBI.getSize().mRow; i++ )
			{
				for( int j = 0; j < mSelectedBuilding.mBI.getSize().mColumn; j++ )
				{
					mMapOccupied [(int)mCurrentOccupiedIndex.x + i - 1, (int)mCurrentOccupiedIndex.y + j - 1] = true;
					mMap.setMapOccupiedInfo((int)mCurrentOccupiedIndex.x + i - 1, (int)mCurrentOccupiedIndex.y + j - 1, true);
					//mTerrainTilesScript [(int)mCurrentOccupiedIndex.x + i, (int)mCurrentOccupiedIndex.y + j].setOccupiedBuilding (mCurrentSelectedBuilding);
				}
			}

			Debug.Log("mCurrentSelectedNode.Position = " + mCurrentSelectedNode.Position.ToString());

			mSelectedBuilding.mBI.mIndex = mSelectedBuilding.GetBuildingIndex(); /*Utility.ConvertFloatPositionToIndex(mCurrentSelectedNode.Position)*/;

			Debug.Log("mSelectedBuilding.mBI.mIndex = " + mSelectedBuilding.mBI.mIndex);

			mSelectedBuilding.UpdateChildPosition();
			
			mMap.addBuilding(mSelectedBuilding.mBI);
			
			mBuldingsInGame.Add(mCurrentSelectedBuilding);
			
			mBuldingsInfoInGame.Add(mSelectedBuilding);

			//Update Node weight
			List<int> weightnodeindexs = mSelectedBuilding.GetWeightNodeIndex();
			
			foreach(int index in weightnodeindexs)
			{
				Debug.Log("weightnodeindex = " + index);
				UpdateSpecificNodeWeight(index, mSelectedBuilding.mWeight);
			}

			//Update Node Info
			if(mSelectedBuilding.mBI.getBuildingType() == BuildingType.E_WALL)
			{
				int index = mSelectedBuilding.mBI.mIndex;
				Debug.Log("WALL Index = " + index);
				CurrentSelectedNode.IsWall = true;
				UpdateSpecificNodeWallStatus(index,true);
			}

			mCurrentSelectedBuilding = null;
			mSelectedBuilding = null;
			
			mIsBuildingSelected = false;
			
			PrintAllOccupiedInfo ();
			
			SaveMap ();
		}
	}

	public void DeselectChoosingStaff()
	{
		mIsSoldierSelected = false;
		mIsBuildingSelected = false;
	}
	
	public void setCurrentSelectedSoldier(SoldierType stp)
	{
		mCurrentSelectedSoldierType = stp;
		mIsSoldierSelected = true;
		mIsBuildingSelected = false;
	}
	
	public void DeploySoldier(Vector3 hitpoint)
	{
		GameObject go = SoldierFactory.SpawnSoldier(mCurrentSelectedSoldierType,hitpoint);
		mSoldiersInGame.Add(go);
		mSoldiersScriptInGame.Add(go.GetComponent<Soldier>());
	}
	
	public Building ObtainAttackObject(Soldier sod)
	{
		/*
		Building targetbuilding = null;
		float shortestdistance = Mathf.Infinity;
		float currentdistance = 0.0f;

		foreach (Building bd in mBuldingsInfoInGame) {
			if( bd.mBI.IsDestroyed )
			{
				//Debug.Log("IsDestroyed = " + bdi.IsDestroyed);
				continue;
			}
			else
			{
				currentdistance = Vector3.Distance(mNodeTerrainList[bd.mBI.mIndex].Position, sod.gameObject.transform.position);
			
				if( currentdistance < shortestdistance )
				{
					shortestdistance = currentdistance;
					targetbuilding = bd;
				}
			}
		}
		*/
		return sod.FindShortestPathObject(mBuldingsInfoInGame);
	}
	
	public Soldier ObtainAttackSoldier(Building bd)
	{
		Soldier targetsoldier = null;
		float shortestdistance = Mathf.Infinity;
		float currentdistance = 0.0f;
		foreach (Soldier so in mSoldiersScriptInGame) {
			if( so.IsDead )
			{
				//Debug.Log("IsDestroyed = " + bdi.IsDestroyed);
				continue;
			}
			else
			{
				currentdistance = Vector3.Distance(so.transform.position, bd.mBI.Position);
				if( currentdistance < shortestdistance )
				{
					shortestdistance = currentdistance;
					targetsoldier = so;
				}
			}
		}
		return targetsoldier;
	}
	
	void PrintAllOccupiedInfo()
	{
		for(int i = 0; i < PathFinder.mRow; i++ )
		{
			for(int j = 0; j < PathFinder.mColumn; j++)
			{
				if(mMapOccupied[i,j])
				{
					Debug.Log(string.Format("mMapOccupied[{0}][{1}] = {2}",i,j,mMapOccupied[i,j]));
				}
			}
		}
	}

	public void Search()
	{
		//mPathFinder.CreatePathAStar ();
		//GameManager.mGameInstance.AttackingSoldierSeeker.CreatePathAStar ();

		UIManager.UIMInstance.UpdateAstarInfo();
	}
	
	public void UpdateSearchInfo(int sourcerow, int sourcecolumn, int targetrow, int targetcolumn, float strickdistance)
	{
		Debug.Log (string.Format ("Source Index = [{0}][{1}]", sourcerow, sourcecolumn));

        Debug.Log (string.Format ("Target Index = [{0}][{1}]", targetrow, targetcolumn));

		GameManager.mGameInstance.AttackingSoldierSeeker.SourceCellIndex = Utility.ConvertRCToIndex (sourcerow, sourcecolumn);

		GameManager.mGameInstance.AttackingSoldierSeeker.TargetCellIndex = Utility.ConvertRCToIndex (targetrow, targetcolumn);

		GameManager.mGameInstance.AttackingSoldierSeeker.StrickDistance = strickdistance;

		Debug.Log ("GameManager.mGameInstance.AttackingSoldierSeeker.SourceCellIndex = " + GameManager.mGameInstance.AttackingSoldierSeeker.SourceCellIndex);

		Debug.Log ("GameManager.mGameInstance.AttackingSoldierSeeker.TargetCellIndex = " + GameManager.mGameInstance.AttackingSoldierSeeker.TargetCellIndex);

		Debug.Log ("GameManager.mGameInstance.AttackingSoldierSeeker.StrickDistance = " + GameManager.mGameInstance.AttackingSoldierSeeker.StrickDistance);
	}

	
	public void UpdateSpecificNodeWeight(int index, float value)
	{
		Assert.IsTrue(index >= 0 && index < mRow * mColumn);
		Assert.IsTrue(value >= 0.0f);

		mNodeTerrainList[index].Weight += value;
	
		Transform nodeweight = mNodeTerrainListObject [index].transform.FindChild("Node_Weight"); 
		nodeweight.gameObject.GetComponent<TextMesh> ().text = mNodeTerrainList[index].Weight.ToString ();
		
		//Update Edge info
		mPathFinder.UpdateNodeEdgesInfo(index, value);
	}

	public void UpdateNodeWeight(float value)
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			Debug.Log("CurrrentSelectedNode.Index = " + CurrentSelectedNode.Index);

			float weight = MapManager.MMInstance.CurrentSelectedNode.Weight + value;

			if (weight < 0) {
				MapManager.MMInstance.CurrentSelectedNode.Weight = 0;
				weight = 0.0f;
			}

			Transform nodeweight = mNodeTerrainListObject [index].transform.FindChild("Node_Weight"); 
			nodeweight.gameObject.GetComponent<TextMesh> ().text = weight.ToString ();

			MapManager.MMInstance.CurrentSelectedNode.Weight = weight;

			//Update Edge info
			mPathFinder.UpdateNodeEdgesInfo(index, value);
		}
	}

	public void UpdateSpecificNodeWallStatus(int index, bool iswall)
	{
		Assert.IsTrue(index >= 0 && index < mRow * mColumn);
		mPathFinder.UpdateNodeWallStatus(index, iswall);
	}

	public void UpdateNodeWallStatus(bool iswall)
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			mPathFinder.UpdateNodeWallStatus(index, iswall);
		}
	}

	public void UpdateNodeWallJumpableStatus(bool iswalljumpable)
	{
		if (CurrentSelectedNode != null) {
			int index = mCurrentSelectedNode.Index;
			mPathFinder.UpdateNodeWallJumpableStatus(index, iswalljumpable);
		}
	}
}
