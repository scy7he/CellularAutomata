using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
  public Transform target;
  Vector3 lookTargetDir;
  private Camera cam;

  void Start(){
    cam = GetComponent<Camera>();
  }

  void Update() {
    Vector3 currentLookAt = transform.forward;
    lookTargetDir = (target.position - transform.position).normalized;
    cam.farClipPlane = Vector3.Distance(target.transform.position, transform.position) + 50f;

    Quaternion targetLookRotation = Quaternion.LookRotation(lookTargetDir);
    Quaternion currentRotation = transform.rotation;
    transform.rotation = Quaternion.Lerp(currentRotation, targetLookRotation, Time.deltaTime);

  }

  void OnDrawGizmosSelected() {
    if (cam != null) {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(transform.position, target.position);
    }
  }
}
