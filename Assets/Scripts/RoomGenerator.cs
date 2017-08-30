using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GenerationAlgorithm { MAZE = 0 };

public class RoomGenerator : MonoBehaviour {
  public GenerationAlgorithm method;
  public GameObject roomPrefab;
  public Material primaryMaterial, secondaryMaterial;
  public float roomScale;
  public bool showIndices;
  public Vector2 floorSize;
  List<GameObject> rooms;
  float lastStepTime = 0f;

  public bool[,] seed = {
    {true, false, false, false, false, false, false, false, false, false },
    {false, true, false, false, false, false, false, false, false, false },
    {false, true, true, false, false, false, false, false, false, false },
    {false, true, false, false, false, false, false, false, false, false },
    {false, false, false, false, false, false, false, false, false, false },
    {false, false, false, false, false, false, false, false, false, false },
    {false, false, false, false, false, false, false, false, false, false },
    {false, false, false, false, false, false, false, false, false, false },
    {false, false, false, false, false, false, false, false, false, false },
    {false, false, false, false, false, false, false, false, false, false },
  };

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
    rooms = new List<GameObject>();

    for (int y=0; y<floorSize.y; y++) {
      for (int x=0; x<floorSize.x; x++) {
        Vector3 position = GetPositionAtIndex(x, y, floorSize, roomScale);
        rooms.Add(CreateCubeMesh(x, y, vertices, triangles, roomScale, position, seed[x,y]));
      }
    }
  }

  void Update() {

    if (Time.fixedTime - lastStepTime > 1f) {
      switch(method) {
        case GenerationAlgorithm.MAZE:
          PerformRandom();
          break;
      }
    }

  }

  void PerformRandom() {

  }

  GameObject CreateCubeMesh(int x, int y, Vector3[] verts, int[] tris, float scale, Vector3 position, bool alive) {
    GameObject newCell = Instantiate(roomPrefab, position, Quaternion.identity, transform) as GameObject;
    newCell.name = "Room "+x+" "+y;
    newCell.GetComponent<Room>().SetAlive(alive);
    newCell.GetComponent<Room>().showNeighbourCount = true;

    Material newMaterial = (Material) Instantiate(alive ? primaryMaterial : secondaryMaterial);
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

    newCell.GetComponent<MeshCollider>().sharedMesh = mesh;

    return newCell;
  }

  Vector3 GetPositionAtIndex(int x, int y, Vector2 max, float scale) {
    Vector3 pos = new Vector3(x * scale, 0f, y * scale);
    Vector3 extents = new Vector3(max.x * scale, 0f, max.y * scale);

    return transform.position + pos - (extents/2);
  }

  Vector3 GetCenterOffset(float scale) {
    return (Vector3.one * scale)/2;
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = Color.cyan;
    for (int y=0; y<floorSize.y; y++) {
      for (int x=0; x<floorSize.x; x++) {
        Gizmos.DrawWireCube(
          GetPositionAtIndex(x, y, floorSize, roomScale) + GetCenterOffset(roomScale), 
          Vector3.one * roomScale
        );
      }
    }
  }

  void OnGUI() {
    if (showIndices) {
      GUI.color = Color.red;
      GUIStyle fontStyle = new GUIStyle();
      fontStyle.fontSize = 50;
      fontStyle.normal.textColor = Color.white;

      for (int i=0; i<rooms.Count; i++) {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
          rooms[i].transform.position + GetCenterOffset(roomScale)
        );
        float invertedY = Screen.height - screenPosition.y;
        GUI.Label(new Rect(screenPosition.x, invertedY,75,25), i.ToString(), fontStyle);
      }
    }
  }
}
