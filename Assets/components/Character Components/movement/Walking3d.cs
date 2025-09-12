using UnityEngine;

public class Walking3d : MonoBehaviour
{
  [SerializeField]
  private float moveSpeed = 1.0f;
  [SerializeField]
  private Rigidbody rb;
  private Vector3 _move = Vector3.zero;

  public void Update()
  {
    HandleInputs();
    HandleMovement();
  }

  private void HandleInputs()
  {
    _move.z = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0;
    _move.x = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0;
  }

  private void HandleMovement()
  {
    Vector3 moveDirection = transform.forward * _move.z + transform.right * _move.x;
    moveDirection = moveDirection.normalized * moveSpeed;

    // Apply movement
    if (rb != null)
    {
      moveDirection.y = rb.velocity.y;
      rb.velocity = moveDirection;
    }
    else
    {
      // Use Transform for non-physics movement
      transform.Translate(moveDirection * Time.deltaTime, Space.World);
    }
  }
}
