using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    public string targetTag = "Player";
    public float targetSpeed;
    public float thiscountDown = 3;
    public static float countDown = 3;
    private float timer;
    public static bool isBoosting = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(targetTag))
        {
            if (isBoosting)
            {
                countDown = thiscountDown;
                timer = 0;
            }
            else
            {
                StartCoroutine(SpeedPadRoutine(other.transform.parent.gameObject.GetComponentInChildren<KartController>()));
            }
        }
    }

    IEnumerator SpeedPadRoutine(KartController kart)
    {
        isBoosting = true;
        float initialSpeed = kart.acceleration;
        kart.acceleration = targetSpeed;
        countDown = thiscountDown;
        while(timer < countDown)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        kart.acceleration = initialSpeed;
        isBoosting = false;
        timer = 0;
    }
}
