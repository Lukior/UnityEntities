using UnityEngine;

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
	///         The array of attributes supported
	///         by this entity;
	/// </summary>
	[SerializeField]
	protected AttributeMechanic[] Attributes;

	/// <summary>
	///         Adds an attribute to this entity.
	/// </summary>
	/// <typeparam name="T">
	///         The type of the attribute to add.
	/// </typeparam>
	/// <returns>
	///         The generated attribute, or the
	///         preexisting attribute if one is
	///         already present.
	/// </returns>
	/// <remarks>
	///         This method will only generate a
	///         new attribute if there is none of
	///         this type on the entity.
	/// </remarks>
	public virtual AttributeMechanic AddAttribute<T>()
		where T : AttributeMechanic
	{
		var attribute = gameObject.GetComponent<T>();
		if (!attribute)
		{
			gameObject.AddComponent<T>();
		}
		return attribute;
	}

	/// <summary>
	///         Removes an attribute from the
	///         entity.
	/// </summary>
	/// <typeparam name="T">
	///         The type of the attribute to
	///         remove.
	/// </typeparam>
	public virtual void RemoveAttribute<T>()
		where T : AttributeMechanic
	{
		var attribute = GetComponent<T>();
		if (attribute)
		{
			Destroy(attribute);
		}
	}

	/// <summary>
	///         Gets an attribute attached to the
	///         entity.
	/// </summary>
	/// <typeparam name="T">
	///         The type of attribute to get.
	/// </typeparam>
	/// <returns>
	///         The attribute if it exists, null
	///         otherwise.
	/// </returns>
	public virtual AttributeMechanic GetAttribute<T>()
		where T : AttributeMechanic
	{
		return GetComponent<T>();
	}
}