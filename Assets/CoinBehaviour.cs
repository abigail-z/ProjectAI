using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public Vector3 spinSpeed;

    void Update()
    {
        transform.Rotate(Time.deltaTime * spinSpeed);
    }
}
