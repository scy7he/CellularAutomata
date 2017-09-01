using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellularAutomata.Assets.Scripts;

public enum GenerationAlgorithm { MAZE = 0 };

public class RoomGenerator : MonoBehaviour
{
  public GenerationAlgorithm method;
  public GameObject roomPrefab;
  public Mesh roomMesh;
  public float roomScale, seedLifeChance, stepDelay;
  public bool showIndices, timerActive, useManualMesh;
  public Vector2 floorSize;
  List<Room> rooms;
  float lastStepTime;
  int generationCount;
  public string increaseAmount;


  void Start() {
    SetTimerActive(false);
    lastStepTime = 0f;
    roomMesh = (!roomMesh || useManualMesh) ? CubeMeshGenerator.GenerateMesh(roomScale) : roomMesh;
    ResetGeneration(ref rooms, floorSize);
  }

  void ResetGeneration(ref List<Room> rooms, Vector2 floorSize) {
    SetGeneration(0);
    rooms = new List<Room>(Utils.GetGridSize(floorSize));
    InstantiateRooms(ref rooms, floorSize, roomScale);
  }

  void InstantiateRooms(ref List<Room> rooms, Vector2 dimensions, float scale) {
    int gridSize = Utils.GetGridSize(dimensions);

    for (int index = 0; index < gridSize; index++) {
      int x = Utils.GetXYFromIndex(index, dimensions)[0],
          y = Utils.GetXYFromIndex(index, dimensions)[1];
      Vector3 position = Utils.GetPositionAtIndex(transform, x, y, dimensions, scale);
      string name = "Room " + x + " " + y;
      bool state = Utils.GetRandomBool(seedLifeChance);
      rooms.Add(CreateCubeGameObj(roomMesh, name, position, state).GetComponent<Room>());
    }
  }

  GameObject CreateCubeGameObj(Mesh mesh, string name, Vector3 position, bool alive) {
    GameObject newCell = Instantiate(roomPrefab, position, Quaternion.identity, transform) as GameObject;
    newCell.name = name;
    Room newRoom = newCell.GetComponent<Room>();
    newRoom.SetAlive(alive, initial: true);
    newCell.GetComponent<MeshFilter>().mesh = roomMesh;
    return newCell;
  }

  void SetGridWith(Vector2 floorSize, int dSize) {
    int oldSize = Utils.GetGridSize(floorSize);
    floorSize.x = dSize;
    floorSize.y = dSize;
    ResetGeneration(ref rooms, floorSize);
  }

  bool IsUpdateTime(bool  timerActive,float timeNow,float lastTime,float delay) {
    return timerActive && (timeNow - lastTime > delay);
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Space) || (IsUpdateTime(timerActive, Time.fixedTime, lastStepTime, stepDelay))) {
      lastStepTime = RunSingleProgression(rooms);
    }
  }

  void SetTimerActive(bool a = true) {
    timerActive = a;
  }

  float RunSingleProgression(List<Room> rooms) {
    foreach (Room room in rooms) {
      room.StepGeneration();
    }
    UpdateGeneration();
    return Time.fixedTime;
  }

  int UpdateGeneration() {
    return generationCount++;
  }

  void SetGeneration(int i) {
    generationCount = i;
  }

  Vector2 GetGridDimensions() {
    return this.floorSize;
  }

  void OnGUI() {
    GUILayout.BeginArea(new Rect(Screen.width-160, Screen.height-200, 150, 200)); 

    if (GUILayout.Button("Play/Pause")) {
      SetTimerActive(!timerActive);
    }

    if (GUILayout.Button("Load Initial State")) {
      SetTimerActive(false);
      SetGeneration(0);
      foreach (Room room in rooms) {
        room.RestoreInitialState();
      }
    }

    if (GUILayout.Button("Load Random State")) {
      SetTimerActive(false);
      SetGeneration(0);
      foreach (Room room in rooms) {
        if (room != null) {
          room.SetAlive(Utils.GetRandomBool(seedLifeChance));
        }
      }
    }

    if (GUILayout.Button("Set grid square length:")) {
      if (increaseAmount != "") {
        DemolishGrid();
        SetGridWith(GetGridDimensions(), int.Parse(increaseAmount));
      }
    }

    increaseAmount = GUILayout.TextField(increaseAmount, 2);

    GUILayout.Label("generation "+generationCount.ToString());
    GUILayout.Label("instances "+rooms.Count.ToString());
    GUILayout.EndArea();
  }

  void DemolishGrid() {
    foreach (Room r in transform.GetComponentsInChildren<Room>()) {
      Destroy(r.gameObject);
    }
  }

  void OnDrawGizmosSelected() { }
  //   Gizmos.color = Color.cyan;
  //   for (int y=0; y<floorSize.y; y++) {
  //     for (int x=0; x<floorSize.x; x++) {
  //       Gizmos.DrawWireCube(
  //         GetPositionAtIndex(x, y, floorSize, roomScale) + GetCenterOffset(roomScale), 
  //         Vector3.one * roomScale
  //       );
  //     }
  //   }
  // }
}
