using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector3 speed;
    public bool local = false;


    // Update is called once per frame
    void Update()
    {
        if(local)
        {
            transform.localPosition += speed * Time.deltaTime;
        }
        else
        {
            transform.position += speed * Time.deltaTime;
        }
    }
}
