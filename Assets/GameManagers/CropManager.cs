using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : MonoBehaviour
{
  public static CropManager Instance;
  public static event Action<GameObject> targetLocationChanged;

  private void Awake()
  {
    // Ensure only one instance of the GameManager exists
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);  // Keeps the GameManager between scenes
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public void HarvestTargetObject(GameObject targetObject)
  {
    targetLocationChanged?.Invoke(targetObject);
  }
}
