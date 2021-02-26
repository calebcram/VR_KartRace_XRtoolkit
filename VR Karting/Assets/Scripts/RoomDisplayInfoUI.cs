using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomDisplayInfoUI : MonoBehaviour
{
    public int targetRoomIndex;
    public TextMeshProUGUI currentNumberOfPlayerInfo;
    public GameObject isOpenIcon;

    private DefaultRoom targetRoom;

    // Start is called before the first frame update
    void Start()
    {
        //Access the room we want to get info from network manager using the target room name
        targetRoom = NetworkManager.singleton.defaultRooms[targetRoomIndex]; 
    }

    // Update is called once per frame
    void Update()
    {
        //update the info
        if(targetRoom != null)
        {
            currentNumberOfPlayerInfo.text = targetRoom.currentNumberOfPlayers + "/" + targetRoom.maxPLayer;
            isOpenIcon.SetActive(!NetworkManager.singleton.CanJoinRoom(targetRoom));
        }
    }
}
