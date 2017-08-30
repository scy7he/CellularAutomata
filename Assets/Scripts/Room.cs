using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Room : MonoBehaviour {
  Vector3[] vertices;
  public bool showIndices, showNeighbourCount;
  Mesh mesh;
  Color debugColor;
  bool alive;
  int walls = 4;
  Vector3 center;
  float detectionRadius = 2f;
  Vector3[] detectDirections;

  List<Room> neighbours;

  bool isSelected = false;

  void Start() {
    mesh = GetComponent<MeshFilter>().mesh;
    vertices = mesh.vertices;
    debugColor = new Color(.9f, .3f, .55f, .8f);
    detectDirections = InitDetectionVectors(walls * 2);
    neighbours = new List<Room>(walls);

    center = GetComponent<Renderer>().bounds.center;
    
    neighbours = findNeighbours(center, detectDirections);

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

  public void SetAlive(bool newAlive) {
    alive = newAlive;
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = debugColor;
    if (vertices != null){
      foreach(Vector3 vertex in vertices) {
        Vector3 localPos = transform.TransformPoint(vertex);
        Gizmos.DrawSphere(localPos, .1f);
      }
    }

    if (detectDirections != null) {
      Gizmos.color = Color.green;
      foreach(Vector3 d in detectDirections) {
        Gizmos.DrawRay(center, d);
      }
    }

    foreach(Room r in neighbours) {
      if (r.alive) {
        Gizmos.DrawCube(r.center, Vector3.one * 1.2f);
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
