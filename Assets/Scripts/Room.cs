using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Room : MonoBehaviour {
  Vector3[] vertices, detectDirections;
  public bool showIndices, showNeighbourCount, initialAlive, alive;
  public Material on, off;
  Mesh mesh;
  Color debugColor;
  public bool timerActive;
  int walls = 4;
  Vector3 center;
  float detectionRadius = 2f;
  List<Room> neighbours;

  bool isSelected;

  void Start() {
    mesh             = GetComponent<MeshFilter>().mesh;
    vertices         = mesh.vertices;
    debugColor       = new Color(.9f, .3f, .55f, .8f);
    detectDirections = InitDetectionVectors(walls * 2);
    neighbours       = new List<Room>(walls);
    center           = GetComponent<Renderer>().bounds.center;
    neighbours       = findNeighbours(center, detectDirections);
  }

  public void StepGeneration() {
    int aliveNeighbours = GetAliveNeighbours(); 
    if (alive) {
      if ((aliveNeighbours < 1) || (aliveNeighbours > 5)){
        SetAlive(false);
      }
    }
    else {
      if (aliveNeighbours == 3) {
        SetAlive(true);
      }
    }
  }

  List<Room> findNeighbours(Vector3 center, Vector3[] directions) {
    List<Room> foundAdjacent = new List<Room>();

    foreach(Vector3 dir in directions) {
      Ray ray = new Ray(center, dir);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, detectionRadius)) {
        Room newNeighb = hit.collider.gameObject.GetComponent<Room>();
        if ((newNeighb != null) && (!foundAdjacent.Contains(newNeighb))) {
          foundAdjacent.Add(newNeighb);
        }
      }
    }

    return foundAdjacent;
  }

  Material getStateMaterial(bool alive) {
    return alive ? on : off;
  }

  void SetOwnMaterial(bool alive) {
    Material newMaterial = (Material) Instantiate(getStateMaterial(alive));
    Material[] materials = GetComponent<Renderer>().materials;
    materials [0] = newMaterial;
    GetComponent<Renderer>().materials = materials;
  }

  public void RestoreInitialState() {
    SetAlive(initialAlive);
  }

  public void SetAlive(bool newAlive, bool initial=false) {
    alive = newAlive;
    SetOwnMaterial(alive);

    if (initial)
      initialAlive = alive;
  }

  public int GetAliveNeighbours() {
    int alive = 0;
    foreach (Room n in neighbours) {
      if (n.alive) {
        alive++;
      }
    }
    return alive;
  }

  Vector3[] InitDetectionVectors(int wallCount) {
    Vector3[] directions = new Vector3[wallCount];

    Vector3 origin = Vector3.forward;
    directions[0] = origin;

    for (int i=1; i< wallCount; i++) {
      Vector3 nextDirection = Quaternion.Euler(0, (360/wallCount) * i, 0) * origin;
      directions[i] = nextDirection;
    }

    return directions;
  }

  public bool IsAlive() {
    return alive;
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = debugColor;
    // if (vertices != null){
    //   foreach(Vector3 vertex in vertices) {
    //     Vector3 localPos = transform.TransformPoint(vertex);
    //     Gizmos.DrawSphere(localPos, .1f);
    //   }
    // }

    if (detectDirections != null) {
      // Gizmos.color = Color.green;
      foreach(Vector3 d in detectDirections) {
        Gizmos.DrawRay(center, d *1.5f);
      }
    }

    foreach(Room r in neighbours) {
      if (r.IsAlive()) {
        Gizmos.color = new Color(.2f, .8f, .2f, .95f);
        Gizmos.DrawWireCube(r.center, r.transform.localScale * 1.2f);
      }
      else {
        // Gizmos.color = Color.black;
        // Gizmos.DrawCube(r.center, Vector3.one * 1.2f);
      }
    }
    isSelected = true;
  }

  void OnGUI() {
    if (showIndices && vertices != null) {
      GUI.color = Color.green;
      for (int i=0; i<vertices.Length; i++) {
        Vector3 localPos = transform.TransformPoint(vertices[i]);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(localPos);
        float invertedY = Screen.height - screenPosition.y;
        GUI.Label(new Rect(screenPosition.x, invertedY,75,25), i.ToString());
      }
    }

    if (showNeighbourCount && isSelected && neighbours != null) {
      GUI.color = Color.white;
      GUIStyle fontStyle = new GUIStyle();
      fontStyle.fontSize = 50;
      fontStyle.normal.textColor = Color.white;

      Vector3 screenPosition = Camera.main.WorldToScreenPoint( center );
      float invertedY = Screen.height - screenPosition.y;
      GUI.Label(new Rect(screenPosition.x, invertedY,75,25), GetAliveNeighbours().ToString(), fontStyle);
    }
  }
}
