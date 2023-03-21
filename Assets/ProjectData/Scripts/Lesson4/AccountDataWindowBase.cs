using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountDataWindowBase : MonoBehaviour
{
    [SerializeField]
    private InputField _usernameField;

    [SerializeField] 
    private InputField _passwordField;

    protected string _username;
    protected string _password;

    private void Start()
    {
        SubscribeUIElements();
    }

    protected virtual void SubscribeUIElements()
    {
        _usernameField.onValueChanged.AddListener(UpdateUsername);
        _passwordField.onValueChanged.AddListener(UpdatePassword);
    }
    private void UpdateUsername(string username)
    {
        _username = username;
    }

    private void UpdatePassword(string password)
    {
        _password = password;
    }

    private void OnDestroy()
    {
        _usernameField.onValueChanged.RemoveAllListeners();
        _passwordField.onValueChanged.RemoveAllListeners();
    }

    protected void EnterInGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
