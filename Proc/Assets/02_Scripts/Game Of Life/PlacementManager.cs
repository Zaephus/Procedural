
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour {

    public static Action<Dictionary<Vector3, GameObject>, int> SimulationStarted;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private Transform cellContainer;

    private Dictionary<Vector3, GameObject> cells = new Dictionary<Vector3, GameObject>();

    private bool canPlaceCells = true;

    private void Start() {}

    private void Update() {
        if(canPlaceCells) {
            if(Input.GetMouseButtonDown(0)) {
                PlaceCell();
            }

            if(Input.GetMouseButtonDown(1)) {
                RemoveCell();
            }

            if(Input.GetKeyDown(KeyCode.Return)) {
                StartSimulation();
            }
        }
    }

    private void StartSimulation() {
        SimulationStarted?.Invoke(cells, cells.Count * 10);
        canPlaceCells = false;
    }

    private void PlaceCell() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 tilePos = new Vector3(
            Mathf.Round(mousePos.x),
            Mathf.Round(mousePos.y),
            0.0f
        );

        if(!cells.ContainsKey(tilePos)) {
            GameObject c = Instantiate(cellPrefab, tilePos, Quaternion.identity, cellContainer);
            cells.Add(tilePos, c);
        }
    }

    private void RemoveCell() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 tilePos = new Vector3(
            Mathf.Round(mousePos.x),
            Mathf.Round(mousePos.y),
            0.0f
        );

        if(cells.ContainsKey(tilePos)) {
            Destroy(cells[tilePos]);
            cells.Remove(tilePos);
        }
    }
    
}