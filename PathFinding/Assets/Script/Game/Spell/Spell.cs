using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour {

	public float mLifeTime = 5.0f;

	public float mValidRange = 14.0f;

	protected SpellDetectRange mSpellDetectRange;

	protected SphereCollider mCollider;

	public virtual void Awake()
	{
		mSpellDetectRange = GetComponent<SpellDetectRange> ();

		mCollider = GetComponent<SphereCollider> ();

		Vector3 currentscale = transform.localScale;
		currentscale.x = mValidRange / 2;
		currentscale.z = mValidRange / 2;
		transform.localScale = currentscale;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
