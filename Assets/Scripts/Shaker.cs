using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    float shakeDuration;
    float maxShakeDistance;
    bool isShaking;
    Vector3 initialPosition;
    float timer;

    public bool IsShaking { get { return isShaking; } }

    void Update()
    {
        if (isShaking)
        {
            timer += Time.deltaTime;
            if (timer >= shakeDuration)
            {
                isShaking = false;
                transform.position = initialPosition;
                return;
            }
            transform.position = initialPosition + Random.insideUnitSphere * maxShakeDistance;
        }
    }

    public void StartShake(float duration, float maxDistance)
    {
        if (isShaking)
        {
            Debug.LogWarning("Shake is already running on this object!");
        }
        initialPosition = transform.position;
        shakeDuration = duration;
        maxShakeDistance = maxDistance;
        timer = 0f;
        isShaking = true;
    }
}

