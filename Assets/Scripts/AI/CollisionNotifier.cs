using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{
    private List<ICollisionSubscriber> subscribers;
    // Start is called before the first frame update
    void Awake()
    {
        subscribers = new List<ICollisionSubscriber>();
    }

    void OnCollisionEnter(Collision col)
    {
        foreach(ICollisionSubscriber sub in subscribers)
        {
            sub.OnCollision(col);
        }
    }

    public void Subscribe(ICollisionSubscriber sub)
    {
        subscribers.Add(sub);
    }
}

public interface ICollisionSubscriber
{
    void OnCollision(Collision col);
}
