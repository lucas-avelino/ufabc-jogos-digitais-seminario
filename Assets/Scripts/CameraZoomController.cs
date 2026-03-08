using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _zoomDuration = 0.7f;
    [SerializeField] private AnimationCurve _zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField] private Volume _blurVolume;
    [SerializeField] [Range(0f, 1f)] private float _maxBlurRadius = 0.6f;

    [SerializeField] private int _pixelsPerUnit = 64;

    [SerializeField] private Vector3 _overviewPosition;
    [SerializeField] private float   _overviewOrthoSize = 10f;
    [SerializeField] private bool    _allowEscapeToZoomOut      = true;
    [SerializeField] private bool    _allowRightClickToZoomOut  = true;

    private bool _isTransitioning = false;
    public bool IsTransitioning => _isTransitioning;
    private bool _isZoomedIn      = false;
    public bool IsZoomedIn => _isZoomedIn;
    private DepthOfField _dof;

    private List<Action> OnZoomEnd = new List<Action>();

    private void Start()
    {
        if (_camera == null)
            _camera = Camera.main;

        _overviewPosition  = _camera.transform.position;
        _overviewOrthoSize = _camera.orthographicSize;

        // Grab DepthOfField from the assigned Volume, if any
        if (_blurVolume != null && _blurVolume.profile != null)
        {
            _blurVolume.profile.TryGet(out _dof);
            if (_dof == null)
                Debug.LogWarning("[CameraZoomController] Volume has no DepthOfField override. Add one (Gaussian mode) to enable blur.");
        }

        SetBlur(0f);
    }

    private void Update()
    {
        if (_isTransitioning || !_isZoomedIn) return;

        if (_allowEscapeToZoomOut && Input.GetKeyDown(KeyCode.Escape))
            ZoomOut();
        if (_allowRightClickToZoomOut && Input.GetMouseButtonDown(1))
            ZoomOut();
    }

    public void ZoomToArea(Vector3 worldPosition, float orthoSize, Action onZoomEnd = null)
    {
        if (_isTransitioning) return;
        StartCoroutine(ZoomCoroutine(worldPosition, orthoSize));
        if(onZoomEnd != null)
            OnZoomEnd.Add(onZoomEnd);
    }

    public void ZoomOut()
    {
        if (_isTransitioning || !_isZoomedIn) return;
        StartCoroutine(ZoomCoroutine(_overviewPosition, _overviewOrthoSize, isZoomOut: true));
    }

    private IEnumerator ZoomCoroutine(Vector3 targetPosition, float targetOrthoSize, bool isZoomOut = false)
    {
        _isTransitioning = true;

        Vector3 fromPos  = _camera.transform.position;
        float   fromSize = _camera.orthographicSize;
        Vector3 toPos    = new Vector3(targetPosition.x, targetPosition.y, fromPos.z);

        float elapsed = 0f;
        while (elapsed < _zoomDuration)
        {
            elapsed += Time.deltaTime;
            float raw = Mathf.Clamp01(elapsed / _zoomDuration);
            float t   = _zoomCurve.Evaluate(raw);

            _camera.transform.position = Vector3.Lerp(fromPos, toPos, t);
            _camera.orthographicSize   = Mathf.Lerp(fromSize, targetOrthoSize, t);

            float blurT = 1f - Mathf.Abs(raw * 2f - 1f); // 0 → 1 → 0
            SetBlur(_maxBlurRadius * blurT);

            yield return null;
        }

        _camera.transform.position = toPos;
        _camera.orthographicSize   = SnapToPixelPerfect(targetOrthoSize);
        SetBlur(0f);

        _isZoomedIn      = !isZoomOut;
        _isTransitioning = false;
        foreach (var callback in OnZoomEnd)
        {
            callback.Invoke();
        }

        OnZoomEnd.Clear();
    }

    private void SetBlur(float radius)
    {
        if (_dof == null) return;
        _dof.active = radius > 0f;
        _dof.gaussianMaxRadius.value = radius;
    }

    // Snaps an orthographic size to the nearest pixel-perfect value.
    // Pixel-perfect: screenHeight == 2 * orthoSize * PPU * integerZoom
    private float SnapToPixelPerfect(float orthoSize)
    {
        float screenHeight = Screen.height;
        if (screenHeight <= 0) return orthoSize;
        float zoom = screenHeight / (2f * _pixelsPerUnit * orthoSize);
        float snappedZoom = Mathf.Max(1f, Mathf.Round(zoom));
        return screenHeight / (2f * _pixelsPerUnit * snappedZoom);
    }
}
