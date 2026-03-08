using UnityEngine;

/// <summary>
/// Listens to InventoryManager events and rebuilds the item grid whenever
/// the inventory changes. Each item is represented by an InventorySlot prefab
/// instantiated inside <see cref="_container"/>.
/// </summary>
public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private Transform _container;
    [SerializeField] private InventorySlot _slotPrefab;

    void Start()
    {
        if (_inventoryManager == null)
        {
            Debug.LogError("[InventoryDisplay] InventoryManager reference is missing.");
            return;
        }

        _inventoryManager.OnInventoryChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (_inventoryManager != null)
            _inventoryManager.OnInventoryChanged -= Refresh;
    }

    private void Refresh()
    {
        print("[InventoryDisplay] Refreshing inventory display.");
        // Clear existing slots
        foreach (Transform child in _container)
            Destroy(child.gameObject);

        // Rebuild from current inventory state
        foreach (var (_, entry) in _inventoryManager.GetAllItems())
        {
            InventorySlot slot = Instantiate(_slotPrefab, _container);
            slot.Setup(entry.item, entry.quantity);
        }
    }
}
