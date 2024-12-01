using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 15f;        // Velocidad de movimiento
    public float zoomSpeed = 15f;       // Velocidad del zoom
    public float minZoom = 1f;         // Zoom mínimo
    public float maxZoom = 15f;        // Zoom máximo

    private Camera cam;

    void Start()
    {
        cam = Camera.main; // Obtiene la cámara principal}
        cam.orthographicSize = maxZoom;
    }

    void Update()
    {
        // Movimiento con WASD
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(new Vector3(moveX, moveY, 0));

        // Zoom con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }
}
