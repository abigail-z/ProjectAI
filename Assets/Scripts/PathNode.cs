using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathNode : MonoBehaviour, IComparable
{
    public int index = 0;

    public int CompareTo(object obj)
    {
        PathNode otherNode = obj as PathNode;
        if (otherNode != null)
        {
            return index - otherNode.index;
        }
        else
        {
            throw new ArgumentException("Object is not a PathNode");
        }
    }
}
