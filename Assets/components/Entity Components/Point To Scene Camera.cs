using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PointToSceneCamera : MonoBehaviour
{
  public Vector3 rotationOffset;

  void Update()
  {
#if UNITY_EDITOR
    // Get the Scene View Camera
    if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
    {
      Camera sceneCamera = SceneView.lastActiveSceneView.camera;

      // Make the object face the Scene View Camera
      transform.LookAt(sceneCamera.transform.position);
      transform.Rotate(rotationOffset, Space.Self);
    }
#endif
  }
}