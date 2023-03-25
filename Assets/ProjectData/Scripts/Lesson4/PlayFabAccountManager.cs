using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _titleLabel;

    [SerializeField]
    private Button _forgetPlayerAccountButton;

    [SerializeField]
    private Transform _parentItems;

    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    private void Start()
    {
        _titleLabel.text = "Loading";
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccount, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogItemesSuccess, OnError);
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

    private void OnGetCatalogItemesSuccess(GetCatalogItemsResult result)
    {
        Debug.Log($"OnGetCatalogItemesSuccess");
        ShowItems(result.Catalog);
    }

    private void ShowItems(List<CatalogItem> catalog)
    {   
        foreach (var item in catalog)
        {
            _catalog.Add(item.ItemId, item);
            CreateItem(_parentItems, item.DisplayName);
            Debug.Log(item.ItemId);
        }
    }

    private void CreateItem(Transform parentItems, string nameOfItem)
    {
        var gameObject = new GameObject();
        gameObject.transform.parent = parentItems.transform;
        gameObject.name = nameOfItem;
        var textComponent = gameObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = nameOfItem;
        textComponent.color = Color.black;
        textComponent.enableWordWrapping = false;
    }

    private void OnDestroy()
    {
        _forgetPlayerAccountButton.onClick.RemoveAllListeners();
    }
}