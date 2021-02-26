using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class NetworkStartPosition
{
    public Transform startPosition;
    public string occupiedPlayerID;

    public NetworkStartPosition(Transform _startPosition)
    {
        startPosition = _startPosition;
        occupiedPlayerID = "";
    }
}

public class NetworkStartPositions : MonoBehaviourPunCallbacks
{
    public List<Transform> startPositions;
    public Transform player;

    private List<NetworkStartPosition> networkStartPositions;

    private void Awake()
    {
        Initialize();
    }

    public override void OnJoinedRoom()
    {
        SetPlayerToStartPosition();
    }

    //Used to set the player to the start position that is currently free in the network
    public void SetPlayerToStartPosition()
    {
        int id = PhotonNetwork.LocalPlayer.ActorNumber-1;
        int availablePositionIndex = (id == -1) ? 0 : id % networkStartPositions.Count;
        TakePosition(availablePositionIndex);

        Transform startPositionTransform = networkStartPositions[availablePositionIndex].startPosition;
        player.transform.position = startPositionTransform.position;
        player.transform.rotation = startPositionTransform.rotation;
    }

    //Initialize the network start positions based on start positions list
    public void Initialize()
    {
        networkStartPositions = new List<NetworkStartPosition>();

        foreach (var item in startPositions)
        {
            networkStartPositions.Add(new NetworkStartPosition(item));
        }
    }


    //USE LocalPlayer Actor Number instead its easier to say the next spawn point !
    /*
    //Browse the start positions list to return the first one that is not occupied by a player
    public int CheckAvailablePosition()
    {
        for (int i = 0; i < networkStartPositions.Count; i++)
        {
            NetworkStartPosition item = networkStartPositions[i];
            if (item.occupiedPlayerID == "")
            {
                Debug.Log("Start Position Found Room at " + i);
                return i;
            }
        }

        Debug.LogWarning("No Free Position Found, Setting Position to last in the list...");
        return networkStartPositions.Count - 1;
    }*/

    //Used to take a position and say it to all other clients
    public void TakePosition(int index)
    {
        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        photonView.RPC("TakePositionRPC", RpcTarget.AllBuffered, index, PhotonNetwork.LocalPlayer.UserId);
    }

    //Set the network occupied player ID using index and userID info sent by network
    [PunRPC]
    public void TakePositionRPC(int index,string userID)
    {
        Debug.Log("TAKING POSITION " + index + " for user " + userID);
        networkStartPositions[index].occupiedPlayerID = userID;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int removeIndex = networkStartPositions.FindIndex(x => x.occupiedPlayerID == otherPlayer.UserId);
        if(removeIndex >= 0)
        {
            networkStartPositions[removeIndex].occupiedPlayerID = "";
        }
    }
}
