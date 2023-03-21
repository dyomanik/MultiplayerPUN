using PlayFab;
using PlayFab.ClientModels;
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
        }, error =>
        {
            Debug.Log($"Registration failed: {error.ErrorMessage}");
        });
    }

    private void OnDestroy()
    {
        _emailField.onValueChanged.RemoveAllListeners();
        _createAccountButton.onClick.RemoveAllListeners();
    }
}
