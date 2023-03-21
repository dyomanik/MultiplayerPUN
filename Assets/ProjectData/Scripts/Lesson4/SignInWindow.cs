using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public sealed class SignInWindow : AccountDataWindowBase
{
    [SerializeField]
    private Button _signInButton;

    protected override void SubscribeUIElements()
    {
        base.SubscribeUIElements();
        _signInButton.onClick.AddListener(SignIn);
    }

    private void SignIn()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            Username = _username,
            Password = _password
        }, result =>
        {
            Debug.Log($"Successful signIn: {_username}");
            EnterInGameScene();
        }, error =>
        {
            Debug.Log($"SignIn failed: {error.ErrorMessage}");
        });
    }

    private void OnDestroy()
    {
        _signInButton.onClick.RemoveAllListeners();
    }
}
