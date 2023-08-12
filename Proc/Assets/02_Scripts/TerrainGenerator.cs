
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    [SerializeField]
    private Material terrainMaterial;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Start() {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public IEnumerator GenerateTerrain(Texture2D _texture) {

        yield return new WaitForEndOfFrame();

        Vector2Int size = new Vector2Int(_texture.width, _texture.height);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for(int x = 0; x < size.x; x++) {
            for(int y = 0; y < size.y; y++) {

                vertices.Add(new Vector3(x/5, y/5, 0.0f));

                if(x >= size.x-1 || y >= size.y-1) {
                    continue;
                }

                int i = vertices.Count-1;

                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i + size.x + 1);

                triangles.Add(i);
                triangles.Add(i + size.x + 1);
                triangles.Add(i + size.x);

            }
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.material = terrainMaterial;

        yield return null;
    }

}