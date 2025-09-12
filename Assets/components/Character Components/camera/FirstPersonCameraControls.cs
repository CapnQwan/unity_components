using UnityEngine;

public class FirstPersonCameraControls : MonoBehaviour
{
  [SerializeField]
  private Camera playerCamera;
  [SerializeField]
  private float mouseSensitivity;
  [SerializeField]
  public float maxLookUpAngle = 90f;
  [SerializeField]
  public float maxLookDownAngle = -90f;
  private float xRotation = 0f;

  // Update is called once per frame
  void Update()
  {
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    transform.Rotate(Vector3.up * mouseX);

    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, maxLookDownAngle, maxLookUpAngle);
    playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
  }
}
