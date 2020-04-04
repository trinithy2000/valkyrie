using System;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    public float targetTime = 0.0f;
    private Action triggerEnd;

    public void Init(float time, Action f)
    {
        targetTime = time;
        triggerEnd = f;
    }

    private void Update()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            if (triggerEnd != null)
            {
                triggerEnd();
            }
        }
    }
}

