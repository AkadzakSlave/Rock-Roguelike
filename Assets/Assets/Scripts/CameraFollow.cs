using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Настройки")]
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float zoomDistance = 2f; 
    public float zoomSpeed = 3f;
    
    [Header("Границы")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 _velocity = Vector3.zero;
    private float _currentZoom = 0f;
    public static CameraFollow Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 targetPosition = target.position + offset;
        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            smoothSpeed * Time.deltaTime
        );
        HandleZoom();
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
    }

    void HandleZoom()
    {
        float targetZoom = 0f;
        
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            targetZoom = zoomDistance;
        }
        _currentZoom = Mathf.Lerp(_currentZoom, targetZoom, zoomSpeed * Time.deltaTime);
        
        Camera.main.orthographicSize = 9 + _currentZoom; 
    }
}