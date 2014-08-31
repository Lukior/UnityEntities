using System.Collections;
using UnityEngine;

public class OtherTestState : MooreState
{
	public MooreMachine fsm;

	public override IEnumerator StateLogic()
	{
		yield return new WaitForSeconds(1.0f);
		Debug.Log("And back");
		fsm.ChangeState<TestState>();
	}

	public override void Launch(MooreMachine mooreMachine)
	{
		fsm = mooreMachine;
	}

	public override void End()
	{
	}
}