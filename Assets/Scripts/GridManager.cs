/*
 * Game of life rules 
 * -------------------
 * 
 * - Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
 * - Any live cell with two or three live neighbours lives on to the next generation.
 * - Any live cell with more than three live neighbours dies, as if by overpopulation.
 * - Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct StateNeighbourMap {
  public bool state;
  public int numNeighbours;
  public StateNeighbourMap(bool s, int n) {
    state = s;
    numNeighbours = n;
  }
}

public class GridManager : MonoBehaviour {

  public float liveAccuracy, liveAccThreshold;
  public float reviveAccuracy, reviveAccThreshold;

  bool rulesShouldLive(bool alive, int neighbours, int aliveNeighbours) {
    float ratio = (float)aliveNeighbours/(float)neighbours;

    if (alive) {
      return (ratio - liveAccuracy) > liveAccThreshold;
      // return (ratio >= 2f/8f - liveAccuracy) && (ratio <= 3f/8f + liveAccuracy);
    }
    else {
      return (ratio - reviveAccuracy) > reviveAccThreshold;
      // return ((ratio >= (3f/8f - reviveAccuracy)) && (ratio <= (3f/8f + reviveAccuracy)));
    }
  }
  Dictionary<StateNeighbourMap, bool> rules = new Dictionary<StateNeighbourMap, bool>{
    {new StateNeighbourMap(true, 0), false},
    {new StateNeighbourMap(true, 1), false},
    {new StateNeighbourMap(true, 2), true},
    {new StateNeighbourMap(true, 3), true},
    {new StateNeighbourMap(true, 4), false},
    {new StateNeighbourMap(true, 5), false},
    {new StateNeighbourMap(true, 6), false},
    {new StateNeighbourMap(true, 7), false},
    {new StateNeighbourMap(true, 8), false},

    {new StateNeighbourMap(false, 0), false},
    {new StateNeighbourMap(false, 1), false},
    {new StateNeighbourMap(false, 2), false},
    {new StateNeighbourMap(false, 3), true},
    {new StateNeighbourMap(false, 4), false},
    {new StateNeighbourMap(false, 5), false},
    {new StateNeighbourMap(false, 6), false},
    {new StateNeighbourMap(false, 7), false},
    {new StateNeighbourMap(false, 8), false},
  };

  public GameObject cellObject;
  public int NUMBER_OF_CELLS;
  public const float SPACE_BETWEEN_CELLS = 0.1f;

  Transform gameSpace;

  private bool[,,] grid;
  private GameObject[,,] displayGrid;

  int currentIteration = 0,
      animationIteration = 0;
  const int MAX_ITERATION_COUNT = 100;
  public float iterationDelay;
  float timeLastIterated = 0,
        animationDelay;

  public bool debug;

	void Awake () {
    gameSpace = GetComponent<Transform> ();

    displayGrid = InitializeDisplayGrid (InitializeGrid (NUMBER_OF_CELLS), debug);

    DrawCells (displayGrid, SPACE_BETWEEN_CELLS, cellObject.transform.localScale.x, gameSpace.position.z);
	}


  bool[,,] InitializeGrid(int howManyCells) {
    bool[,,] grid = new bool[howManyCells, howManyCells, howManyCells];

    for (int y = 0; y < howManyCells; y++) {
      for (int x = 0; x < howManyCells; x++) {
        for (int z = 0; z < howManyCells; z++) {
          grid [z, y, x] = GetAliveOrDeadRandom ();
        }
      }
    }

    return grid;
  }

  GameObject[,,] InitializeDisplayGrid(bool[,,] referenceGrid, bool debug){
    
    GameObject[,,] visualGrid = new GameObject[referenceGrid.GetLength(0), referenceGrid.GetLength(1), referenceGrid.GetLength(2)];

    for (int y = 0; y < referenceGrid.GetLength(0); y++) {
      for (int x = 0; x < referenceGrid.GetLength(0); x++) {
        for (int z = 0; z < referenceGrid.GetLength(0); z++) {
          bool isAlive = referenceGrid [z, y, x];

          GameObject currentCell = (GameObject)Instantiate (cellObject);
          currentCell.name = "Cell ("+x+";"+y+";"+z+")";
          currentCell.GetComponent<Cell>().SetAlive(isAlive);

          if (!debug)
            currentCell.transform.GetChild(0).gameObject.SetActive(false);
          visualGrid [z, y, x] = currentCell;
        }

      }
    }

    return visualGrid;
  }



  bool GetAliveOrDeadRandom(){
    return (Random.value > 0.8f) ? true : false;
  }


  void DrawCells(GameObject[,,] actualObjectsToDraw, float spaceBetweenCells, float cellScale, float depthPosition) {

    for (int y = 0; y < actualObjectsToDraw.GetLength(0); y++) {
      for (int x = 0; x < actualObjectsToDraw.GetLength(0); x++) {
        for (int z = 0; z < actualObjectsToDraw.GetLength(0); z++) {
          float positionX = x * cellScale;
          float spaceX = x * spaceBetweenCells;
          float positionWithSpaceX = positionX + spaceX;

          float positionY = y * cellScale;
          float spaceY = y * spaceBetweenCells;
          float positionWithSpaceY = positionY + spaceY;

          float positionZ = z * cellScale;
          float spaceZ = z * spaceBetweenCells;
          float positionWithSpaceZ = positionZ + spaceZ;

          Vector3 clonePosition = new Vector3 (positionWithSpaceX, positionWithSpaceY, positionWithSpaceZ);

          actualObjectsToDraw [z, y, x].transform.position = clonePosition;
        }
      }
    }
  }


	void Update () {

    if (!debug)
      if ((currentIteration < MAX_ITERATION_COUNT) && ((Time.fixedTime - timeLastIterated) >= iterationDelay)) {
        IncrementGeneration();
      }
	}

  void IncrementGeneration(){
    UpdateDisplayGrid();
    timeLastIterated = Time.fixedTime;
    currentIteration++;
  }

  void UpdateDisplayGrid() {
    for (int y = 0; y < displayGrid.GetLength(0); y++) {
      for (int x = 0; x < displayGrid.GetLength(0); x++) {
        for (int z = 0; z < displayGrid.GetLength(0); z++) {
          Cell cell = displayGrid[z, y, x].GetComponent<Cell>();
          bool isAlive = cell.alive;
          int liveNeighbours = cell.GetLiveNeighbours();

          // Debug.Log(cell.name+": "+liveNeighbours+","+cell.GetNeighbours()+" ("+(float)liveNeighbours/(float)cell.GetNeighbours());
          bool shouldLive = rulesShouldLive(isAlive, cell.GetNeighbours(), liveNeighbours);
          cell.SetAlive(shouldLive);
        }
      }
    }
  }

  //  * - Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

  void OnGUI() {
    if (debug)
      if (GUILayout.Button("Next Generation"))
        IncrementGeneration();
  }
}

//
//[[0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
//[0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
//[0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
//[0, 0, 0, 0, 0, 0, 0, h0, g0, f0]
//[0, 0, 0, 0, 0, 0, a0, *0*, e0, 0]
//[0, 0, 0, 0, 0, 0, b0, c0, d0, 0]
//[0, 0, 0, 0, 0, 0, 0, 0, 0, 0]]
//
//x: 8
//y: 7


//*a* (
