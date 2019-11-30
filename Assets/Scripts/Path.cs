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
            Vector3 startNode = GetNode(i);
            Vector3 endNode = GetNode(i + 1);

            // find the closest point on this line
            
            Vector3 lineDir = (endNode - startNode).normalized;
            float distanceOnLine = Vector3.Dot(point - startNode, lineDir);
            Vector3 pointOnLine = startNode + lineDir * distanceOnLine;

            // if the lead point is beyond the end of the line, bend it around corners
            (Vector3, Vector3) leadingPoint = GetLeadingPointAndDir(i, leadDistance, distanceOnLine);

            // check if the line point is the closest one, if it is store its leading point
            float magnitude = (point - pointOnLine).magnitude;
            if (magnitude < lowestMagnitude)
            {
                closestPoint = leadingPoint.Item1;
                forward = leadingPoint.Item2;
                lowestMagnitude = magnitude;
            }
        }

        return new PathPointInfo
        {
            point = closestPoint,
            direction = forward
        };
    }

    (Vector3, Vector3) GetLeadingPointAndDir(int i, float leadDistance, float distanceOnLine)
    {
        Vector3 startNode = GetNode(i);
        Vector3 endNode = GetNode(i + 1);
        Vector3 lineDir = (endNode - startNode).normalized;

        float lineLength = (endNode - startNode).magnitude;

        if (distanceOnLine + leadDistance > lineLength)
        {
            float overrunDistance = distanceOnLine + leadDistance - lineLength;
            startNode = endNode;
            endNode = GetNode(i + 2);
            lineLength = (endNode - startNode).magnitude;
            // keep bending around corners until there's no overrun
            int j = 3;
            while (overrunDistance > lineLength)
            {
                // subtract the line length from the overrun
                overrunDistance -= lineLength;
                // get the next line and calculate its length
                startNode = endNode;
                endNode = GetNode(i + j);
                lineLength = (endNode - startNode).magnitude;
                // update iterator
                j += 1;
            }

            lineDir = (endNode - startNode).normalized;
            Vector3 leadingPoint = startNode + lineDir * overrunDistance;

            return (leadingPoint, lineDir);
        }
        else
        {
            // otherwise, map it on this line as expected
            Vector3 leadingPoint = startNode + lineDir * distanceOnLine + lineDir * leadDistance;

            return (leadingPoint, lineDir);
        }
    }

    Vector3 GetNode(int i)
    {
        return nodes[i % nodes.Count].position;
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
            Gizmos.color = Color.white;
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
