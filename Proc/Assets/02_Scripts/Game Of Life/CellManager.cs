
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour {

    public Dictionary<Vector3, bool> cells = new Dictionary<Vector3, bool>();

    [SerializeField]
    private float tickTime = 0.2f;

    private void Start() {
        
    }

    private void Update() {
        
    }

    private IEnumerator Simulate() {

        yield return new WaitForSeconds(tickTime);

    }

    private void CalculateCycle() {

        foreach(KeyValuePair<Vector3, bool> kvp in cells) {
            if(kvp.Value == true) {
                
            }
        }

    }

    private void ApplyCycle(List<Vector3> _dead, List<Vector3> _alive) {

    }
    
}