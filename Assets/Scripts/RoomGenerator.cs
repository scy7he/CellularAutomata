using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGenerator : MonoBehaviour {
	public GameObject overlay;
	public Text textPrefab;
	public bool indices;

	Vector3[] vertices = {
		Vector3.zero,
		Vector3.zero + Vector3.right,
		Vector3.zero + Vector3.right + Vector3.up,
		Vector3.zero + Vector3.right + Vector3.up + Vector3.left,
		Vector3.zero + Vector3.right + Vector3.up + Vector3.left + Vector3.forward,
		Vector3.zero + Vector3.right + Vector3.up + Vector3.left + Vector3.forward + Vector3.right,
		Vector3.zero + Vector3.right + Vector3.up + Vector3.left + Vector3.forward + Vector3.right + Vector3.down,
		Vector3.zero + Vector3.right + Vector3.up + Vector3.left + Vector3.forward + Vector3.right + Vector3.down + Vector3.left,
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

	Text[] indexLabels;

	void Start() {
		indexLabels = new Text[vertices.Length];

		for (int i=0; i<vertices.Length; i++) {
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(vertices[i]);
			Text label = Instantiate(textPrefab, screenPosition, Quaternion.identity) as Text;
			label.text = i.ToString();
			label.transform.parent = overlay.transform;
			indexLabels[i] = label;
		}

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();

		CreateCubeMesh();
	}

	void Update() {
		for (int i=0; i<vertices.Length; i++) {
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(vertices[i]);
			indexLabels[i].transform.position = screenPosition;
		}
		CreateCubeMesh();
	}

	void CreateCubeMesh() {
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;

		foreach(Vector3 vertex in vertices) {
			Gizmos.DrawSphere(vertex, .2f);
		}
  }

	void OnDrawGizmosSelected() {
		if (vertices != null){
			foreach(Vector3 vertex in vertices) {
				// Debug.Log(Camera.main.WorldToScreenPoint(vertex));
			}
		}
	}
}
