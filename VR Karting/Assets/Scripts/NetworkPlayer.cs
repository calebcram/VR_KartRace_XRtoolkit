using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : MonoBehaviour
{
    public List<GameObject> avatars;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;
    private Transform rootRig;

    private GameObject spawnedAvatar;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        XRRig rig = FindObjectOfType<XRRig>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
        rootRig = rig.transform.parent;

        if(photonView.IsMine)
            photonView.RPC("LoadAvatar", RpcTarget.AllBuffered, AvatarSelector.currentSelectedAvatarID);
    }

    //Function that is responsible to load an avatar among the avatar list
    [PunRPC]
    public void LoadAvatar(int index)
    {
        if (spawnedAvatar)
            Destroy(spawnedAvatar);

        spawnedAvatar = Instantiate(avatars[index], root);
        AvatarInfo avatarInfo = spawnedAvatar.GetComponent<AvatarInfo>();

        if(avatarInfo)
        {
            avatarInfo.head.SetParent(head, false);
            avatarInfo.head.localRotation = Quaternion.identity;
            avatarInfo.head.localPosition = Vector3.zero;
           
            avatarInfo.leftHand.SetParent(leftHand, false);
            avatarInfo.leftHand.localRotation = Quaternion.identity;
            avatarInfo.leftHand.localPosition = Vector3.zero;

            avatarInfo.rightHand.SetParent(rightHand, false);
            avatarInfo.leftHand.localRotation = Quaternion.identity;
            avatarInfo.leftHand.localPosition = Vector3.zero;


            leftHandAnimator = avatarInfo.leftHandAnimator;
            rightHandAnimator = avatarInfo.rightHandAnimator;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            if(spawnedAvatar)
                spawnedAvatar.SetActive(false);
            MapPosition(root, rootRig);
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }
      
    }

    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (!handAnimator)
            return;

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }


    void MapPosition(Transform target,Transform rigTransform)
    {    
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
