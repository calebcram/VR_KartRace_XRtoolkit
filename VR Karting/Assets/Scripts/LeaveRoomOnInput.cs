using UnityEngine.XR;
using Photon.Pun;

public class LeaveRoomOnInput : MonoBehaviourPunCallbacks
{
    public InputHelpers.Button inputHelpers = InputHelpers.Button.MenuButton;
    public XRNode controller = XRNode.LeftHand;
    
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(controller), inputHelpers, out bool isPressed);

        if (isPressed)
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel(0);
        }
    }
}
