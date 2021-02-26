using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DefaultRoom
{
    public string Name;
    public int sceneIndex;
    public int maxPLayer;
    public bool isOpen = true;
    public int currentNumberOfPlayers;
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public bool joinTargetRoomOnLoadedScene = true;
    public bool dontDestroyOnLoad = true;
    public List<DefaultRoom> defaultRooms;

    public UnityEvent OnJoinedLobbyEvent;
    public UnityEvent OnConnectToMasterEvent;
    public UnityEvent OnDisconnectedEvent;

    public static NetworkManager singleton;

    //The Room we are currently in
    public DefaultRoom CurrentSelectedRoom { get => currentSelectedRoom; set => currentSelectedRoom = value; }
    [HideInInspector]
    public DefaultRoom currentSelectedRoom;

    private void Awake()
    {
        //Setup Singleton
        if (singleton && singleton != this)
            Destroy(singleton.gameObject);
        else
            singleton = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(joinTargetRoomOnLoadedScene && PhotonNetwork.IsConnected)
        {
            if(CurrentSelectedRoom != null)
            {
                if(arg0.buildIndex == CurrentSelectedRoom.sceneIndex)
                {
                    InitiliazeRoom(CurrentSelectedRoom.sceneIndex);
                }
            }
        }
    }

    public void ConnectToServer()
    {
        //We connect to server, then we joined the lobby automatically
        Debug.Log("Try Connect To Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    //Check if we can join a room
    public bool CanJoinRoom(DefaultRoom roomInfo)
    {
        return roomInfo.isOpen && (roomInfo.currentNumberOfPlayers < roomInfo.maxPLayer);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Server.");
        base.OnConnectedToMaster();
        OnJoinedLobbyEvent.Invoke();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("WE JOINED THE LOBBY");
        base.OnJoinedLobby();
        OnJoinedLobbyEvent.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        OnDisconnectedEvent.Invoke();
    }

    public void SetCurrentSelectedRoom(int index)
    {
        if(!CanJoinRoom(defaultRooms[index]))
        {
            return;
        }

        if (CurrentSelectedRoom.Name == defaultRooms[index].Name)
        {
            Debug.LogWarning("Trying to initialize the room we are currently in.");
            return;
        }

        CurrentSelectedRoom = defaultRooms[index];
    }

    public void InitiliazeRoom(int defaultRoomIndex)
    {
        //CREATE THE ROOM
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)CurrentSelectedRoom.maxPLayer;
        roomOptions.IsOpen = CurrentSelectedRoom.isOpen;
        roomOptions.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(CurrentSelectedRoom.Name, roomOptions, TypedLobby.Default);
    }

    public void LoadSelectedRoomScene()
    {
        StartCoroutine(LoadSelectedRoomSceneRoutine());
    }

    public IEnumerator LoadSelectedRoomSceneRoutine()
    {
        PhotonNetwork.loadingLevelAndPausedNetwork = true;
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(CurrentSelectedRoom.sceneIndex);
        asyncOperation.allowSceneActivation = false;

        if(Fader.singleton)
        {
            Fader.singleton.FadeIn(4);
        }

        while((Fader.singleton && Fader.singleton.isFading) && !asyncOperation.isDone)
        {
            Debug.Log("Loadin Level .. " + PhotonNetwork.LevelLoadingProgress);
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
        PhotonNetwork.loadingLevelAndPausedNetwork = false;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    //Used to update the default Room Info that we have
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (var item in roomList)
        {
            DefaultRoom targetRoom = defaultRooms.Find(x => x.Name == item.Name);
            targetRoom.currentNumberOfPlayers = item.PlayerCount;
            targetRoom.isOpen = item.IsOpen;
        }
    }
}
