
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Camera mainCamera;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float zoomSpeed;

    [SerializeField]
    private float minZoomAmount;
    [SerializeField]
    private float maxZoomAmount;

    private void Start() {
        mainCamera = GetComponent<Camera>();
    }

    private void Update() {
        Move();
        Zoom();
    }

    private void Move() {

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        
        float moveModifier = mainCamera.orthographicSize * (0.6f + 0.4f * (1/(mainCamera.orthographicSize/2)));

        Vector3 move = new Vector3(hor, ver, 0.0f);

        transform.position += move * moveSpeed * moveModifier * Time.deltaTime;

    }

    private void Zoom() {

        float scrollDelta = Input.mouseScrollDelta.y;

        if(scrollDelta < 0.0f && mainCamera.orthographicSize <= maxZoomAmount) {
            mainCamera.orthographicSize += zoomSpeed * Time.deltaTime;
        }
        if(scrollDelta > 0.0f && mainCamera.orthographicSize >= minZoomAmount) {
            mainCamera.orthographicSize -= zoomSpeed * Time.deltaTime;
        }

        if(mainCamera.orthographicSize > maxZoomAmount) {
            mainCamera.orthographicSize = maxZoomAmount;
        }
        if(mainCamera.orthographicSize < minZoomAmount) {
            mainCamera.orthographicSize = minZoomAmount;
        }

    }
    
}