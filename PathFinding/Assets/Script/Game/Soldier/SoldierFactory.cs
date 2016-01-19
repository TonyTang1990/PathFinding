using UnityEngine;
using System.Collections;

public class SoldierFactory : MonoBehaviour {

	public static GameObject SpawnSoldier(SoldierType st, Vector3 spawnpoint)
	{
		return Instantiate (MapManager.MMInstance.mSoldiers [(int)st], spawnpoint, Quaternion.identity) as GameObject;
	}
}
