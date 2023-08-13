
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    [SerializeField]
    private Material terrainMaterial;

    [SerializeField]
    private float heightModifier;
    [SerializeField]
    private float heightLerpSpeed;

    [SerializeField]
    private float beforeHeightLerpWaitTime = 1.0f;

    [SerializeField]
    private Animator camAnimator;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Start() {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public IEnumerator GenerateTerrain(Texture2D _texture) {

        yield return new WaitForEndOfFrame();

        Vector2Int size = new Vector2Int(_texture.width, _texture.height);

        meshFilter.mesh = GeneratePlane(size);
        meshRenderer.material = terrainMaterial;

        yield return new WaitForSeconds(beforeHeightLerpWaitTime);

        StartCoroutine(SetTerrainHeight(size, _texture));

    }

    private IEnumerator SetTerrainHeight(Vector2Int _size, Texture2D _texture) {

        float completion = 0.0f;

        camAnimator.SetTrigger("StartPanning");

        while(completion < 1.0f) {
            
            List<Vector3> vertices = new List<Vector3>();

            for(int x = 0; x < _size.x; x++) {
                for(int y = 0; y < _size.y; y++) {
                    float targetHeight = Mathf.Lerp(0.0f, -heightModifier * _texture.GetPixel(x, y).r, completion);
                    vertices.Add(new Vector3(((float)x)/5, ((float)y)/5, targetHeight));
                }
            }

            meshFilter.mesh.vertices = vertices.ToArray();

            meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.RecalculateNormals();

            completion += heightLerpSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }

    }

    private Mesh GeneratePlane(Vector2Int _size) {
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for(int x = 0; x < _size.x; x++) {
            for(int y = 0; y < _size.y; y++) {

                vertices.Add(new Vector3(((float)x)/5, ((float)y)/5, 0.0f));

                if(x >= _size.x-1 || y >= _size.y-1) {
                    continue;
                }

                int i = vertices.Count-1;

                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i + _size.x + 1);

                triangles.Add(i);
                triangles.Add(i + _size.x + 1);
                triangles.Add(i + _size.x);

            }
        }

        Mesh mesh = new()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
        return mesh;

    }

}