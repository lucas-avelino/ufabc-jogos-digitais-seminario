using UnityEngine;

public class PlayerScaleController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private CameraZoomController _cameraZoomController;

    [SerializeField] private float _normalScale = 0.5f;
    [SerializeField] private float _zoomedScale = 0.25f;

    private bool _wasZoomedIn;

    private void Start()
    {
        bool isZoomedIn = _cameraZoomController.IsZoomedIn;
        ApplyScale(isZoomedIn);
        _wasZoomedIn = isZoomedIn;
    }

    private void Update()
    {
        bool isZoomedIn = _cameraZoomController.IsZoomedIn;
        if (isZoomedIn == _wasZoomedIn) return;

        ApplyScale(isZoomedIn);
        _wasZoomedIn = isZoomedIn;
    }

    private void ApplyScale(bool isZoomedIn)
    {
        float s = isZoomedIn ? _zoomedScale : _normalScale;
        _playerTransform.localScale = new Vector3(s, s, s);
    }
}
