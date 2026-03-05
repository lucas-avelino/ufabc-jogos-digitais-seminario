using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private Player _player;
    [SerializeField] private PointOfInterestManager _pointOfInterestManager;
    [SerializeField] private CameraZoomController _cameraZoomController;

    [Header("Zoom Areas")]
    [SerializeField] private List<MapZoomArea> _mapZoomAreas;

    [Header("Areas Items")]
    [SerializeField] private List<GameObject> _areasItemsContainer;
    private Dictionary<string, GameObject> _areasItemsContainerByName = new();

    [Header("Other")]
    [SerializeField] private Button _backButton;

    private MapZoomArea _currentZoomArea;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var zoomArea in _mapZoomAreas)
        {
            zoomArea.OnClicked += () => OnAreaClicked(zoomArea);
        }

        _backButton.onClick.AddListener(OnBackButtonClicked);

        _pointOfInterestManager.OnPointOfInterestClickedEvent += OnInterestPointClicked;

        foreach (var container in _areasItemsContainer)
        {
            _areasItemsContainerByName.Add(container.name, container);
            container.SetActive(false);
        }
    }

    void OnAreaClicked(MapZoomArea zoomArea)
    {
        if (_cameraZoomController.IsTransitioning)
        {
            Debug.Log("[GameManager] Clicked on a MapZoomArea while camera is transitioning. Ignoring.");
            return;
        }

        if (_currentZoomArea != null && _currentZoomArea != zoomArea)
        {
            Debug.Log("[GameManager] Clicked on a new MapZoomArea while another is active. Zooming out first.");
            //_cameraZoomController.ZoomOut();
            return;
        }

        if (zoomArea == _currentZoomArea)
        {
            Debug.Log("[GameManager] Clicked on the active MapZoomArea. Ignoring.");
            return;
        }

        foreach (var area in _mapZoomAreas)
        {
            area.gameObject.SetActive(false);
        }

        _backButton.gameObject.SetActive(true);
        _currentZoomArea = zoomArea;
        
        _player.GoTo(zoomArea.CameraTargetPos, () =>
        {
            _cameraZoomController.ZoomToArea(zoomArea.CameraTargetPos, zoomArea.TargetOrthoSize, () =>
            {
                var containerName = zoomArea.name + "Items";
                if (_areasItemsContainerByName.ContainsKey(containerName))
                {
                    _areasItemsContainerByName[containerName].SetActive(true);
                }
            });
        });
    }

    void OnBackButtonClicked()
    {
        if (_cameraZoomController.IsTransitioning)
        {
            Debug.Log("[GameManager] Clicked on a MapZoomArea while camera is transitioning. Ignoring.");
            return;
        }

        if (_currentZoomArea == null)
        {
            Debug.LogWarning("[GameManager] Back button clicked but no active MapZoomArea. Ignoring.");
            return;
        }

        if (_player.IsMoving)
        {
            Debug.Log("[GameManager] Back button clicked while player is moving. Ignoring.");
            return;
        }

        _backButton.gameObject.SetActive(false);
        foreach (var area in _mapZoomAreas)
        {
            area.gameObject.SetActive(true);
        }
        foreach (var container in _areasItemsContainer)
        {
            container.SetActive(false);
        }
        _currentZoomArea = null;
        _cameraZoomController.ZoomOut();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnInterestPointClicked(string pointName, Vector3 pointPosition)
    {
        _player.GoTo(pointPosition, () => {});
    }


}
