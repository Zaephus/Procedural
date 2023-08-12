
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour {

    private GameObject[,] cells;
    private Queue<GameObject> cellPool = new Queue<GameObject>();

    private TextureGenerator textureGenerator;

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
    private int iterationAmount;

    private void Start() {
        PlacementManager.SimulationStarted += StartSimulation;

        textureGenerator = GetComponent<TextureGenerator>();

        cells = new GameObject[fieldSize.x, fieldSize.y];
    }

    private void Update() {

        if(cellPool.Count < poolAmount*0.2f) {
            AddCellToPool();
        }
        
    }

    private void StartSimulation(Dictionary<Vector3, GameObject> _cells, int _poolAmount, int _iterations) {

        foreach(KeyValuePair<Vector3, GameObject> kvp in _cells) {
            cells[(int)kvp.Key.x, (int)kvp.Key.y] = kvp.Value;
        }

        poolAmount = _poolAmount;
        for(int i = 0; i < _poolAmount; i++) {
            AddCellToPool();
        }

        iterationAmount = _iterations;

        StartCoroutine(Simulate());
        PlacementManager.SimulationStarted -= StartSimulation;
        
    }

    private IEnumerator Simulate() {
        
        for(int i = 0; i < iterationAmount; i++) {
            yield return new WaitForSeconds(tickTime);
            CalculateCycle();
        }

        pool.gameObject.SetActive(false);
        cellContainer.gameObject.SetActive(false);
        textureGenerator.StartCoroutine(textureGenerator.GenerateTexture(cells));

    }

    private void CalculateCycle() {

        GameObject[,] cellBuffer = (GameObject[,])cells.Clone();

        for(int x = 0; x < fieldSize.x; x++) {
            for(int y = 0; y < fieldSize.y; y++) {

                GameObject cell = cellBuffer[x, y];

                if(cell != null) {

                    int neighbourCount = GetNeighbourAmount(x, y, ref cellBuffer);

                    if(neighbourCount < 2 || neighbourCount > 3) {

                        cells[x, y] = null;

                        cell.transform.position = pool.position;
                        cell.GetComponent<SpriteRenderer>().enabled = false;
                        cell.transform.parent = pool;
                        cellPool.Enqueue(cell);
                    }

                }
                else {

                    int neighbourCount = GetNeighbourAmount(x, y, ref cellBuffer);

                    if(neighbourCount == 3) {
                        GameObject c = cellPool.Dequeue();
                        c.transform.position = new Vector3(x, y, 0.0f);
                        c.transform.parent = cellContainer;
                        c.GetComponent<SpriteRenderer>().enabled = true;
                        cells[x, y] = c;
                    }

                }

            }

        }

    }

    private int GetNeighbourAmount(int _x, int _y, ref GameObject[,] _buffer) {

        int neighbourCount = 0;

        for(int i = -1; i < 2; i++) {
            for(int j = -1; j < 2; j++) {

                if(i == 0 && j == 0) {
                    continue;
                }

                Vector2Int neighbourPos = new Vector2Int((_x + i) % (fieldSize.x), (_y + j) % (fieldSize.y));
                neighbourPos = neighbourPos.x == -1 ? neighbourPos = new Vector2Int(fieldSize.x-1, neighbourPos.y) : neighbourPos;
                neighbourPos = neighbourPos.x == fieldSize.x ? neighbourPos = new Vector2Int(0, neighbourPos.y) : neighbourPos;
                
                neighbourPos = neighbourPos.y == -1 ? neighbourPos = new Vector2Int(neighbourPos.x, fieldSize.y-1) : neighbourPos;
                neighbourPos = neighbourPos.y == fieldSize.y ? neighbourPos = new Vector2Int(neighbourPos.x, 0) : neighbourPos;

                if(_buffer[neighbourPos.x, neighbourPos.y] != null) {
                    neighbourCount++;
                }

            }
        }

        return neighbourCount;

    }

    private void AddCellToPool() {
        GameObject poolCell = Instantiate(cellPrefab, pool.position, Quaternion.identity, pool);
        poolCell.GetComponent<SpriteRenderer>().enabled = false;
        cellPool.Enqueue(poolCell);
    }
    
}