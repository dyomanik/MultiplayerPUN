using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Matchmaking : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private ServerSettings _serverSettings;
    [SerializeField]
    private TMP_Text _stateUIText;

    [SerializeField]
    private GameObject _roomParent;

    [SerializeField]
    private Button _createRoom;

    [SerializeField]
    private Button _joinRoom;

    private TypedLobby _myLobby = new TypedLobby("myLobby", LobbyType.Default);
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        _createRoom.onClick.AddListener(CreateRoom);
        _joinRoom.onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom("newRoom");
    }

    private void OnDestroy()
    {
        _createRoom.onClick.RemoveAllListeners();
        _joinRoom.onClick.RemoveAllListeners();
    }
    private void CreateRoom()
    {
        PhotonNetwork.CreateRoom("newRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    private void Update()
    {
        _stateUIText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby(_myLobby);
    }
   
    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("roomListUpdated");
        UpdateCachedRoomList(roomList);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        if (roomList.Count > 0)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                var info = roomList[i];
                if (info.RemovedFromList)
                {
                    cachedRoomList.Remove(info.Name);
                }
                else
                {
                    cachedRoomList[info.Name] = info;
                    var roomObject = new GameObject(info.Name);
                    roomObject.transform.parent = _roomParent.transform;
                    roomObject.AddComponent<Button>();
                }
            }
            _roomParent.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.CountOfRooms.ToString();
        }
        else Debug.Log("No rooms");
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
}
