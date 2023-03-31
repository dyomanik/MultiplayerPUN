using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ConnectAndJoinRandomLB : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
{
    [SerializeField]
    private ServerSettings _serverSettings;
    [SerializeField]
    private TMP_Text _stateUIText;
    private LoadBalancingClient _loadBalancingClient;
    private const string GAME_MODE_KEY = "gm";
    private const string AI_MODE_KEY = "ai";

    private const string MAP_PROP_KEY = "C0";
    private const string GOLD_PROP_KEY = "C1";
    private TypedLobby _sqlLobby = new TypedLobby("customSQLLobby", LobbyType.SqlLobby);

    private void Start()
    {
        _loadBalancingClient = new LoadBalancingClient();
        _loadBalancingClient.AddCallbackTarget(this);

        _loadBalancingClient.ConnectUsingSettings(_serverSettings.AppSettings);
    }

    private void OnDestroy()
    {
        _loadBalancingClient.RemoveCallbackTarget(this);
    }

    private void Update()
    {
        if (_loadBalancingClient == null)
        {
            return;
        }
        _loadBalancingClient.Service();

        var state = _loadBalancingClient.State.ToString();
        _stateUIText.text = state;
    }
    public void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        _loadBalancingClient.OpJoinRandomRoom();
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        throw new System.NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected" + cause.ToString());
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinedLobby()
    {
        var sqlLobby = $"{MAP_PROP_KEY} = Map3 AND {GOLD_PROP_KEY} BETWEEN 300 AND 500";
        var opJoinRandomRoomParams = new OpJoinRandomRoomParams
        {
            SqlLobbyFilter = sqlLobby
        };
        
        _loadBalancingClient.OpJoinRandomRoom(opJoinRandomRoomParams);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        _loadBalancingClient.CurrentRoom.IsOpen = false;
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        var roomOptions = new RoomOptions 
        { 
            MaxPlayers = 12,
            PublishUserId = true,
            CustomRoomPropertiesForLobby = new[] {MAP_PROP_KEY, GOLD_PROP_KEY},
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { GOLD_PROP_KEY, 400}, { MAP_PROP_KEY, "Map3"} }
        };
        var enterRoomParams = new EnterRoomParams
        {
            RoomOptions = roomOptions,
            RoomName = "NewRoom",
            ExpectedUsers = new[] { "132456" },
            Lobby = _sqlLobby
        };
        _loadBalancingClient.OpCreateRoom(enterRoomParams);
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnLeftLobby()
    {
        throw new System.NotImplementedException();
    }

    public void OnLeftRoom()
    {
        throw new System.NotImplementedException();
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        throw new System.NotImplementedException();
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionListReceived");
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        throw new System.NotImplementedException();
    }



}
