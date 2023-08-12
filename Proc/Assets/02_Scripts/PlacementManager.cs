
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlacementManager : MonoBehaviour {

    public static System.Action<Dictionary<Vector3, GameObject>, int, int> SimulationStarted;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private Transform cellContainer;

    private Dictionary<Vector3, GameObject> cells = new Dictionary<Vector3, GameObject>();

    [SerializeField]
    private Slider iterationSlider;
    [SerializeField]
    private TMP_Text iterationText;
    [SerializeField]
    private Vector2Int iterationRange;
    private int iterations;

    private void Start() {
        iterationSlider.minValue = iterationRange.x;
        iterationSlider.maxValue = iterationRange.y;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            PlaceCell();
        }
        if(Input.GetMouseButtonDown(1)) {
            RemoveCell();
        }

        iterations = (int)iterationSlider.value;
        iterationText.text = $"Iterations: {iterations}";

    }

    public void StartSimulation() {
        SimulationStarted?.Invoke(cells, cells.Count * 10, iterations);
        gameObject.SetActive(false);
    }

    private void PlaceCell() {

        Vector3 tilePos;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            tilePos = new Vector3(
                Mathf.Round(hit.point.x),
                Mathf.Round(hit.point.y),
                hit.point.z
            );
        }
        else {
            return;
        }

        if(!cells.ContainsKey(tilePos)) {
            GameObject c = Instantiate(cellPrefab, tilePos, Quaternion.identity, cellContainer);
            c.name += tilePos.ToString();
            cells.Add(tilePos, c);
        }
    }

    private void RemoveCell() {

        Vector3 tilePos;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            tilePos = new Vector3(
                Mathf.Round(hit.point.x),
                Mathf.Round(hit.point.y),
                hit.point.z
            );
        }
        else {
            return;
        }

        if(cells.ContainsKey(tilePos)) {
            Destroy(cells[tilePos]);
            cells.Remove(tilePos);
        }
    }
    
}