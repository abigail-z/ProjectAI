using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    // top level transforms
    private Transform sphere;
    private Transform car;

    // control vars
    public float maxTurnRate;
    public float accelForce;
    public float requiredTurningVelocity;
    
    // chassis tilting vars
    public float maxTopTilt;
    public float topTiltDelta;
    public float maxTiltSpeed;
    private Transform chassis;

    // wheel vars
    public float frontWheelTurnAngle;
    public float wheelTurnDelta;
    private Transform leftFrontWheel;
    private Transform rightFrontWheel;

    // misc privates
    private Vector3 offset;
    private Rigidbody sphereRB;

    // Start is called before the first frame update
    void Start()
    {
        // transforms
        sphere = transform.Find("Sphere");
        car = transform.Find("Model");
        chassis = car.Find("Chassis");
        leftFrontWheel = car.Find("Front Left");
        rightFrontWheel = car.Find("Front Right");

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

        // animating car chassis
        float perpendicularSpeed = -Vector3.Dot(car.right, sphereRB.velocity);
        Quaternion topTargetRot = Quaternion.AngleAxis((perpendicularSpeed / maxTiltSpeed) * maxTopTilt, Vector3.forward);
        chassis.localRotation = Quaternion.RotateTowards(chassis.localRotation, topTargetRot, topTiltDelta * Time.deltaTime);

        // animating tires
        Quaternion frontWheelTargetRot = Quaternion.AngleAxis(frontWheelTurnAngle * horizontalInput, Vector3.up);
        rightFrontWheel.localRotation = leftFrontWheel.localRotation
            = Quaternion.RotateTowards(leftFrontWheel.localRotation, frontWheelTargetRot, wheelTurnDelta * Time.deltaTime);
    }
}
