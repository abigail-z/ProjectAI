using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    // top level transforms
    private Transform sphere;
    private Transform car;

    // input vars
    private CarInput input;

    // control vars
    public float maxTurnRate;
    public float accelForce;
    public float requiredTurningVelocity;
    [HideInInspector]
    public int boostCount;
    public float boostAmount;
    
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
    private Transform leftRearWheel;
    private Transform rightRearWheel;
    private Quaternion turnRot;
    private float velocityRot;

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
        leftRearWheel = car.Find("Rear Left Tire");
        rightRearWheel = car.Find("Rear Right Tire");

        offset = car.position - sphere.position;
        sphereRB = sphere.GetComponent<Rigidbody>();

        // particle systems
        particleSystems = new ParticleSystem[]
        {
            leftFrontWheel.GetComponentInChildren<ParticleSystem>(),
            rightFrontWheel.GetComponentInChildren<ParticleSystem>(),
            leftRearWheel.GetComponentInChildren<ParticleSystem>(),
            rightRearWheel.GetComponentInChildren<ParticleSystem>()
        };
        foreach (ParticleSystem p in particleSystems)
        {
            p.Play();
            var emission = p.emission;
            emission.rateOverTime = 0;
        }

        // input
        input = new CarInput
        {
            acceleration = 0,
            turn = 0
        };
    }

    void FixedUpdate()
    {
        // don't allow applying forces if input is blocked
        if (CountdownBehaviour.Instance.InputBlocked)
        {
            return;
        }

        // acceleration
        Vector3 accel = car.forward * input.acceleration;
        sphereRB.AddForce(accel * (accelForce + boostCount * boostAmount), ForceMode.Acceleration);
    }

    void Update()
    {
        // steering
        float turnRate = maxTurnRate * Mathf.Clamp(sphereRB.velocity.magnitude / requiredTurningVelocity, 0, maxTurnRate);

        car.position = sphere.position + offset;
        if (!CountdownBehaviour.Instance.InputBlocked && Vector3.Dot(car.forward, sphereRB.velocity) >= 0)
        {
            car.forward = Quaternion.AngleAxis(input.turn * turnRate * Time.deltaTime, Vector3.up) * car.forward;
        }
        else
        {
            car.forward = Quaternion.AngleAxis(input.turn * -1 * turnRate * Time.deltaTime, Vector3.up) * car.forward;
        }

        // animating car chassis
        float perpendicularSpeed = -Vector3.Dot(car.right, sphereRB.velocity);
        float topTargetRotDeg = Mathf.Clamp(perpendicularSpeed / maxTiltSpeed * maxTopTilt, -maxTopTilt, maxTopTilt);
        Quaternion topTargetRot = Quaternion.AngleAxis(topTargetRotDeg, Vector3.forward);
        chassis.localRotation = Quaternion.RotateTowards(chassis.localRotation, topTargetRot, topTiltDelta * Time.deltaTime);

        // animating tires
        // side-to-side
        Quaternion frontWheelTargetRot = Quaternion.AngleAxis(frontWheelTurnAngle * input.turn, Vector3.up);
        turnRot = Quaternion.RotateTowards(turnRot, frontWheelTargetRot, wheelTurnDelta * Time.deltaTime);
        // rotation
        float forwardSpeed = Vector3.Dot(car.forward, sphereRB.velocity);
        velocityRot += forwardSpeed * Time.deltaTime * 180;
        velocityRot %= 360;
        // applied
        rightFrontWheel.localRotation = leftFrontWheel.localRotation = turnRot * Quaternion.AngleAxis(velocityRot, Vector3.right);
        rightRearWheel.localRotation = leftRearWheel.localRotation = Quaternion.AngleAxis(velocityRot, Vector3.right);

        // apply smoke effects
        foreach (ParticleSystem p in particleSystems)
        {
            var emission = p.emission;
            emission.rateOverTime = maxEmissionRate * Mathf.Clamp01((Mathf.Abs(perpendicularSpeed) - minSmokeSpeed) / (maxSmokeSpeed - minSmokeSpeed));
        }
    }

    public void ApplyInput(CarInput input)
    {
        this.input = input;
    }
}
