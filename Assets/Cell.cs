using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
	public bool alive;

  public Material ALIVE_MATERIAL,
                  DEAD_MATERIAL;

	List<GameObject> neighbourObj;
	TextMesh debugText;

	void Awake() {
		neighbourObj = new List<GameObject>();
		debugText = GetComponentInChildren<TextMesh>();
		debugText.gameObject.SetActive(false);
		ChangeObjectMaterial(ALIVE_MATERIAL);
	}

	void Update() {
		if (debugText.gameObject.activeInHierarchy)	
			debugText.text = GetLiveNeighbours().ToString();
	}

	public int GetNeighbours() {
		return neighbourObj.Count;
	}

	public int GetLiveNeighbours() {
		int liveNeighbours = 0;
		foreach (GameObject g in neighbourObj) {
			bool isAlive = g.GetComponent<Cell>().alive;
			if (isAlive)
				liveNeighbours++;
		}

		return liveNeighbours;
	}
	
	public void SetAlive(bool alive) {
		this.alive = alive;
		if (alive)
			// ChangeObjectMaterial(ALIVE_MATERIAL);
			GetComponent<MeshRenderer>().enabled = true;
		else
			// ChangeObjectMaterial(DEAD_MATERIAL);
			GetComponent<MeshRenderer>().enabled= false;
	}

	void OnTriggerEnter(Collider col) {
		if (!neighbourObj.Contains(col.gameObject)) {
			neighbourObj.Add(col.gameObject);
		}
	}

	void OnTriggerLeave(Collider col) {
		neighbourObj.Remove(col.gameObject);
	}

	void ChangeObjectMaterial(Material newMat) {
		Material[] materials = GetComponent<Renderer>().materials;
		materials [0] = (Material)Instantiate (newMat);
		GetComponent<Renderer>().materials = materials;
	}

	void OnDrawGizmosSelected() {
		debugText.gameObject.SetActive(true);
    Gizmos.color = Color.magenta;
		foreach (GameObject g in neighbourObj) {
			if (g.GetComponent<Cell>().alive)
				Gizmos.DrawCube(g.transform.position, g.transform.localScale);
		}
	}
}
