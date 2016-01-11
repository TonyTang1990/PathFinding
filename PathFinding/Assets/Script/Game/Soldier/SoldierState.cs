using UnityEngine;
using System.Collections;

public interface SoldierState {

	void EnterState();
	
	void UpdateState();
	
	void ExitState();

	void ToMoveState();

	void ToAttackState();

	void ToDeadState();
}
