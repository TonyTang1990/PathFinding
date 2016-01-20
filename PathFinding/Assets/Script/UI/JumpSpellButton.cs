using UnityEngine;
using System.Collections;

public class JumpSpellButton : MonoBehaviour {
	public void onClick()
	{
		Debug.Log ("JumpSpellButton::onClick");
		MapManager.MMInstance.setCurrentSelectedSpell (SpellType.EST_JUMPSPELL);
	}
}
