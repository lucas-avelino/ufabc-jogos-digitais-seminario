using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private Player _player;
    [SerializeField] private PointOfInterestManager _pointOfInterestManager;
    [SerializeField] private CameraZoomController _cameraZoomController;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private ChatManager _chatManager;

    [Header("Zoom Areas")]
    [SerializeField] private List<MapZoomArea> _mapZoomAreas;

    [Header("Areas Items")]
    [SerializeField] private List<GameObject> _areasItemsContainer;
    private Dictionary<string, GameObject> _areasItemsContainerByName = new();

    [Header("Other")]
    [SerializeField] private Button _backButton;

    private MapZoomArea _currentZoomArea;
    public bool IsZoomedIn => _currentZoomArea != null;
    public bool ChatActive => _chatManager != null && _chatManager.gameObject.activeSelf;

    [SerializeField] private MapZoomArea _startingZoomArea;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var zoomArea in _mapZoomAreas)
        {
            zoomArea.OnClicked += () => OnAreaClicked(zoomArea);
        }

        _backButton.onClick.AddListener(OnBackButtonClicked);

        _pointOfInterestManager.OnPointOfInterestClickedEvent += OnInterestPointClicked;

        StartCoroutine(DisableItems());
        
    }

    // coroutine to set all areas items visibility to false at the start of the game
    private IEnumerator<WaitForEndOfFrame> DisableItems()
    {
        // Wait for the end of the frame to ensure all Start() methods have been called
        yield return new WaitForEndOfFrame();

        foreach (var container in _areasItemsContainer)
        {
            _areasItemsContainerByName[container.name] = container;
            container.SetActive(false);
        }
        yield return new WaitForEndOfFrame();

        if (_startingZoomArea != null)
        {
            OnAreaClicked(_startingZoomArea);
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
        
        _player.GoTo(zoomArea.PlayerTargetPos, () =>
        {
            _cameraZoomController.ZoomToArea(zoomArea.CameraTargetPos, zoomArea.TargetOrthoSize, () =>
            {
                var containerName = zoomArea.name + "Items";
                if (_areasItemsContainerByName.ContainsKey(containerName))
                {
                    _areasItemsContainerByName[containerName].SetActive(true);
                }

                if (zoomArea.Talker != null && zoomArea.Talker.HasDialog())
                {
                    _chatManager.RegisterDialog(zoomArea.Talker);
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
        _chatManager.CancelDialogs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnInterestPointClicked(PointOfInterest pointOfInterest, Action<PointOfInterest> onArrival)
    {
        if (Vector3.Distance(_player.transform.position, pointOfInterest.CameraTargetPos) < 0.1f)
        {
            onArrival?.Invoke(pointOfInterest);
            return;
        }

        _player.GoTo(pointOfInterest.CameraTargetPos, () => {
            onArrival?.Invoke(pointOfInterest);
            if (pointOfInterest.BackToMap)
            {
                OnBackButtonClicked();
            }
        });
    }

}
