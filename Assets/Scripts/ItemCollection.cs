using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class ItemCollection : ScriptableObject
{
    [Header("Main")]
    public Item [] ItemArray;
    public string Description;

    public Item this [int index]
    {
        get
        {
            return ItemArray [index];
        }
        set
        {
            ItemArray [index] = value;
        }
    }

    public Item PickRandom()
    {
        return ItemArray [Random.Range(0, ItemArray.Length)];
    }

    public int Length
    {
        get
        {
            if (ItemArray == null)
            {
                return 0;
            }
            return ItemArray.Length;
        }
    }

    public int ConstrainIndex(int index, bool wrap = false)
    {
        if (ItemArray.Length < 1)
        {
            throw new System.IndexOutOfRangeException("ItemCollection.ConstrainIndex(): no items in " + name);
        }

        if (index < 0) return 0;
        if (index >= ItemArray.Length)
        {
            if (wrap)
            {
                index = 0;
            }
            else
            {
                index = ItemArray.Length - 1;
            }
        }
        return index;
    }

    public void SetID()
    {
        for (int i = 0; i < ItemArray.Length; i++)
        {
            ItemArray [i].ID = i;
        }
    }
}