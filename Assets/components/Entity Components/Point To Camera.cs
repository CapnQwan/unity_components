using UnityEngine;

public class PointToCamera : MonoBehaviour
{
  private Camera _targetCamera;
  public Vector3 rotationOffset;

  void Start()
  {
    if (_targetCamera == null)
    {
      _targetCamera = Camera.main;
    }
  }

  void Update()
  {
    if (_targetCamera != null)
    {
      // Make the object face the camera
      transform.LookAt(_targetCamera.transform.position);
      transform.Rotate(rotationOffset, Space.Self);
    }
  }
}
