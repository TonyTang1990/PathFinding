using UnityEngine;
using System.Collections;

public enum ObjectType
{
	EOT_BUILDING = 0,
	EOT_SOLDIER = 1,
	EOT_TERRAIN = 2
}

public enum SpellType
{
	EST_JUMPSPELL = 0,
	EST_HEALINGSPELL = 1
}

public interface GameObjectType {
	ObjectType GameType
	{
		get;
		set;
	}
}
