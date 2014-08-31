using System;
using UnityEngine;

/// <summary>
///         This class handles states transition
///         for the entity system.
/// </summary>
[RequireComponent(typeof(Entity)), Serializable]
public sealed class MooreMachine : MonoBehaviour
{
	/// <summary>
	///         The current state of the machine.
	/// </summary>
	public MooreState CurrentState;

	/// <summary>
	///         The current name of the state
	///         machine.
	/// </summary>
	public string Name;

	/// <summary>
	///         The entity the machine is attached
	///         to.
	/// </summary>
	public Entity Entity { get; private set; }

	/// <summary>
	///         Starts the state machine.
	/// </summary>
	private void Start()
	{
		Entity = GetComponent<Entity>();
		CurrentState.Launch(this);
		StartCoroutine(CurrentState.StateLogic());
	}

	/// <summary>
	///         Stops the state machine.
	/// </summary>
	private void OnDisable()
	{
		CurrentState.End();
		StopAllCoroutines();
	}

	/// <summary>
	///         Sets a new state for this machine.
	/// </summary>
	/// <param name="newState">
	///         The new state of the machine.
	/// </param>
	public void ChangeState<T>()
		where T : MooreState
	{
		CurrentState.End();
		StopAllCoroutines();
		DestroyImmediate(CurrentState);

		CurrentState = MooreState.NewState<T>();
		CurrentState.Launch(this);
		StartCoroutine(CurrentState.StateLogic());
	}
}