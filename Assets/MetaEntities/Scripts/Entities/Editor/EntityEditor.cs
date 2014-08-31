using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
///         This class permits us to edit Entities
///         the most encapsulating way. It is
///         surely overencapsulated but fuck this;
/// </summary>
[CustomEditor(typeof(Entity), true), CanEditMultipleObjects]
public class EntityEditor : Editor
{
	/// <summary>
	///         The array of attributes of our
	///         target.
	/// </summary>
	protected SerializedProperty Attributes;

	/// <summary>
	///         The array of state machines of out
	///         target.
	/// </summary>
	protected SerializedProperty StateMachines;

	/// <summary>
	///         An array representing whether we
	///         should display the editor for each
	///         attributes.
	/// </summary>
	protected bool[] ShowAttributes;

	/// <summary>
	///         The index selected in the
	///         attribute selector.
	/// </summary>
	protected int SelectedIndex;

	/// <summary>
	///         The indexes selected in the start
	///         state selectors.
	/// </summary>
	protected int[] StatesIndex;

	/// <summary>
	///         let's draw this baby.
	/// </summary>
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		InspectAttributes();
		EditorGUILayout.Separator();
		InspectStateMachines();

		serializedObject.ApplyModifiedProperties();
	}

	#region State Machines Handling

	/// <summary>
	///         The current state machine name.
	/// </summary>
	protected string CurrentMachineName = "Default";

	/// <summary>
	///         Lets us interacts with the state
	///         machines of the entity.
	/// </summary>
	protected virtual void InspectStateMachines()
	{
		EditorGUILayout.HelpBox("State Machines Management", MessageType.None);
		GUILayout.BeginHorizontal();
		AddStateMachineButton();
		GUILayout.EndHorizontal();
		EditorGUILayout.Separator();

		var guids = AssetDatabase.FindAssets("t:Monoscript");
		var scripts = new List<MonoScript>();

		for (int j = 0; j < guids.Length; j++)
		{
			var path = AssetDatabase.GUIDToAssetPath(guids[j]);
			if (path.Contains(".cs"))
			{
				var compClass =
					((MonoScript)AssetDatabase.LoadMainAssetAtPath(path));
				if (compClass.GetClass().IsSubclassOf(typeof(MooreState)))
				{
					scripts.Add(compClass);
				}
			}
		}

		for (int i = 0; i < StateMachines.arraySize; i++)
		{
			var fsm = (MooreMachine)StateMachines
				.GetArrayElementAtIndex(i)
				.objectReferenceValue;

			if (fsm == null)
			{
				continue;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(fsm.Name);
			SelectStartState(new SerializedObject(fsm), scripts.ToArray(), i);
			RemoveStateMachineButton(fsm);
			EditorGUILayout.EndHorizontal();
		}
	}

	/// <summary>
	///         Selects the start state of a state
	///         machine.
	/// </summary>
	/// <param name="fsm">   
	///         The state machine we are
	///         interacting with.
	/// </param>
	/// <param name="states">
	///         The available states types.
	/// </param>
	/// <param name="idx">   
	///         The index of the state machine.
	/// </param>
	protected virtual void SelectStartState(SerializedObject fsm,
		MonoScript[] states,
		int idx)
	{
		var names = states.Select(state => state.GetClass().Name).ToList();

		StatesIndex[idx] =
			EditorGUILayout.Popup(
				StatesIndex[idx] >= names.Count ? 0 : StatesIndex[idx],
				names.ToArray());

		var type = states[StatesIndex[idx]].GetClass();

		var prop = fsm.FindProperty("CurrentState");
		if (prop.objectReferenceValue == null
			|| prop.objectReferenceValue.GetType().Name != type.Name)
		{
			var reference = fsm.FindProperty("CurrentState").objectReferenceValue;
			if (reference != null)
			{
				DestroyImmediate(reference);
			}
			fsm.FindProperty("CurrentState").objectReferenceValue = CreateInstance(type);
		}
		fsm.ApplyModifiedProperties();
	}

	/// <summary>
	///         Lets us add a state machine on the
	///         fly.
	/// </summary>
	protected virtual void AddStateMachineButton()
	{
		CurrentMachineName = EditorGUILayout.TextField("Machine Name :"
			, CurrentMachineName);
		if (GUILayout.Button("Add"))
		{
			if (CurrentMachineName == String.Empty)
			{
				return;
			}

			var fsms = ((Entity)target).GetComponents<MooreMachine>();
			if (fsms.Any(machine => machine.Name == CurrentMachineName))
			{
				CurrentMachineName = "";
				return;
			}

			var fsm = ((Entity)target).gameObject.AddComponent<MooreMachine>();
			fsm.hideFlags = HideFlags.HideInInspector;
			fsm.Name = CurrentMachineName;
			StateMachines.InsertArrayElementAtIndex(StateMachines.arraySize);
			StateMachines
				.GetArrayElementAtIndex(StateMachines.arraySize - 1)
				.objectReferenceValue = fsm;
			CurrentMachineName = "";
			Array.Resize(ref StatesIndex, StateMachines.arraySize);
		}
	}

	/// <summary>
	///         Lets us remove a state machine on
	///         the fly.
	/// </summary>
	/// <param name="fsm">
	///         The state machine with impending
	///         doom.
	/// </param>
	protected virtual void RemoveStateMachineButton(MooreMachine fsm)
	{
		if (GUILayout.Button("Remove"))
		{
			for (int i = 0; i < StateMachines.arraySize; i++)
			{
				var machine = StateMachines.GetArrayElementAtIndex(i);
				if (machine.objectReferenceValue == null)
				{
					continue;
				}

				if (((MooreMachine)machine.objectReferenceValue).Name == fsm.Name)
				{
					StateMachines.DeleteArrayElementAtIndex(i);

					var fsms = ((Entity)target).GetComponents<MooreMachine>();
					for (int j = 0; j < fsms.Length; j++)
					{
						if (fsms[j].Name == fsm.Name)
						{
							DestroyImmediate(fsms[j].CurrentState);
							DestroyImmediate(fsms[j]);
							DestroyImmediate(fsm);
						}
					}

					for (int j = i; j < StateMachines.arraySize; j++)
					{
						StateMachines.MoveArrayElement(j + 1, j);
					}
					StateMachines.arraySize = StateMachines.arraySize - 1;
					Array.Resize(ref StatesIndex, StateMachines.arraySize);
				}
			}
		}
	}

	#endregion State Machines Handling

	#region Attributes Handling

	/// <summary>
	///         Inpects all the attributes
	///         assigned to the target entity.
	/// </summary>
	protected virtual void InspectAttributes()
	{
		EditorGUILayout.HelpBox("Attributes Management", MessageType.None);
		GUILayout.BeginHorizontal();
		AddAttributeButton();
		GUILayout.EndHorizontal();

		for (int i = 0; i < Attributes.arraySize; i++)
		{
			var attribute = (AttributeMechanic)Attributes
				.GetArrayElementAtIndex(i)
				.objectReferenceValue;

			if (attribute == null)
			{
				continue;
			}

			EditorGUILayout.BeginHorizontal();
			ShowAttributes[i] = EditorGUILayout.Foldout(ShowAttributes[i]
				, attribute.GetType().Name);
			RemoveAttributeButton(attribute);
			EditorGUILayout.EndHorizontal();

			if (ShowAttributes[i])
			{
				EditorGUI.indentLevel++;
				InspectSingleAttribute(Attributes
					.GetArrayElementAtIndex(i));
				EditorGUI.indentLevel--;
			}
		}
	}

	/// <summary>
	///         Enables us to inspect an attribute
	///         within the entity editor. Goes
	///         through great length for
	///         genericity.
	/// </summary>
	/// <param name="attribute">
	///         The attribute to inspect.
	/// </param>
	/// <remarks>
	///         Lays your eyes upon this and alas
	///         mock me for else if forest. This
	///         is none of my choice for there are
	///         no generic method for such a gest.
	///
	///			UPDATE: Turns out there was a VERY
	///			NICE way of handling that.
	/// </remarks>
	protected virtual void InspectSingleAttribute(SerializedProperty attribute)
	{
		var serObj = new SerializedObject(attribute.objectReferenceValue);
		var ite = serObj.GetIterator();

		ite.NextVisible(true);
		for (; ite.NextVisible(true); )
		{
			EditorGUILayout.PropertyField(ite, true);
		}

		serObj.ApplyModifiedProperties();
	}

	/// <summary>
	///         Manages the addition of attributes
	///         to the current entity. Goes
	///         through great lengths to find the
	///         available attributes.
	/// </summary>
	/// <remarks>
	///         This is bad and I should feel bad.
	///         I'll most certainly find a better
	///         way later.
	/// </remarks>
	protected virtual void AddAttributeButton()
	{
		var guids = AssetDatabase.FindAssets("t:Monoscript");
		var names = new List<string>();

		for (int i = 0; i < guids.Length; i++)
		{
			var path = AssetDatabase.GUIDToAssetPath(guids[i]);
			if (path.Contains(".cs"))
			{
				var compClass =
					((MonoScript)AssetDatabase.LoadMainAssetAtPath(path)).GetClass();
				if (compClass.IsSubclassOf(typeof(AttributeMechanic)))
				{
					var attribs = ((Entity)target).GetComponents<AttributeMechanic>();
					if (attribs.All(mechanic => mechanic.GetType().Name != compClass.Name))
					{
						names.Add(compClass.Name);
					}
				}
			}
		}

		SelectedIndex =
			EditorGUILayout.Popup(SelectedIndex >= names.Count ? 0 : SelectedIndex,
				names.ToArray());
		if (GUILayout.Button("Add Attribute"))
		{
			var newAttribute =
				(AttributeMechanic)
					((Entity)target).gameObject.AddComponent(names[SelectedIndex]);

			Attributes.InsertArrayElementAtIndex(Attributes.arraySize);
			Attributes
				.GetArrayElementAtIndex(Attributes.arraySize - 1)
				.objectReferenceValue = newAttribute;

			newAttribute.hideFlags = HideFlags.HideInInspector;
			Array.Resize(ref ShowAttributes, Attributes.arraySize);
		}
	}

	/// <summary>
	///         This method removes an attribute
	///         from the target entity.
	/// </summary>
	/// <param name="attribute">
	///         The attribute to remove.
	/// </param>
	protected virtual void RemoveAttributeButton(AttributeMechanic attribute)
	{
		var attribName = attribute.GetType().Name;

		if (GUILayout.Button("Remove"))
		{
			for (int i = 0; i < Attributes.arraySize; i++)
			{
				var attrib = Attributes.GetArrayElementAtIndex(i);
				if (attrib.objectReferenceValue == null)
				{
					continue;
				}

				if (attrib.objectReferenceValue.GetType().Name == attribName)
				{
					ShowAttributes[i] = false;
					Attributes.DeleteArrayElementAtIndex(i);
					DestroyImmediate(((Entity)target).GetComponent(attribName));
					for (int j = i; j < Attributes.arraySize; j++)
					{
						Attributes.MoveArrayElement(j + 1, j);
					}
					Attributes.arraySize = Attributes.arraySize - 1;
				}
			}
		}
	}

	#endregion Attributes Handling

	/// <summary>
	///         Initializes internal values.
	/// </summary>
	protected virtual void OnEnable()
	{
		Attributes = serializedObject.FindProperty("Attributes");
		StateMachines = serializedObject.FindProperty("StateMachines");
		ShowAttributes = new bool[Attributes.arraySize];
		StatesIndex = new int[StateMachines.arraySize];
	}
}