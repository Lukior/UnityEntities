using System.Collections;
using UnityEngine;

public class TestState : MooreState
{
	private MooreMachine fsm;

	public override IEnumerator StateLogic()
	{
		while (true)
		{
			yield return null;
			if (Input.GetKeyDown(KeyCode.Space))
			{
				fsm.ChangeState<OtherTestState>();
			}
		}
	}

	public override void Launch(MooreMachine mooreMachine)
	{
		fsm = mooreMachine;
		Debug.Log("New state");
	}

	public override void End()
	{
		Debug.Log("Endy");
	}
}