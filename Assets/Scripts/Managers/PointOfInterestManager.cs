using System;
using UnityEngine;

public class PointOfInterestManager : MonoBehaviour
{
    [SerializeField] private PointOfInterest[] _pointsOfInterest;
    [SerializeField] private GameObject _world;

    [SerializeField] private ChatManager _chatManager;
    [SerializeField] private InventoryManager _inventoryManager;

    public event Action<PointOfInterest, Action<PointOfInterest>> OnPointOfInterestClickedEvent;
    private event Action<PointOfInterest> OnArrivedAtPointOfInterestEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pointsOfInterest = _world.GetComponentsInChildren<PointOfInterest>(true);
        Debug.Log($"Found {_pointsOfInterest.Length} Points of Interest in the world.");
        foreach (var pointOfInterest in _pointsOfInterest)
        {
            Debug.Log($"Registering Point of Interest '{pointOfInterest.name}' with the PointOfInterestManager.");
            if (pointOfInterest == null)
            {
                Debug.LogError("Point of Interest is not assigned in the inspector.");
                continue;
            }
            
            pointOfInterest.OnClicked += OnPointOfInterestClicked;
        }
    }

    private void OnPointOfInterestClicked(PointOfInterest pointOfInterest)
    {
        OnPointOfInterestClickedEvent?.Invoke(pointOfInterest, OnArrivedAtPointOfInterest);
    }

    private void OnArrivedAtPointOfInterest(PointOfInterest pointOfInterest)
    {
        if (pointOfInterest.ItemToGive != null)
        {
            _inventoryManager.AddItem(pointOfInterest.ItemToGive);
            pointOfInterest.gameObject.SetActive(false);
        }

        if (pointOfInterest.Talker != null)
        {
            _chatManager.RegisterDialog(pointOfInterest.Talker.GetCurrentDialog());
        }

        OnArrivedAtPointOfInterestEvent?.Invoke(pointOfInterest);
    }
}
