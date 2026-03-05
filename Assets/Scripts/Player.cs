using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpHeight = 2f;
    Vector3 _targetPosition;
    bool _isMoving = false;
    public bool IsMoving => _isMoving;
    Action _onArrivalCallback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMoving)
        {
            MoveTowardsTarget();
        }
    }

    public void GoTo(Vector3 position, Action onArrival = null)
    {
        if (_isMoving)
        {
            Debug.LogWarning("Player is already moving. Ignoring new destination.");
            return;
        }

        _targetPosition = position;
        _onArrivalCallback = onArrival;
        _isMoving = true;
    }

    public void JumpTo(GameObject floorObject)
    {
        if (_isMoving)
        {
            Debug.LogWarning("Player is already moving. Ignoring jump command.");
            return;
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (_targetPosition - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f) // Check if the player is close enough to the target position
        {
            _isMoving = false;
            _onArrivalCallback?.Invoke();
            _onArrivalCallback = null; // Clear the callback after invoking to prevent unintended calls in the future.
        }
    }
}
