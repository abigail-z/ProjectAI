using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathNode : MonoBehaviour
{
    public Path path;
    public bool criticalSection;
    public Vector3 Pos { get { return transform.position; } }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        path.OnDrawGizmosSelected();
    }
#endif
}
