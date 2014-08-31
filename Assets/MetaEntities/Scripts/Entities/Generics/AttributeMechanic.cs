using System;
using UnityEngine;

/// <summary>
///         This will manage a generic attribute,
///         which is a value with its notifiers
///         and range.
/// </summary>
[Serializable]
public abstract class AttributeMechanic : MonoBehaviour
{
	#region Notifications

	/// <summary>
	///         Delegate model used when notifying
	///         the modification of the attribute.
	/// </summary>
	/// <param name="newValue">
	///         The new value of the attribute.
	/// </param>
	public delegate void SetNotification(float newValue);

	/// <summary>
	///         Event fired when
	/// </summary>
	public event SetNotification OnSet;

	/// <summary>
	///         Call to fire event.
	/// </summary>
	/// <param name="newvalue">
	///         The parameter of the event.
	/// </param>
	protected virtual void CallOnSet(float newvalue)
	{
		var handler = OnSet;
		if (handler != null)
		{
			handler(newvalue);
		}
	}

	#endregion Notifications

	/// <summary>
	///         The minimum value of the
	///         attribute.
	/// </summary>
	public float MinimumValue = 0;

	/// <summary>
	///         The maximum value of the
	///         attribute.
	/// </summary>
	public float MaximumValue = 100;

	/// <summary>
	///         The value of the attribute at the
	///         initialization of the object.
	/// </summary>
	public float DefaultValue = 100;

	/// <summary>
	///         The value of the attribute in
	///         itself.
	/// </summary>
	protected float AttributeValue;

	/// <summary>
	///         Gets the internal value, and sets
	///         it clamped to its minimal and
	///         maximal values.
	/// </summary>
	public virtual float Value
	{
		get
		{
			return AttributeValue;
		}
		set
		{
			AttributeValue = Mathf.Clamp(MinimumValue, MaximumValue, value);
			CallOnSet(AttributeValue);
		}
	}

	/// <summary>
	///         Gets and sets the internal value,
	///         with normalized (between 0 and
	///         1) representation.
	/// </summary>
	public virtual float NormalizedValue
	{
		get
		{
			return Mathf.InverseLerp(MinimumValue, MaximumValue, AttributeValue);
		}
		set
		{
			AttributeValue = Mathf.Lerp(MinimumValue, MaximumValue, Mathf.Clamp01(value));
			CallOnSet(AttributeValue);
		}
	}

	/// <summary>
	///         Initializes internal value.
	/// </summary>
	protected virtual void Start()
	{
		AttributeValue = Mathf.Clamp(MinimumValue, MaximumValue, DefaultValue);
	}
}