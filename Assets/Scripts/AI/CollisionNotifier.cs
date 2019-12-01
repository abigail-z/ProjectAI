using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{
    private List<ICollisionSubscriber> subscribers;
    
    void Awake()
    {
        subscribers = new List<ICollisionSubscriber>();
    }

    void OnCollisionEnter(Collision col)
    {
        foreach (ICollisionSubscriber sub in subscribers)
        {
            sub.OnCollision(col);
        }
    }

    public void Subscribe(ICollisionSubscriber sub)
    {
        subscribers.Add(sub);
    }

    public void Unsubscribe(ICollisionSubscriber sub)
    {
        subscribers.Remove(sub);
    }
}

public interface ICollisionSubscriber
{
    void OnCollision(Collision col);
}
