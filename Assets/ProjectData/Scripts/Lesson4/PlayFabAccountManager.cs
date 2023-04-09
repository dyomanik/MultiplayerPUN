using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.Text;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _titleLabel;

    [SerializeField]
    private Button _forgetPlayerAccountButton;

    [SerializeField]
    private Transform _parentItems;

    [SerializeField]
    private GameObject _newCharacterCreatePanel;

    [SerializeField]
    private Button _createCharacterButton;

    [SerializeField]
    private TMP_InputField _inputField;

    [SerializeField]
    private List<SlotCharacterWidget> _slots;

    private string _characterName;

    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    private void Start()
    {
        _titleLabel.text = "Loading";
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccount, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogItemesSuccess, OnError);
        
        _forgetPlayerAccountButton.onClick.AddListener(ForgetPlayerAccount);

        GetCharacters();

        foreach (var slot in _slots)
        {
            slot.SlotButton.onClick.AddListener(OpenCreateNewCharacterPanel);
        }

        _inputField.onValueChanged.AddListener(OnNameChanged);
        _createCharacterButton.onClick.AddListener(CreateCharacter);
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

    private void GetCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            result =>
            {
                Debug.Log($"Character count {result.Characters.Count}");;
                if (result.Characters.Count < 2)
                {
                    GiveCharacterTokenToPlayer();
                }
                ShowCharectersInSlot(result.Characters);
            },
            OnError);
    }

    private void ShowCharectersInSlot(List<CharacterResult> characters)
    {
        foreach (var slot in _slots)
        {
            slot.gameObject.SetActive(false);
        }

        if (characters.Count == 0)
        {
            _slots[0].gameObject.SetActive(true);
            _slots[0].ShowEmptySlot();
        }
        else if (characters.Count > 0 && characters.Count < _slots.Count)
        {
            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(true);
            }
            
            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
            {
                CharacterId = characters[0].CharacterId,
            },
            result =>
            {
                var health = result.CharacterStatistics["Health"].ToString();
                var damage = result.CharacterStatistics["Damage"].ToString();
                var exp = result.CharacterStatistics["Exp"].ToString();
                _slots[0].ShowInfoCharacterSlot(characters[0].CharacterName, health, damage, exp);
                _slots[1].ShowEmptySlot();
            },
            OnError);
        }
        else
        {
            for (var i = 0; i < (characters.Count); i++)
            {
                var counter = i;
                PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
                {
                    CharacterId = characters[counter].CharacterId,
                },
                result =>
                {
                    _slots[counter].gameObject.SetActive(true);
                    var health = result.CharacterStatistics["Health"].ToString();
                    var damage = result.CharacterStatistics["Damage"].ToString();
                    var exp = result.CharacterStatistics["Exp"].ToString();
                    _slots[counter].ShowInfoCharacterSlot(characters[counter].CharacterName, health, damage, exp);
                },
                OnError);
            }
        }
    }

    private void OpenCreateNewCharacterPanel()
    {
        _newCharacterCreatePanel.gameObject.SetActive(true);
    }

    private void CloseCreateNewCharacterPanel()
    {
        _newCharacterCreatePanel.gameObject.SetActive(false);
    }

    private void OnNameChanged(string changedName)
    {
        _characterName = changedName;
    }

    private void CreateCharacter()
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = _characterName,
            ItemId = "character_token"
        },
        result =>
        {
            UpdateCharacterStatistics(result.CharacterId);
        },
        OnError);
    }

    private void GiveCharacterTokenToPlayer()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                int tokenCounter = 0;
                foreach (var item in result.Inventory)
                {
                    if (item.ItemId == "character_token")
                    {
                        tokenCounter += 1;
                    }
                }
                
                if (tokenCounter < 3)
                {
                    PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
                    {
                        ItemId = "character_token",
                        VirtualCurrency = "GD"
                    }, result => Debug.Log("Success give character_token"),
                OnError);
                }
            },
            OnError);
    }

    private void UpdateCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Health", 100},
                {"Damage", 20},
                {"Exp", 0}
            }
        },
        result =>
        {
            Debug.Log($"Completed Update CharacterStatistics");
            CloseCreateNewCharacterPanel();
            GetCharacters();
        },
        OnError);
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