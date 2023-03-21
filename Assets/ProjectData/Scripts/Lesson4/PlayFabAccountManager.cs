using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _titleLabel;

    [SerializeField]
    private Button _forgetPlayerAccountButton;

    private void Start()
    {
        _titleLabel.text = "Loading";
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccount, OnError);
        _forgetPlayerAccountButton.onClick.AddListener(ForgetPlayerAccount);
    }

    private void OnGetAccount(GetAccountInfoResult result)
    {
        _titleLabel.text = $"PlayFab ID: {result.AccountInfo.PlayFabId}" +
            $"\nUsername: {result.AccountInfo.Username}" +
            $"\nAccount was created: {result.AccountInfo.Created}";
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    private void ForgetPlayerAccount()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        _forgetPlayerAccountButton.onClick.RemoveAllListeners();
    }
}