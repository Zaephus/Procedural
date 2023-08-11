
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour {

    private Dictionary<Vector3, GameObject> cells = new Dictionary<Vector3, GameObject>();
    private Queue<GameObject> cellPool = new Queue<GameObject>();

    [SerializeField]
    private Vector2Int fieldSize;

    [SerializeField]
    private GameObject cellPrefab;

    private int poolAmount;
    [SerializeField]
    private Transform pool;

    [SerializeField]
    private Transform cellContainer;
    
    [SerializeField]
    private float tickTime = 0.2f;

    [SerializeField]
    private bool isSimulating = false;

    private void Start() {
        PlacementManager.SimulationStarted += StartSimulation;
    }

    private void Update() {

        if(Input.GetKeyDown(KeyCode.Escape)) {
            isSimulating = false;
        }

        if(cellPool.Count < poolAmount*0.2f) {
            AddCellToPool();
        }
        
    }

    private void StartSimulation(Dictionary<Vector3, GameObject> _cells, int _poolAmount) {

        cells = _cells;

        poolAmount = _poolAmount;
        for(int i = 0; i < _poolAmount; i++) {
            AddCellToPool();
        }

        StartCoroutine(Simulate());
        PlacementManager.SimulationStarted -= StartSimulation;
        
    }

    private IEnumerator Simulate() {

        isSimulating = true;
        
        while(isSimulating) {
            CalculateCycle();
            yield return new WaitForSeconds(tickTime);
        }

    }

    private void CalculateCycle() {

        List<Vector3> emptyNeighbours = new List<Vector3>();

        Dictionary<Vector3, GameObject> cellBuffer = new Dictionary<Vector3, GameObject>();
        foreach(KeyValuePair<Vector3, GameObject> kvp in cells) {
            cellBuffer.Add(kvp.Key, kvp.Value);
        }

        foreach(KeyValuePair<Vector3, GameObject> kvp in cellBuffer) {
            
            int neighbourCount = 0;

            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {

                    if(x == 0 && y == 0) {
                        continue;
                    }

                    Vector3 pos = kvp.Key + new Vector3(x, y, 0.0f);

                    if(cellBuffer.ContainsKey(pos)) {
                        neighbourCount++;
                    }
                    else {
                        if(!emptyNeighbours.Contains(pos)) {
                            emptyNeighbours.Add(pos);
                        }
                    }

                }
            }

            if(neighbourCount < 2 || neighbourCount > 3) {
                GameObject c = kvp.Value;
                cells.Remove(c.transform.position);
                c.transform.position = pool.position;
                c.GetComponent<SpriteRenderer>().enabled = false;
                c.transform.parent = pool;
                cellPool.Enqueue(c);
            }

        }

        for(int i = 0; i < emptyNeighbours.Count; i++) {

            int neighbourCount = 0;

            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {

                    if(x == 0 && y == 0) {
                        continue;
                    }

                    Vector3 pos = emptyNeighbours[i] + new Vector3(x, y, 0.0f);

                    if(cellBuffer.ContainsKey(pos)) {
                        neighbourCount++;
                    }

                }
            }

            if(neighbourCount == 3) {
                GameObject c = cellPool.Dequeue();
                c.transform.position = emptyNeighbours[i];
                c.transform.parent = cellContainer;
                c.GetComponent<SpriteRenderer>().enabled = true;
                cells.Add(emptyNeighbours[i], c);
            }

        }

    }

    private void AddCellToPool() {
        GameObject poolCell = Instantiate(cellPrefab, pool.position, Quaternion.identity, pool);
        poolCell.GetComponent<SpriteRenderer>().enabled = false;
        cellPool.Enqueue(poolCell);
    }
    
}