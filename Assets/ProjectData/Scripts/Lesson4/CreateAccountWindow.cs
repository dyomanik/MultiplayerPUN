using PlayFab;
using PlayFab.ClientModels;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public sealed class CreateAccountWindow : AccountDataWindowBase
{
    [SerializeField]
    private InputField _emailField;

    [SerializeField]
    private Button _createAccountButton;

    private string _email;

    protected override void SubscribeUIElements()
    {
        base.SubscribeUIElements();
        _emailField.onValueChanged.AddListener(UpdateEmail);
        _createAccountButton.onClick.AddListener(CreateAccount);
    }

    private void UpdateEmail(string email)
    {
        _email = email;
    }

    private void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
        {
            Username = _username,
            Email = _email,
            Password = _password,
        }, result =>
        {
            Debug.Log($"Successful registation: {_username}");
            EnterInGameScene();
            GiveCharacterTokenToPlayer();
            GiveCharacterTokenToPlayer();
        }, error =>
        {
            Debug.Log($"Registration failed: {error.ErrorMessage}");
        });
    }

    private void GiveCharacterTokenToPlayer()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            ItemId = "character_token",
            VirtualCurrency = "GD"
        }, 
        result => Debug.Log("Success give character_token"),
        OnError);              
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    private void OnDestroy()
    {
        _emailField.onValueChanged.RemoveAllListeners();
        _createAccountButton.onClick.RemoveAllListeners();
    }
}
