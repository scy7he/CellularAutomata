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

public class GridManager : MonoBehaviour {

  public GameObject cellObject;
  public Material ALIVE_MATERIAL,
                  DEAD_MATERIAL;

  public const int NUMBER_OF_CELLS = 10;
  public const float SPACE_BETWEEN_CELLS = 0.1f;

  Transform gameSpace;

  private bool[,] grid;
  private GameObject[,] displayGrid;

  int currentIteration = 0;
  const int MAX_ITERATION_COUNT = 1;
  float timeLastIterated = 0,
        iterationDelay = 1f;


	void Awake () {
    gameSpace = GetComponent<Transform> ();
    grid = InitializeGrid (NUMBER_OF_CELLS);

    displayGrid = InitializeDisplayGrid (grid);

    DrawCells (displayGrid, SPACE_BETWEEN_CELLS, cellObject.transform.localScale.x, gameSpace.position.z);
	}


  bool[,] InitializeGrid(int howManyCells) {
    bool[,] grid = new bool[howManyCells, howManyCells];

    for (int y = 0; y < howManyCells; y++) {
      for (int x = 0; x < howManyCells; x++) {
        grid [y, x] = GetLiveOrDie ();
      }
    }

    return grid;
  }

  GameObject[,] InitializeDisplayGrid(bool[,] referenceGrid){
    
    GameObject[,] visualGrid = new GameObject[referenceGrid.GetLength(0), referenceGrid.GetLength(0)];

    for (int y = 0; y < referenceGrid.GetLength(0); y++) {
      for (int x = 0; x < referenceGrid.GetLength(0); x++) {

        bool isAlive = referenceGrid [y, x];

        GameObject currentCell = (GameObject)Instantiate (cellObject);
        visualGrid [y, x] = currentCell;

        Material[] materials = currentCell.GetComponent<Renderer>().materials;

        Material selectedMaterial = isAlive ? ALIVE_MATERIAL : DEAD_MATERIAL;

        materials [0] = (Material)Instantiate (selectedMaterial);

        currentCell.GetComponent<Renderer>().materials = materials;

      }
    }

    return visualGrid;
  }



  bool GetLiveOrDie(){
    float randomSeed = Random.value;

    return (randomSeed > 0.5f) ? true : false;
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

    if ((currentIteration < MAX_ITERATION_COUNT) && ((Time.fixedTime - timeLastIterated) >= iterationDelay)) {
      List<bool> neighbours = GetAllNeighbours(0,0);

      foreach (bool cell in neighbours) {
        Debug.Log (cell);
      }

      timeLastIterated = Time.fixedTime;
      currentIteration++;
    }
	}

  //  * - Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

  List<bool> GetAllNeighbours(int xCoord, int yCoord) {

    List<bool> neighbours = new List<bool> ();

    for (int y = -1; y <= 1; y++) {
      for (int x = -1; x <= 1; x++) {

        int yIndex = yCoord + y;
        int xIndex = xCoord + x;

        if ((yIndex >= 0) && (xIndex >= 0) &&                                        // protect against negative array index
            (yIndex < grid.GetLength (0)) && (xIndex < grid.GetLength (0)) &&        // protect against index past array length
            (!((xIndex == xCoord) && (yIndex == yCoord)))) {                         // we shouldn't count ourself (when calculated indexes equal what was given to us)

          GameObject currentDisplayCell = displayGrid [yIndex, xIndex];
          bool currentGridCell = grid [yIndex, xIndex];
          neighbours.Add (currentGridCell);
        }
      }
    }
  
    return neighbours;
  }

  void ChangeObjectColor(GameObject objectToChange, Color color) {
    Renderer renderer = objectToChange.GetComponent<Renderer> ();
    Material[] materials = renderer.materials;
    materials [0].SetColor ("_Color", Color.cyan);
    renderer.materials = materials;

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