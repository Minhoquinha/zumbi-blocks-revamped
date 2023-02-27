using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ControlsCollection : ScriptableObject
{
    [Header("Main")]
	public Control [] ControlArray;
	public float MouseSensitivity;
	public string Description;

	public Control this [int index]
	{
		get
		{
			return ControlArray [index];
		}
		set
		{
            ControlArray [index] = value;
		}
	}

	public Control PickRandom()
	{
		return ControlArray [Random.Range(0, ControlArray.Length)];
	}

	public int Length
	{
		get
		{
			if (ControlArray == null)
			{
				return 0;
			}

			return ControlArray.Length;
		}
	}

	public int ConstrainIndex(int index, bool wrap = false)
	{
		if (ControlArray.Length < 1)
		{
			throw new System.IndexOutOfRangeException("ControlsCollection.ConstrainIndex(): no items in " + name);
		}

		if (index < 0) return 0;
		if (index >= ControlArray.Length)
		{
			if (wrap)
			{
				index = 0;
			}
			else
			{
				index = ControlArray.Length - 1;
			}
		}
		return index;
	}
}
