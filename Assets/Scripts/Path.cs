using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Transform> nodes;
    public float radius;

    public PathPointInfo FindClosestLeadingPoint(Vector3 point, float leadDistance)
    {
        Vector3 closestPoint = Vector3.zero;
        Vector3 forward = Vector3.zero;
        float lowestMagnitude = float.MaxValue;

        for (int i = 0; i < nodes.Count; ++i)
        {
            // first grab 2 nodes in sequence
            // this will represent our line
            Vector3 startNode = nodes[i].position;
            Vector3 endNode;
            Vector3 nextNode;
            if (i == nodes.Count - 2)
            {
                endNode = nodes[i + 1].position;
                nextNode = nodes[0].position;
            }
            else if (i == nodes.Count - 1)
            {
                endNode = nodes[0].position;
                nextNode = nodes[1].position;
            }
            else
            {
                endNode = nodes[i + 1].position;
                nextNode = nodes[i + 2].position;
            }

            // find the closest point on this line
            float lineLength = (endNode - startNode).magnitude;
            Vector3 lineDir = (endNode - startNode).normalized;
            float distanceOnLine = Vector3.Dot(point - startNode, lineDir);
            Vector3 pointOnLine = startNode + lineDir * distanceOnLine;
            Vector3 leadingPoint;
            if (distanceOnLine + leadDistance > (endNode - startNode).magnitude)
            {
                // if the lead point is beyond the end of the line, bend it around the corner
                float overrunDistance = distanceOnLine + leadDistance - lineLength;
                lineDir = (nextNode - endNode).normalized;
                leadingPoint = endNode + lineDir * overrunDistance;
            }
            else
            {
                // if the lead point is still on this line, do nothing
                leadingPoint = pointOnLine + lineDir * leadDistance;
            }

            // check if the line point is the closest one, if it is store its leading point
            float magnitude = (point - pointOnLine).magnitude;
            if (magnitude < lowestMagnitude)
            {
                closestPoint = leadingPoint;
                forward = lineDir;
                lowestMagnitude = magnitude;
            }
        }

        return new PathPointInfo
        {
            point = closestPoint,
            direction = forward
        };
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            // get nodes
            Vector3 currentNode = nodes[i].position;
            Vector3 nextNode;
            if (i == nodes.Count - 1)
            {
                nextNode = nodes[0].position;
            }
            else
            {
                nextNode = nodes[i + 1].position;
            }
            // draw node center
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(currentNode, Vector3.one * 0.25f);
            // draw node radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currentNode, radius);
            // draw path radius
            float angle = Mathf.Atan2(nextNode.x - currentNode.x, nextNode.z - currentNode.z) * 180 / Mathf.PI;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 left = rotation * Vector3.left;
            Vector3 right = rotation * Vector3.right;
            Gizmos.DrawLine(currentNode + left * radius, nextNode + left * radius);
            Gizmos.DrawLine(currentNode + right * radius, nextNode + right * radius);
        }
    }
#endif
}
