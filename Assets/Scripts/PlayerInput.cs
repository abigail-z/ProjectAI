using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private CarBehaviour cb;

    void Start()
    {
        cb = GetComponent<CarBehaviour>();
    }

    void Update()
    {
        // every frame, grab user input and apply it to this obj's carbehaviour
        cb.accelerationInput = Input.GetAxisRaw("Vertical");
        cb.turnInput = Input.GetAxisRaw("Horizontal");
    }
}
