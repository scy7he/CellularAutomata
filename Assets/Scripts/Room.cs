using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Room : MonoBehaviour {

  Vector3[] vertices;
  public bool showIndices;
  Mesh mesh;

  Color debugColor;

  void Awake() {
  }

  void Start() {
    mesh = GetComponent<MeshFilter>().mesh;
    vertices = mesh.vertices;
    debugColor = new Color(.9f, .3f, .55f, .8f);

  }

  void OnDrawGizmosSelected() {
    Gizmos.color = debugColor;
    if (vertices != null){
      foreach(Vector3 vertex in vertices) {
        Vector3 localPos = transform.TransformPoint(vertex);
        Gizmos.DrawSphere(localPos, .1f);
      }
    }
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
  }
}
