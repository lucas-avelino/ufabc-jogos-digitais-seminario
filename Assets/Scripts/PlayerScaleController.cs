using System.Collections;
using UnityEngine;

public class PlayerScaleController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private CameraZoomController _cameraZoomController;

    [SerializeField] private float _normalScale = 0.5f;
    [SerializeField] private float _zoomedScale = 0.25f;

    private bool _wasZoomedIn;
    private bool _wasTransitioning;
    private Coroutine _scaleCoroutine;

    private void Start()
    {
        bool isZoomedIn = _cameraZoomController.IsZoomedIn;
        ApplyScale(isZoomedIn);
        _wasZoomedIn = isZoomedIn;
        _wasTransitioning = false;
    }

    private void Update()
    {
        bool isZoomedIn = _cameraZoomController.IsZoomedIn;
        bool isTransitioning = _cameraZoomController.IsTransitioning;

        // Detect when zoom starts transitioning
        if (isTransitioning && !_wasTransitioning)
        {
            float targetScale = isZoomedIn ? _zoomedScale : _normalScale;
            AnimateScaleWithZoom(targetScale);
        }

        _wasZoomedIn = isZoomedIn;
        _wasTransitioning = isTransitioning;
    }

    private void ApplyScale(bool isZoomedIn)
    {
        float targetScale = isZoomedIn ? _zoomedScale : _normalScale;
        _playerTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    private void AnimateScaleWithZoom(float targetScale)
    {
        if (_scaleCoroutine != null)
            StopCoroutine(_scaleCoroutine);

        _scaleCoroutine = StartCoroutine(ScaleCoroutine(targetScale));
    }

    private IEnumerator ScaleCoroutine(float targetScale)
    {
        float startScale = _playerTransform.localScale.x;
        float duration = _cameraZoomController.ZoomDuration;
        AnimationCurve curve = _cameraZoomController.ZoomCurve;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float raw = Mathf.Clamp01(elapsed / duration);
            float t = curve.Evaluate(raw);
            float currentScale = Mathf.Lerp(startScale, targetScale, t);
            _playerTransform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return null;
        }

        _playerTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }
}
