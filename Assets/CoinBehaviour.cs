using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public Vector3 spinSpeed;
    public float respawnTime;
    private new SphereCollider collider;
    private new MeshRenderer renderer;

    void Awake()
    {
        collider = GetComponent<SphereCollider>();
        renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        transform.Rotate(Time.deltaTime * spinSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            other.transform.parent.GetComponent<CarBehaviour>().boostCount += 1;

            StartCoroutine(DisableTimer());
        }
    }

    IEnumerator DisableTimer()
    {
        collider.enabled = renderer.enabled = false;

        yield return new WaitForSecondsRealtime(respawnTime);

        collider.enabled = renderer.enabled = true;
    }
}
