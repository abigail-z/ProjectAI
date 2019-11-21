using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
    public Path path;
    public Transform car;

    void Update()
    {
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
            PathPointInfo ppi = path.findClosestLeadingPoint(car.position, 5);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(ppi.point, 0.25f);
            Gizmos.DrawLine(ppi.point, ppi.point + ppi.direction);
    }
#endif
}
