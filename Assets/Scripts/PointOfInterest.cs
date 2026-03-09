using System;
using Unity.VisualScripting;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{

    [SerializeField] private Vector3 _cameraOffset = Vector3.zero;
    public Vector3 CameraTargetPos => transform.position + _cameraOffset;
    public event Action<PointOfInterest> OnClicked;

    [SerializeField] private Item _itemToGive;
    public Item ItemToGive => _itemToGive;

    [SerializeField] private Talker _talker;
    public Talker Talker => _talker;

    [SerializeField] private bool _backToMap;
    public bool BackToMap => _backToMap;

    [SerializeField] private bool _isClickableOutsideZoom = false;
    public bool IsClickableOutsideZoom => _isClickableOutsideZoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }
}
