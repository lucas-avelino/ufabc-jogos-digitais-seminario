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

    private Animator anim;
    private SpriteRenderer sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sprite = anim.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isWalking", _isMoving);
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
        float angle = Vector3.Angle(_targetPosition, transform.position);
        
        //Checa se a posição do target está à esquerda ou à direita do jogador
        //Se estiver à esquerda, flipa o sprite para a esquerda (a original está para a direita)
        if(_targetPosition.x < transform.position.x) sprite.flipX = true;
        else sprite.flipX = false;

        
        //Checa o ângulo entre a posição atual e o alvo. Se ele estiver entre 45° e 135°, o avatar fica de lado (isSided = true)
        //Caso contrário, ela continua de frente (isSided = false)
        if(angle < 45f || angle > 135f) anim.SetBool("isSided", false);
        else anim.SetBool("isSided", true);

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
