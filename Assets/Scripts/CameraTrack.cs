using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    public float predictionTime;
    public float maxFollowTime;
    public Rigidbody toTrack;

    private Vector3 offset;
    private Vector3 refVelocity;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 predictedPos = toTrack.position + toTrack.velocity * predictionTime;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, predictedPos + offset, ref refVelocity, maxFollowTime);
        newPos.y = offset.y;
        transform.position = newPos;
    }
}
