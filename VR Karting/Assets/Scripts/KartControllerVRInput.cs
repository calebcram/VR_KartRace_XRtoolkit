using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class KartControllerVRInput : MonoBehaviour
{
    public GameObject Tube001;
    public GameObject Tube002;

    [Header("Speed")]
    public float speedThreshold = 0.7f;
    public XRNode speedXrNode = XRNode.RightHand;

    [Header("Drift")]
    public float driftThreshold = 0.7f;
    public XRNode driftXRNode = XRNode.LeftHand;

    [Header("Turn")]
   
    public HingeJoint wheel;
    public float maxValue = 0.35f;
    public float minValue = -0.35f;
    public float turnThreshold = 0.2f;

    private KartController KartController;

    // Start is called before the first frame update
    void Awake()
    {
        KartController = GetComponent<KartController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Speed Input
        if(Input.GetKey(KeyCode.UpArrow) || (InputDevices.GetDeviceAtXRNode(speedXrNode).TryGetFeatureValue(CommonUsages.trigger, out float v) && v > speedThreshold))
        {
            KartController.speedInput = true;
            //Tube001.GetComponentInChildren<ParticleSystem>().Play(); //Plays particle system when trigger is pulled
            //Tube002.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            KartController.speedInput = false;
            //Tube001.GetComponentInChildren<ParticleSystem>().Stop(); //Stops particle system when trigger is released
            //Tube002.GetComponent<ParticleSystem>().Stop();
        }

        //Drift Input
        if (Input.GetKey(KeyCode.Space) || (InputDevices.GetDeviceAtXRNode(driftXRNode).TryGetFeatureValue(CommonUsages.trigger, out float g) && g > driftThreshold))
        {
            KartController.driftInput = true;
        }
        else
        {
            KartController.driftInput = false;
        }

        //Turn Input
        float steeringNormal = Mathf.InverseLerp(minValue, maxValue, wheel.transform.localRotation.x);
        float steeringRange = Mathf.Lerp(-1, 1, steeringNormal);
        if (Mathf.Abs(steeringRange) < turnThreshold)
        {
            steeringRange = 0;
        }
       
        if(steeringRange == 0)
        {
            KartController.turnInput = Input.GetAxis("Horizontal");
        }
        else
        {
            KartController.turnInput = steeringRange;
        }
        
    }
}
