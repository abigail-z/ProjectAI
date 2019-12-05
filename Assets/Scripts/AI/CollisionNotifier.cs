using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{
    public delegate void OnCollision(Collision col);
    public event OnCollision CollisionEvent;

    void OnCollisionEnter(Collision col)
    {
        CollisionEvent(col);
    }
}
