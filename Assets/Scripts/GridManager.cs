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
  public Material ALIVE_MATERIAL,
                  DEAD_MATERIAL;

  public int NUMBER_OF_CELLS = 15;
  public const float SPACE_BETWEEN_CELLS = 0.1f;

  Transform gameSpace;

  private bool[,] grid, prevGrid;
  private GameObject[,] displayGrid;

  int currentIteration = 0,
      animationIteration = 0;
  const int MAX_ITERATION_COUNT = 100;
  public float iterationDelay;
  float timeLastIterated = 0,
        animationDelay;

  public bool debug;

	void Awake () {
    gameSpace = GetComponent<Transform> ();
    grid = InitializeGrid (NUMBER_OF_CELLS);
    animationDelay = iterationDelay/10f;

    displayGrid = InitializeDisplayGrid (grid, debug);

    DrawCells (displayGrid, SPACE_BETWEEN_CELLS, cellObject.transform.localScale.x, gameSpace.position.z);
	}


  bool[,] InitializeGrid(int howManyCells) {
    bool[,] grid = new bool[howManyCells, howManyCells];

    for (int y = 0; y < howManyCells; y++) {
      for (int x = 0; x < howManyCells; x++) {
        grid [y, x] = GetAliveOrDeadRandom ();

        
      }
    }

    return grid;
  }

  GameObject[,] InitializeDisplayGrid(bool[,] referenceGrid, bool debug){
    
    GameObject[,] visualGrid = new GameObject[referenceGrid.GetLength(0), referenceGrid.GetLength(0)];

    for (int y = 0; y < referenceGrid.GetLength(0); y++) {
      for (int x = 0; x < referenceGrid.GetLength(0); x++) {

        bool isAlive = referenceGrid [y, x];

        GameObject currentCell = (GameObject)Instantiate (cellObject);
        currentCell.name = "Cell ("+x+";"+y+")";
        currentCell.GetComponent<Cell>().SetAlive(isAlive);

        if (!debug)
          currentCell.transform.GetChild(0).gameObject.SetActive(false);
        visualGrid [y, x] = currentCell;

      }
    }

    return visualGrid;
  }



  bool GetAliveOrDeadRandom(){
    return (Random.value > 0.5f) ? true : false;
  }


  void DrawCells(GameObject[,] actualObjectsToDraw, float spaceBetweenCells, float cellScale, float depthPosition) {

    for (int y = 0; y < actualObjectsToDraw.GetLength(0); y++) {
      for (int x = 0; x < actualObjectsToDraw.GetLength(0); x++) {

        float positionX = x * cellScale;
        float spaceX = x * spaceBetweenCells;
        float positionWithSpaceX = positionX + spaceX;

        float positionY = y * cellScale;
        float spaceY = y * spaceBetweenCells;
        float positionWithSpaceY = positionY + spaceY;

        Vector3 clonePosition = new Vector3 (positionWithSpaceX, positionWithSpaceY, depthPosition);

        actualObjectsToDraw [y, x].transform.position = clonePosition;
       
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
    UpdateDisplayGrid(grid);
    timeLastIterated = Time.fixedTime;
    currentIteration++;
  }

  void UpdateDisplayGrid(bool[,] updatedGeneration) {
    for (int y = 0; y < updatedGeneration.GetLength(0); y++) {
      for (int x = 0; x < updatedGeneration.GetLength(0); x++) {
        Cell cell = displayGrid[y, x].GetComponent<Cell>();
        bool isAlive = cell.alive;
        int liveNeighbours = cell.GetLiveNeighbours();

        StateNeighbourMap snm = new StateNeighbourMap(isAlive, liveNeighbours);

        bool shouldLive = rules[snm];
        cell.SetAlive(shouldLive);
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
