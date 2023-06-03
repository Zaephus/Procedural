
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour {

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private Transform cellContainer;

    private List<GameObject> cells = new List<GameObject>();

    private void Start() {
        
    }

    private void Update() {

        if(Input.GetMouseButtonDown(0)) {
            PlaceCell();
        }
        
    }

    private void PlaceCell() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 tilePos = new Vector3(
            Mathf.Round(mousePos.x),
            Mathf.Round(mousePos.y),
            0.0f
        );
        GameObject c = Instantiate(cellPrefab, tilePos, Quaternion.identity, cellContainer);
        cells.Add(c);
    }
    
}