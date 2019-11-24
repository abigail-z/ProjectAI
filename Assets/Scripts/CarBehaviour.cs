using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    // top level transforms
    private Transform sphere;
    private Transform car;

    // input vars
    [HideInInspector]
    public float turnInput;
    [HideInInspector]
    public float accelerationInput;

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

    // particle systems
    public float minSmokeSpeed;
    public float maxSmokeSpeed;
    public float maxEmissionRate;
    private ParticleSystem[] particleSystems;

    // misc privates
    private Vector3 offset;
    private Rigidbody sphereRB;

    void Awake()
    {
        // transforms
        sphere = transform.Find("Sphere");
        car = transform.Find("Model");
        chassis = car.Find("Chassis");
        leftFrontWheel = car.Find("Front Left Tire");
        rightFrontWheel = car.Find("Front Right Tire");

        offset = car.position - sphere.position;
        sphereRB = sphere.GetComponent<Rigidbody>();

        // particle systems
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particleSystems)
        {
            p.Play();
            var emission = p.emission;
            emission.rateOverTime = 0;
        }
    }

    void FixedUpdate()
    {
        // acceleration
        Vector3 input = car.forward * accelerationInput;
        sphereRB.AddForce(input * accelForce, ForceMode.Acceleration);
    }

    void Update()
    {
        // steering
        float turnRate = maxTurnRate * Mathf.Clamp(sphereRB.velocity.magnitude / requiredTurningVelocity, 0, maxTurnRate);

        car.position = sphere.position + offset;
        if (Vector3.Dot(car.forward, sphereRB.velocity) >= 0)
        {
            car.forward = Quaternion.AngleAxis(turnInput * turnRate * Time.deltaTime, Vector3.up) * car.forward;
        }
        else
        {
            car.forward = Quaternion.AngleAxis(turnInput * -1 * turnRate * Time.deltaTime, Vector3.up) * car.forward;
        }

        // animating car chassis
        float perpendicularSpeed = -Vector3.Dot(car.right, sphereRB.velocity);
        Quaternion topTargetRot = Quaternion.AngleAxis((perpendicularSpeed / maxTiltSpeed) * maxTopTilt, Vector3.forward);
        chassis.localRotation = Quaternion.RotateTowards(chassis.localRotation, topTargetRot, topTiltDelta * Time.deltaTime);

        // animating tires
        Quaternion frontWheelTargetRot = Quaternion.AngleAxis(frontWheelTurnAngle * turnInput, Vector3.up);
        rightFrontWheel.localRotation = leftFrontWheel.localRotation
            = Quaternion.RotateTowards(leftFrontWheel.localRotation, frontWheelTargetRot, wheelTurnDelta * Time.deltaTime);

        // apply smoke effects
        foreach (ParticleSystem p in particleSystems)
        {
            var emission = p.emission;
            emission.rateOverTime = maxEmissionRate * Mathf.Clamp01((perpendicularSpeed - minSmokeSpeed) / (maxSmokeSpeed - minSmokeSpeed));
        }
    }
}
