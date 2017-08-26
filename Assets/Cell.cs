using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
	public bool alive;

  public Material ALIVE_MATERIAL,
                  DEAD_MATERIAL;

	List<GameObject> neighbourObj;

	void Awake() {
		neighbourObj = new List<GameObject>();
	}

	void Update() {
		GetComponentInChildren<TextMesh>().text = GetLiveNeighbours().ToString();
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
			ChangeObjectMaterial(ALIVE_MATERIAL);
		else
			ChangeObjectMaterial(DEAD_MATERIAL);
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
}
