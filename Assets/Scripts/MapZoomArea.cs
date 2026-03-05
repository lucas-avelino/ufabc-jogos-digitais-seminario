using System;
using UnityEngine;

public class MapZoomArea : MonoBehaviour
{
    [SerializeField] private float _targetOrthoSize = 3f;
    public float TargetOrthoSize => _targetOrthoSize;

    [SerializeField] private Vector3 _cameraOffset = Vector3.zero;
    public Vector3 CameraTargetPos => transform.position + _cameraOffset;

    [SerializeField] private bool _showGizmo = true;

    [SerializeField] public bool _isZoomed = true;

    public event Action OnClicked;

    private void OnMouseDown()
    {
      OnClicked?.Invoke();
    }
    
}
