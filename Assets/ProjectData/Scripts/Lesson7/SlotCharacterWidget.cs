using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotCharacterWidget : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    [SerializeField]
    private GameObject _emptySlot;

    [SerializeField]
    private GameObject _infoCharacterSlot;

    [SerializeField]
    private TMP_Text _nameLabel;

    [SerializeField]
    private TMP_Text _healthLabel;

    [SerializeField]
    private TMP_Text _damageLabel;

    [SerializeField]
    private TMP_Text _expLabel;

    public Button SlotButton => _button;

    public void ShowInfoCharacterSlot(string name, string health, string damage, string exp)
    {
        _nameLabel.text = name;
        _healthLabel.text = $"Health: {health}";
        _damageLabel.text = $"Damage: {damage}";
        _expLabel.text = $"Exp.: {exp}";

        _infoCharacterSlot.SetActive(true);
        _emptySlot.SetActive(false);
    }

    public void ShowEmptySlot()
    {
        _infoCharacterSlot.SetActive(false);
        _emptySlot.SetActive(true);
    }
}
