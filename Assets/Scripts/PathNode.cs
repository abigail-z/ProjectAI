using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathNode : MonoBehaviour
{
    public Path path;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        path.OnDrawGizmosSelected();
    }
#endif
}
