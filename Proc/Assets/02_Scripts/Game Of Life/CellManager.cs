
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour {

    private Dictionary<Vector3, GameObject> cells = new Dictionary<Vector3, GameObject>();
    private Queue<GameObject> cellPool = new Queue<GameObject>();

    [SerializeField]
    private GameObject cellPrefab;


    [SerializeField]
    private int poolAmount = 50;
    [SerializeField]
    private Transform pool;

    [SerializeField]
    private Transform cellContainer;
    
    [SerializeField]
    private float tickTime = 0.2f;

    [SerializeField]
    private bool isSimulating = false;

    private bool isFillingCellPool = false;

    private void Start() {
        PlacementManager.SimulationStarted += StartSimulation;
    }

    private void Update() {

        if(Input.GetKeyDown(KeyCode.Escape)) {
            isSimulating = false;
        }

        if(cellPool.Count < poolAmount*0.2f) {
            GameObject poolCell = Instantiate(cellPrefab, pool.position, Quaternion.identity, pool);
            poolCell.GetComponent<SpriteRenderer>().enabled = false;
            cellPool.Enqueue(poolCell);
        }
        
    }

    private void StartSimulation(Dictionary<Vector3, GameObject> _cells) {

        cells = _cells;

        for(int i = 0; i < poolAmount; i++) {
            GameObject poolCell = Instantiate(cellPrefab, pool.position, Quaternion.identity, pool);
            poolCell.GetComponent<SpriteRenderer>().enabled = false;
            cellPool.Enqueue(poolCell);
        }

        StartCoroutine(Simulate());
        
    }

    private IEnumerator Simulate() {

        isSimulating = true;
        
        while(isSimulating) {

            CalculateCycle();

            // if(!isFillingCellPool && cellPool.Count < 10) {
            //     StartCoroutine(FillCellPool());
            // }

            yield return new WaitForSeconds(tickTime);

        }

    }

    private void CalculateCycle() {

        List<Vector3> emptyNeighbours = new List<Vector3>();

        List<GameObject> cellsToKill = new List<GameObject>();
        List<Vector3> cellsToInstantiate = new List<Vector3>();

        foreach(KeyValuePair<Vector3, GameObject> kvp in cells) {
            
            int neighbourCount = 0;

            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {

                    if(x == 0 && y == 0) {
                        continue;
                    }

                    Vector3 pos = kvp.Key + new Vector3(x, y, 0.0f);

                    if(cells.ContainsKey(pos)) {
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
                cellsToKill.Add(c);
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

                    if(emptyNeighbours.Contains(pos)) {
                        neighbourCount++;
                    }

                }
            }

            if(neighbourCount == 3) {
                cellsToInstantiate.Add(emptyNeighbours[i]);
            }

        }

        for(int i = 0; i < cellsToKill.Count; i++) {
            cellsToKill[i].transform.position = pool.position;
            cellsToKill[i].GetComponent<SpriteRenderer>().enabled = false;
            cellsToKill[i].transform.parent = pool;
            cellPool.Enqueue(cellsToKill[i]);
            cells.Remove(cellsToKill[i].transform.position);
        }

        for(int i = 0; i < cellsToInstantiate.Count; i++) {
            GameObject c = cellPool.Dequeue();
            c.transform.position = cellsToInstantiate[i];
            c.transform.parent = cellContainer;
            c.GetComponent<SpriteRenderer>().enabled = true;
            cells.Add(cellsToInstantiate[i], c);
        }

    }

    private IEnumerator FillCellPool() {

        isFillingCellPool = true;

        while(cellPool.Count <= poolAmount/5) {
            GameObject poolCell = Instantiate(cellPrefab, pool.position, Quaternion.identity, pool);
            poolCell.GetComponent<SpriteRenderer>().enabled = false;
            cellPool.Enqueue(poolCell);
            yield return new WaitForEndOfFrame();
        }

        isFillingCellPool = false;

    }
    
}