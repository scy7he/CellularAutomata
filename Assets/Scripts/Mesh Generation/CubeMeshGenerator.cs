using System.Collections;
using UnityEngine;

public class CubeMeshGenerator {

  static Vector3[] vertices = {
    Vector3.zero,
    Vector3.right,
    Vector3.right + Vector3.up,
    Vector3.up,
    Vector3.up + Vector3.forward,
    Vector3.up + Vector3.forward + Vector3.right,
    Vector3.forward + Vector3.right,
    Vector3.forward
  };

  static int[] triangles = {
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

  public static Mesh GenerateMesh(float scale) {
    Mesh mesh = new Mesh();

    for (int i=0; i<vertices.Length; i++) {
      vertices[i] *= scale;
    }

    mesh.Clear ();
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.RecalculateNormals ();

    return mesh;
  }
}
