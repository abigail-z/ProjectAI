using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float force;
    [HideInInspector]
    public Vector3 forward;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        forward = transform.forward;
    }

    void FixedUpdate()
    {
        forward = Quaternion.AngleAxis(Input.GetAxisRaw("Horizontal"), Vector3.up) * forward;
        Vector3 input = forward * Input.GetAxisRaw("Vertical");
        rb.AddForce(input * force, ForceMode.Acceleration);
    }
}
