using UnityEngine;
using System.Collections;

public class SpellFactory : MonoBehaviour {
	
	public static GameObject SpawnSpell(SpellType st, Vector3 spawnpoint)
	{
		return Instantiate (MapManager.MMInstance.mSpells [(int)st], spawnpoint, Quaternion.identity) as GameObject;
	}
}
