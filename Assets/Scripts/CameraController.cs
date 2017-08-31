using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode { KEEP_LOOKING = 0, SCENE_VIEW };

public class CameraController : MonoBehaviour {
  public Transform target;
  public CameraMode cameraMode;
  Vector3 lookTargetDir;
  private Camera cam;

  void Start(){
    cam = GetComponent<Camera>();
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
    // return target.GetComponent<Renderer>().bounds.center;
    return target.position;
  }

  // void OnDrawGizmosSelected() {
  //   if (cam != null) {
  //     Gizmos.color = Color.red;
  //     Gizmos.DrawLine(transform.position, GetTargetCenter());
  //   }
  // }
}
