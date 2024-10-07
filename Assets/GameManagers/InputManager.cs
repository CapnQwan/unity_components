using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
  public static InputManager Instance;
  public static Vector3 targetLocation;
  public static event Action<Vector3> targetLocationChanged;
  public static GameObject target;
  public static event Action<GameObject> targetChanged;

  private void Awake()
  {
    // Ensure only one instance of the GameManager exists
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);  // Keeps the InputManager between scenes
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public void UpdateTargetLocation(Vector3 newTarget)
  {
    targetLocation = newTarget;
    targetLocationChanged?.Invoke(targetLocation);
  }

  public void UpdateTargetLocation(Transform newTarget)
  {
    targetLocation = newTarget.position;
    targetLocationChanged?.Invoke(targetLocation);
  }

  public void UpdateTarget(GameObject newTarget)
  {
    target = newTarget;
    targetChanged?.Invoke(target);
  }
}
