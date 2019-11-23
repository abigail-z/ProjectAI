using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtil
{
    static public Vector3 GetLineIntersectionPoint(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2, out bool found)
    {
        float tmp = (B2.x - B1.x) * (A2.z - A1.z) - (B2.z - B1.z) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            found = false;
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.z - A1.z) - (A1.z - B1.z) * (A2.x - A1.x)) / tmp;

        found = true;

        return new Vector3(
            B1.x + (B2.x - B1.x) * mu,
            0,
            B1.z + (B2.z - B1.z) * mu
        );
    }
}
