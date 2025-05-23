using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Настройки")]
    public bool isLocked = false;
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    [Header("Ссылки")]
    public Transform doorTransform;
    public Collider2D doorBlocker;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isMoving = false;
    private Vector3 _targetPosition;

    void Start()
    {
        _closedPosition = doorTransform.position;
        _openPosition = _closedPosition + Vector3.up * moveDistance;
        
        if (isLocked)
        {
            _targetPosition = _closedPosition;
            doorTransform.position = _closedPosition;
        }
        else
        {
            _targetPosition = _openPosition;
            doorTransform.position = _openPosition;
        }
        
        doorBlocker.enabled = isLocked;
    }

    public void LockDoor()
    {
        Debug.Log($"Дверь {name} блокируется. Двигаемся к {_closedPosition}");
        isLocked = true;
        doorBlocker.enabled = true;
        _targetPosition = _closedPosition;
        _isMoving = true;
    }

    public void UnlockDoor()
    {
        Debug.Log($"Дверь {name} разблокируется. Двигаемся к {_openPosition}");
        isLocked = false;
        doorBlocker.enabled = false;
        _targetPosition = _openPosition;
        _isMoving = true;
    }

    void Update()
    {
        if (_isMoving)
        {
            doorTransform.position = Vector3.MoveTowards(
                doorTransform.position,
                _targetPosition,
                moveSpeed * Time.deltaTime
            );
            
            if (Vector3.Distance(doorTransform.position, _targetPosition) < 0.01f)
            {
                _isMoving = false;
                Debug.Log($"Дверь {name} достигла позиции: {doorTransform.position}");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (doorTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_openPosition, Vector3.one * 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_closedPosition, Vector3.one * 0.5f);
        }
    }
}
