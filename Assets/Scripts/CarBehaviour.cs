using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    public float maxTurnRate;
    public float accelForce;
    public float requiredTurningVelocity;
    public Transform tiltablePiece;
    public Transform sphere;
    public Transform car;
    public float maxTopTilt;
    public float topTiltDelta;

    private Vector3 offset;
    private Rigidbody sphereRB;

    // Start is called before the first frame update
    void Start()
    {
        offset = car.position - sphere.position;
        sphereRB = sphere.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 input = car.forward * verticalInput;

        sphereRB.AddForce(input * accelForce, ForceMode.Acceleration);
    }

    void Update()
    {
        // steering
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        /*
        if (Vector3.Dot(car.forward, sphereRB.velocity) >= 0)
        {
            horizontalInput *= -1;
        }
        */

        float turnRate = maxTurnRate * Mathf.Clamp(sphereRB.velocity.magnitude / requiredTurningVelocity, 0, maxTurnRate);

        car.position = sphere.position + offset;
        if (Vector3.Dot(car.forward, sphereRB.velocity) >= 0)
        {
            car.forward = Quaternion.AngleAxis(horizontalInput * turnRate * Time.deltaTime, Vector3.up) * car.forward;
        }
        else
        {
            car.forward = Quaternion.AngleAxis(horizontalInput * -1 * turnRate * Time.deltaTime, Vector3.up) * car.forward;
        }

        // animating model
        Quaternion topTargetRot = Quaternion.AngleAxis(horizontalInput * (turnRate / maxTurnRate) * maxTopTilt, Vector3.forward);
        tiltablePiece.localRotation = Quaternion.RotateTowards(tiltablePiece.localRotation, topTargetRot, topTiltDelta * Time.deltaTime);
    }
}
