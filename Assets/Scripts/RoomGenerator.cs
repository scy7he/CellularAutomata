using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GenerationAlgorithm { MAZE = 0 };

public class RoomGenerator : MonoBehaviour {
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
    timerActive = false;
    lastStepTime = 0f;
    roomMesh = (!roomMesh || useManualMesh) ? CubeMeshGenerator.GenerateMesh(roomScale) : roomMesh;
    ResetGeneration(floorSize);
  }

  void InstantiateRooms(Vector2 dimensions, float scale, int indexStartAt=0) {
    int gridSize = GetGridSize(dimensions);

    for (int index = indexStartAt; index < gridSize; index++) {
      int x = GetXYFromIndex(index, dimensions)[0],
          y = GetXYFromIndex(index, dimensions)[1];
      Vector3 position = GetPositionAtIndex(x, y, dimensions, scale);
      string name ="Room "+x+" "+y; 
      bool state = GetRandomBool(seedLifeChance);
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

  Vector3 GetPositionAtIndex(int x, int y, Vector2 max, float scale) {
    Vector3 pos = new Vector3(x * scale, 0f, y * scale);
    Vector3 extents = new Vector3(max.x * scale, 0f, max.y * scale);
    return transform.position + pos - (extents/2);
  }

  Vector3 GetCenterOffset(float scale) {
    return (Vector3.one * scale)/2;
  }

  int[] GetXYFromIndex(int index, Vector2 dimensions) {
      int x = (int) (index % dimensions.x),
          y = (int) ((index - x) / dimensions.x);
    return new int[2]{x,y};
  }

  int GetGridSize(Vector2 xy) {
    return (int) (xy.x * xy.y);
  }

  void ResetGeneration(Vector2 floorSize, int offset=0) {
    generationCount = 0;
    if (offset == 0) {
      rooms = new List<Room>(GetGridSize(floorSize));
    }
    InstantiateRooms(floorSize, roomScale, offset);
  }

  void IncreaseGridBy(Vector2 floorSize, int dSize) {
    int oldSize = GetGridSize(floorSize);
    floorSize.x += dSize;
    floorSize.y += dSize;
    ResetGeneration(floorSize, oldSize);
  }

  bool GetRandomBool(float chance) {
    return (Random.value > chance) ? true : false;
  }

  bool IsUpdateTime(float timeD, float lastTime, float delay) {
    return (timeD - lastTime > delay);
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Space) ||
        (timerActive && IsUpdateTime(Time.deltaTime, lastStepTime, stepDelay))){
      lastStepTime = RunSingleProgression(rooms);
    }
  }

  float RunSingleProgression(List<Room> rooms) {
    foreach(Room room in rooms) {
      room.StepGeneration();
    }
    UpdateGeneration();
    return Time.fixedTime;
  }

  int UpdateGeneration() {
    return generationCount++;
  }

  void ResetGeneration() {
    generationCount = 0;
  }

  Vector2 GetGridDimensions() {
    return this.floorSize;
  }

  void OnGUI() {
  
    if (GUILayout.Button("Load Initial State")) {
      timerActive = false;
      generationCount = 0;
      foreach (Room room in rooms) {
        room.RestoreInitialState();
      }
    }

    if (GUILayout.Button("Load Random State")) {
      timerActive = false;
      generationCount = 0;
      foreach (Room room in rooms) {
        room.SetAlive(GetRandomBool(.8f));
      }
    }

    if (GUILayout.Button("Play/Pause")) {
      timerActive = !timerActive;
    }

    increaseAmount = GUILayout.TextField(increaseAmount,2);
    if (GUILayout.Button("Increase grid by: ")) {
      IncreaseGridBy(GetGridDimensions(), int.Parse(increaseAmount));
    }
  }
  // void OnDrawGizmosSelected() {
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
