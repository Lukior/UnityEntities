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
	///         let's draw this baby.
	/// </summary>
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		InpectAttributes();

		serializedObject.ApplyModifiedProperties();
	}

	#region Attributes Handling

	/// <summary>
	///         Inpects all the attributes
	///         assigned to the target entity.
	/// </summary>
	protected virtual void InpectAttributes()
	{
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
			ShowAttributes[i] = EditorGUILayout.ToggleLeft(attribute.GetType().Name
				, ShowAttributes[i]);
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
		ShowAttributes = new bool[Attributes.arraySize];
	}
}