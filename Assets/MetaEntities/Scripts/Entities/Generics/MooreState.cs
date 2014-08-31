using System;
using System.Collections;
using UnityEngine;

/// <summary>
///         This interface is the model to
///         implement states used in the Moore
///         machine.
/// </summary>
[Serializable]
public abstract class MooreState : ScriptableObject
{
	/// <summary>
	///         The state main logic.
	/// </summary>
	/// <returns> This is a coroutine. </returns>
	public abstract IEnumerator StateLogic();

	/// <summary>
	///         Executed at the start of the
	///         state.
	/// </summary>
	/// <param name="mooreMachine">
	///         The machine managing this state.
	/// </param>
	public abstract void Launch(MooreMachine mooreMachine);

	/// <summary>
	///         Executed at the end of the state.
	/// </summary>
	public abstract void End();

	/// <summary>
	///         Creates a new instance of state T.
	/// </summary>
	/// <typeparam name="T">
	///         The state to instantiate.
	/// </typeparam>
	/// <returns>
	///         The instantiated state.
	/// </returns>
	public static MooreState NewState<T>()
		where T : MooreState
	{
		return CreateInstance<T>();
	}
}