using UnityEngine;
using AttributeMap =
	System.Collections.Generic.Dictionary<string, AttributeMechanic>;

/// <summary>
///         30/08/2014: The main class of the
///         thing. The entity will support many
///         methods of extension, like generically
///         managed attributes and states. Will
///         certainly come with a pooling method.
/// </summary>
public class Entity : MonoBehaviour
{
	/// <summary>
	///         The map of attributes supported by
	///         this entity, to reference them by
	///         name directly.
	/// </summary>
	protected AttributeMap AttributesMap;

	/// <summary>
	///         The array of attributes supported
	///         by this entity;
	/// </summary>
	public AttributeMechanic[] Attributes;
}