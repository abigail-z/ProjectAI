using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLightBehaviour : MonoBehaviour
{
    private Light red;
    private Light yellow;
    private Light green;

    void Awake()
    {
        red = transform.Find("Red Light").GetComponent<Light>();
        yellow = transform.Find("Yellow Light").GetComponent<Light>();
        green = transform.Find("Green Light").GetComponent<Light>();
    }

    public void Set(State state)
    {
        switch (state)
        {
            case State.Red:
                red.enabled = true;
                yellow.enabled = false;
                green.enabled = false;
                break;
            case State.Yellow:
                red.enabled = false;
                yellow.enabled = true;
                green.enabled = false;
                break;
            case State.Green:
                red.enabled = false;
                yellow.enabled = false;
                green.enabled = true;
                break;
            case State.Off:
                red.enabled = false;
                yellow.enabled = false;
                green.enabled = false;
                break;
        }
    }

    public enum State
    {
        Red, Yellow, Green, Off
    }
}
