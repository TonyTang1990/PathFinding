using UnityEngine;
using System.Collections;


public interface BuildingState {

	void EnterState();

	void UpdateState();

	void ExitState();
	
	void ToAttackState();
	
	void ToIdleState();
}