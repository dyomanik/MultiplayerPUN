using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogIn : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _logInButton;
    [SerializeField] private Button _connectToPhotonButton;
    [SerializeField] private TextMeshProUGUI _statusLogin;
    
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "A2891";
        }
        _logInButton.onClick.AddListener(LogInToPlayFab);
        _connectToPhotonButton.onClick.AddListener(ConnectToPhoton);
    }

    private void LogInToPlayFab()
    {
        var request = new LoginWithCustomIDRequest()
        {
            CustomId = "TestUser",
            CreateAccount = true,
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        _logInButton.interactable = false;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        var resultMessage = result.Request.ToString();
        _statusLogin.text = "Successful login";
        _statusLogin.color = Color.green;
        Debug.Log(resultMessage);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        _statusLogin.text = errorMessage;
        _statusLogin.color = Color.red;
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    private void ConnectToPhoton()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        }
        _connectToPhotonButton.onClick.RemoveListener(ConnectToPhoton);
        _connectToPhotonButton.onClick.AddListener(DisconnectFromPhoton);
        _connectToPhotonButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Disconnect\nfrom\nPhoton";
    }

    private void DisconnectFromPhoton()
    {
        PhotonNetwork.Disconnect();
        _connectToPhotonButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Connect\nto\nPhoton";
        _connectToPhotonButton.onClick.RemoveListener(DisconnectFromPhoton);
        _connectToPhotonButton.onClick.AddListener(ConnectToPhoton);
    }

    private void OnDestroy()
    {
        _logInButton.onClick.RemoveAllListeners();
        _connectToPhotonButton.onClick.RemoveAllListeners();
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
    }
}
