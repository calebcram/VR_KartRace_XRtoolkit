using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class VRKartInput : BaseInput
{
    public HingeJoint wheel;
    public XRNode acceleratorInputNode;
    public XRNode deceleratorInputNode;
    public float maxValue = 0.35f;
    public float minValue = -0.35f;
    public float turnThreshold = 0.2f;
    public TMPro.TextMeshPro text;


    public override Vector2 GenerateInput()
    {
        float steeringNormal = Mathf.InverseLerp(minValue, maxValue, wheel.transform.localRotation.x);
        float steeringRange = Mathf.Lerp(-1, 1, steeringNormal);

        float accelerator = 0;
        float decelerator = 0;

        if (InputDevices.GetDeviceAtXRNode(acceleratorInputNode).TryGetFeatureValue(CommonUsages.trigger, out float v))
        {
            accelerator = v;
        }

        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, devices);

        if(devices.Count > 0)
        {
            if (devices[0].TryGetFeatureValue(CommonUsages.trigger, out float d))
            {
                decelerator = -d;
            }
        }
        
        if(Mathf.Abs(steeringRange) < turnThreshold)
        {
            steeringRange = 0;
        }

        text.text = (-steeringRange).ToString("0.00");

        return new Vector2(-steeringRange, accelerator + decelerator);
    }
}