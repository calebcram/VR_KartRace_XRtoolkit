using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OnSelectMakeHandFollow : MonoBehaviour
{
    public Transform newParent;
    private XRBaseInteractable interactable;
    private HandPresence hand;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.onSelectEntered.AddListener(StartHandFollow);
        interactable.onSelectExited.AddListener(FinishHandFollow);
    }

    void StartHandFollow(XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor && !hand)
        {
            hand = interactor.GetComponentInChildren<HandPresence>();
            if(hand)
                hand.transform.SetParent(newParent, true);
        }
    }

    void FinishHandFollow(XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor)
        {
            if(hand)
            {

                hand.transform.SetParent(interactor.transform, true);
                hand.transform.localPosition = Vector3.zero;
                hand.transform.localRotation = Quaternion.identity;
                hand = null;
            }
        }
    }
}
