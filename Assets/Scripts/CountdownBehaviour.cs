using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownBehaviour : MonoBehaviour
{
    public static CountdownBehaviour Instance { get; private set; }

    public bool InputBlocked { get; private set; }

    public float beforeCountdownTime;
    public float perLightTime;
    public float holdGreenTime;
    private StartLightBehaviour[] startLights;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        startLights = transform.GetComponentsInChildren<StartLightBehaviour>();
    }

    void Start()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        InputBlocked = true;
        SetStates(StartLightBehaviour.State.Off);

        yield return new WaitForSecondsRealtime(beforeCountdownTime);

        SetStates(StartLightBehaviour.State.Red);

        yield return new WaitForSecondsRealtime(perLightTime);

        SetStates(StartLightBehaviour.State.Yellow);

        yield return new WaitForSecondsRealtime(perLightTime);

        InputBlocked = false;
        SetStates(StartLightBehaviour.State.Green);

        yield return new WaitForSecondsRealtime(holdGreenTime);

        SetStates(StartLightBehaviour.State.Off);
    }

    void SetStates(StartLightBehaviour.State state)
    {
        foreach (StartLightBehaviour startLight in startLights)
        {
            startLight.Set(state);
        }
    }
}
