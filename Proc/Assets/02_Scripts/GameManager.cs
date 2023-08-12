
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject cellField;
    [SerializeField]
    private Material gridMaterial;
    
    private void Start() {
        cellField.GetComponent<MeshRenderer>().material = gridMaterial;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }       
    }

}