using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<PathNode> nodes;
    public float radius;

    public PathPointInfo FindClosestLeadingPoint(Vector3 point, float leadDistance)
    {
        int index = 0;
        float distanceOnLine = 0;
        float lowestMagnitude = float.MaxValue;

        // first, grab the closest line and line point
        for (int i = 0; i < nodes.Count; ++i)
        {
            // first grab 2 nodes in sequence
            // this will represent our line
            PathNode startNode = GetNode(i);
            PathNode endNode = GetNode(i + 1);

            // find the closest point on this line
            Vector3 lineDir = (endNode.Pos - startNode.Pos).normalized;
            float maxDistance = (endNode.Pos - startNode.Pos).magnitude;
            float distance = Mathf.Clamp(Vector3.Dot(point - startNode.Pos, lineDir), 0, maxDistance);
            Vector3 pointOnLine = startNode.Pos + lineDir * distance;

            // check if the line point is the closest one, if it is store it
            float magnitude = (point - pointOnLine).magnitude;
            if (magnitude < lowestMagnitude)
            {
                index = i;
                distanceOnLine = distance;
                lowestMagnitude = magnitude;
            }
        }

        return GetLeadingPointAndDir(index, leadDistance, distanceOnLine);
    }

    PathPointInfo GetLeadingPointAndDir(int i, float leadDistance, float distanceOnLine)
    {
        PathNode startNode = GetNode(i);
        PathNode endNode = GetNode(i + 1);

        Vector3 lineDir = (endNode.Pos - startNode.Pos).normalized;
        float lineLength = (endNode.Pos - startNode.Pos).magnitude;

        // if lead point overruns the current line, bend it around corners
        if (distanceOnLine + leadDistance > lineLength)
        {
            float overrunDistance = distanceOnLine + leadDistance - lineLength;
            startNode = endNode;
            endNode = GetNode(i + 2);
            lineLength = (endNode.Pos - startNode.Pos).magnitude;
            // keep bending around corners until there's no overrun
            int j = 3;
            while (overrunDistance > lineLength)
            {
                // subtract the line length from the overrun
                overrunDistance -= lineLength;
                // get the next line and calculate its length
                startNode = endNode;
                endNode = GetNode(i + j);
                lineLength = (endNode.Pos - startNode.Pos).magnitude;
                // update iterator
                j += 1;
            }

            lineDir = (endNode.Pos - startNode.Pos).normalized;
            Vector3 leadingPoint = startNode.Pos + lineDir * overrunDistance;

            return new PathPointInfo
            {
                point = leadingPoint,
                direction = lineDir,
                criticalSection = startNode.criticalSection
            };
        }
        else
        {
            // otherwise, map it on this line as expected
            Vector3 leadingPoint = startNode.Pos + lineDir * distanceOnLine + lineDir * leadDistance;

            return new PathPointInfo
            {
                point = leadingPoint,
                direction = lineDir
            };
        }
    }

    PathNode GetNode(int i)
    {
        return nodes[i % nodes.Count];
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            // get nodes
            Vector3 currentNode = nodes[i].Pos;
            Vector3 nextNode;
            if (i == nodes.Count - 1)
            {
                nextNode = nodes[0].Pos;
            }
            else
            {
                nextNode = nodes[i + 1].Pos;
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
