using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode { KEEP_LOOKING = 0, SCENE_VIEW };

public class CameraController : MonoBehaviour {
  public Transform target;
  public Light mainDirectional;
  public CameraMode cameraMode;
  Vector3 lookTargetDir;
  private Camera cam;
  public bool affectLight;
  float lightTimer;

   void Start(){
    cam = GetComponent<Camera>();
    lightTimer = Time.fixedTime;
  }

  void Update() {
    switch(cameraMode) {
      case CameraMode.KEEP_LOOKING:
        PerformKeepLookingMode();
        break;
      case CameraMode.SCENE_VIEW:
        PerformSceneViewMode();
        break;
    }
    
    if ((affectLight) && (Time.fixedTime - lightTimer > .05f)) {
      Vector3 targTransposed = new Vector3(0, target.position.y, 0);
      Vector3 transTransposed = new Vector3(0, transform.position.y, 0);
      Vector3 lightTransposed = new Vector3(0, mainDirectional.transform.position.y, 0);

      float meFloor = Vector3.Distance(transTransposed, targTransposed);
      float lightFloor = Vector3.Distance(lightTransposed, targTransposed)/5;
      
      mainDirectional.intensity = Mathf.LerpUnclamped(0f, 1f, meFloor/lightFloor);
      lightTimer = Time.fixedTime;
    }
  }

  void PerformKeepLookingMode() {
    Vector3 targetCenter = GetTargetCenter();
    lookTargetDir = (targetCenter - transform.position).normalized;
    cam.farClipPlane = Vector3.Distance(targetCenter, transform.position) + 50f;

    Quaternion targetLookRotation = Quaternion.LookRotation(lookTargetDir);
    Quaternion currentRotation = transform.rotation;
    transform.rotation = Quaternion.Lerp(currentRotation, targetLookRotation, Time.deltaTime);
  }

  void PerformSceneViewMode() {
    Camera[] cams = UnityEditor.SceneView.GetAllSceneCameras();
    transform.position = Vector3.Lerp(transform.position, cams[0].transform.position, Time.deltaTime);

    Quaternion currentRotation = transform.rotation;
    transform.rotation = Quaternion.Lerp(currentRotation, cams[0].transform.rotation, Time.deltaTime);
  }

  Vector3 GetTargetCenter() {
    return target.position;
  }

}
