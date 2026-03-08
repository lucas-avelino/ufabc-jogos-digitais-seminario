using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the item-slot prefab used by InventoryDisplay.
/// Assign an Image (icon) and an optional TextMeshProUGUI (quantity) in the inspector.
/// </summary>
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _quantityText;

    public void Setup(Item item, int quantity)
    {
        _icon.sprite = item.Icon;
        _icon.enabled = item.Icon != null;

        if (_quantityText != null)
            _quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
    }
}
