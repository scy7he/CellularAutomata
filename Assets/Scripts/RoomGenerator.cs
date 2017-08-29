using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGenerator : MonoBehaviour {
  public GameObject roomPrefab;
  public Material cellMaterial;
  public float roomScale;

  Vector3[] vertices = {
    Vector3.zero,
    Vector3.right,
    Vector3.right + Vector3.up,
    Vector3.up,
    Vector3.up + Vector3.forward,
    Vector3.up + Vector3.forward + Vector3.right,
    Vector3.forward + Vector3.right,
    Vector3.forward
  };

  int[] triangles = {
    0, 2, 1, //face front
    0, 3, 2,
    2, 3, 4, //face top
    2, 4, 5,
    1, 2, 5, //face right
    1, 5, 6,
    0, 7, 4, //face left
    0, 4, 3,
    5, 4, 7, //face back
    5, 7, 6,
    0, 6, 7, //face bottom
    0, 1, 6
  };

  void Start() {
    CreateCubeMesh(vertices, triangles, roomScale);
  }

  void CreateCubeMesh(Vector3[] verts, int[] tris, float scale) {
    GameObject newCell = Instantiate(roomPrefab, transform.position, Quaternion.identity);

    Material newMaterial = (Material) Instantiate(cellMaterial);
    Material[] materials = newCell.GetComponent<Renderer>().materials;
    materials [0] = newMaterial;
    newCell.GetComponent<Renderer>().materials = materials;

    Mesh mesh = newCell.GetComponent<MeshFilter>().mesh;

    for (int i=0; i<verts.Length; i++) {
      verts[i] *= scale;
    }

    mesh.Clear ();
    mesh.vertices = verts;
    mesh.triangles = tris;
    mesh.RecalculateNormals ();
  }
}
