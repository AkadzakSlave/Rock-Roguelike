using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Настройки")]
    public Transform target; // Цель (игрок)
    public float smoothSpeed = 5f; // Плавность слежения
    public Vector3 offset = new Vector3(0, 0, -10); // Смещение камеры
    public float zoomDistance = 2f; // Дистанция отдаления
    public float zoomSpeed = 3f; // Скорость изменения зума
    
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

        // Рассчитываем позицию с учетом смещения
        Vector3 targetPosition = target.position + offset;

        // Плавное перемещение
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            smoothSpeed * Time.deltaTime
        );
        // Динамический зум при движении
        HandleZoom();
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
    }

    void HandleZoom()
    {
        float targetZoom = 0f;
        
        // Увеличиваем зум при движении
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            targetZoom = zoomDistance;
        }

        // Плавное изменение зума
        _currentZoom = Mathf.Lerp(_currentZoom, targetZoom, zoomSpeed * Time.deltaTime);
        
        // Применяем зум
        Camera.main.orthographicSize = 9 + _currentZoom; // 5 - базовый размер
    }
}