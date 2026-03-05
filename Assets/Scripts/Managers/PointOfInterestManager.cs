using System;
using UnityEngine;

public class PointOfInterestManager : MonoBehaviour
{
    [SerializeField] private PointOfInterest[] _pointsOfInterest;
    [SerializeField] private GameObject _world;

    public event Action<string, Vector3> OnPointOfInterestClickedEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pointsOfInterest = _world.GetComponentsInChildren<PointOfInterest>();

        foreach (var pointOfInterest in _pointsOfInterest)
        {
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
        Debug.Log($"Point of Interest '{pointOfInterest.name}' clicked.");
        OnPointOfInterestClickedEvent?.Invoke(pointOfInterest.name, pointOfInterest.CameraTargetPos);
    }
}
