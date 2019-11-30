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
        float acceleration = Input.GetAxisRaw("Vertical");
        float turn = Input.GetAxisRaw("Horizontal");

        cb.ApplyInput(new CarInput
        {
            acceleration = acceleration,
            turn = turn
        });
    }
}
