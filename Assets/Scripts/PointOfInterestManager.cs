using System;
using UnityEngine;

public class PointOfInterestManager : MonoBehaviour
{
    [SerializeField] private PointOfInterest[] _pointsOfInterest; 

    public event Action<string, Vector3> OnPointOfInterestClickedEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var pointOfInterest in _pointsOfInterest)
        {
            if (pointOfInterest == null)
            {
                Debug.LogError("Point of Interest is not assigned in the inspector.");
                continue;
            }
            
            pointOfInterest.OnClicked += () => OnPointOfInterestClicked(pointOfInterest.gameObject);
        }
    }

    private void OnPointOfInterestClicked(GameObject pointOfInterest)
    {
        Debug.Log($"Point of Interest '{pointOfInterest.name}' clicked.");
        OnPointOfInterestClickedEvent?.Invoke(pointOfInterest.name, pointOfInterest.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
