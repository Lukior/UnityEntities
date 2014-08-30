using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Entity), true), CanEditMultipleObjects]
public class EntityEditor : Editor
{
	protected SerializedProperty Attributes;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		foreach (var attribute in Attributes)
		{
			EditorGUILayout.ObjectField(attribute.GetType().Name
				, (UnityEngine.Object)attribute
				, typeof(AttributeMechanic)
				, true);
		}
		GUILayout.BeginHorizontal();
		AddAttributeButton();
		RemoveAttributeButton();
		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}

	protected virtual void AddAttributeButton()
	{
		GUILayout.Button("Add Attribute");
	}

	protected virtual void RemoveAttributeButton()
	{
		GUILayout.Button("Remove Attribute");
	}

	protected virtual void OnEnable()
	{
		Attributes = serializedObject.FindProperty("Attributes");
	}
}