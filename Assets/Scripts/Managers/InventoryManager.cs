using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private PointOfInterestManager _pointOfInterestManager;
    private readonly Dictionary<string, (Item item, int quantity)> _items = new();
    /// <summary>Fired after an item is added, passing the item and the amount added.</summary>
    public event Action<Item, int> OnItemAdded;

    /// <summary>Fired after an item is removed, passing the item and the amount removed.</summary>
    public event Action<Item, int> OnItemRemoved;

    /// <summary>Fired whenever the inventory changes (add or remove).</summary>
    public event Action OnInventoryChanged;

    /// <summary>Adds <paramref name="quantity"/> of <paramref name="item"/> to the inventory.</summary>
    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventoryManager] Tried to add a null item.");
            return;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning($"[InventoryManager] Tried to add '{item.DisplayName}' with invalid quantity {quantity}.");
            return;
        }

        if (_items.ContainsKey(item.Id))
            _items[item.Id] = (item, _items[item.Id].quantity + quantity);
        else
            _items[item.Id] = (item, quantity);

        Debug.Log($"[InventoryManager] Added {quantity}x '{item.DisplayName}'. Total: {_items[item.Id].quantity}");
        OnItemAdded?.Invoke(item, quantity);
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Removes <paramref name="quantity"/> of <paramref name="item"/> from the inventory.
    /// Returns <c>true</c> on success, <c>false</c> if the item or enough quantity is not present.
    /// </summary>
    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (item == null || !_items.ContainsKey(item.Id))
        {
            Debug.LogWarning($"[InventoryManager] Tried to remove item '{item?.DisplayName}' that is not in the inventory.");
            return false;
        }

        int current = _items[item.Id].quantity;
        if (current < quantity)
        {
            Debug.LogWarning($"[InventoryManager] Not enough '{item.DisplayName}' to remove. Has {current}, tried to remove {quantity}.");
            return false;
        }

        int remaining = current - quantity;
        if (remaining == 0)
            _items.Remove(item.Id);
        else
            _items[item.Id] = (item, remaining);

        Debug.Log($"[InventoryManager] Removed {quantity}x '{item.DisplayName}'. Remaining: {remaining}");
        OnItemRemoved?.Invoke(item, quantity);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (itemId == null || !_items.ContainsKey(itemId))
        {
            Debug.LogWarning($"[InventoryManager] Tried to remove item with ID '{itemId}' that is not in the inventory.");
            return false;
        }

        var item = _items[itemId].item;
        return RemoveItem(item, quantity);
    }

    /// <summary>Returns <c>true</c> if the inventory contains at least <paramref name="quantity"/> of <paramref name="item"/>.</summary>
    public bool HasItem(Item item, int quantity = 1)
    {
        return item != null
            && _items.TryGetValue(item.Id, out var entry)
            && entry.quantity >= quantity;
    }

    public bool HasItem(string itemId, int quantity = 1)
    {
        return itemId != null
            && _items.TryGetValue(itemId, out var entry)
            && entry.quantity >= quantity;
    }

    /// <summary>Returns the current stack count for <paramref name="item"/>, or 0 if not present.</summary>
    public int GetItemCount(Item item)
    {
        if (item == null || !_items.TryGetValue(item.Id, out var entry))
            return 0;

        return entry.quantity;
    }

    /// <summary>Returns a read-only view of all items currently in the inventory.</summary>
    public IReadOnlyDictionary<string, (Item item, int quantity)> GetAllItems() => _items;
}

