using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//s1- Extendaing Native XR interactable
using UnityEngine.XR.Interaction.Toolkit;

//s2- changing Monobehaviour name space to  XRGrabIneractable makes it possible to append our own script to exsisting feature of XRGrab
public class XROffsetGrabInteractable : XRGrabInteractable
{
    //s5-1 -it will be safe to reset grab point to origianl if we not offset grabbing
    private Vector3 initialAttachLocalPos;
    private Quaternion initialAttachLocalRot;

    // Start is called before the first frame update
    void Start()
    {
        //s4 - create attach point to offset
        if(!attachTransform)
        {
            //check if we have an attachment point, if none we recreate a new one
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }
        //s5-2
        initialAttachLocalPos = attachTransform.localPosition;
        initialAttachLocalRot = attachTransform.localRotation;

    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {        //s3- we change the snap position of the object
        if (interactor is XRDirectInteractor)
        {
            attachTransform.position = interactor.transform.position;
            attachTransform.rotation = interactor.transform.rotation;
        }
        //s5-3
        else
        {
            Debug.Log("Ray Interactor");
            attachTransform.localPosition = initialAttachLocalPos;
            attachTransform.localRotation = initialAttachLocalRot;
        }
        
        base.OnSelectEntered(interactor);
    }
}
