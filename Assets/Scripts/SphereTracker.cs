using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTracker : MonoBehaviour
{
    public PlayerControl sphere;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = sphere.transform.position.x;
        pos.z = sphere.transform.position.z;
        transform.position = pos;

        transform.forward = sphere.forward;
    }
}
