using System;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{

    [SerializeField] private Vector3 _cameraOffset = Vector3.zero;
    public Vector3 CameraTargetPos => transform.position + _cameraOffset;
    public event Action<PointOfInterest> OnClicked;

    [SerializeField] private Item _itemToGive;
    public Item ItemToGive => _itemToGive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }
}
