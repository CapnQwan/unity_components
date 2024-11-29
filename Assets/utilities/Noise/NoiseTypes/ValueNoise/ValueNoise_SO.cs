using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/ValueNoise", order = 2)]
public class ValueNoise_SO : Noise_SO
{
  [SerializeField]
  private int gridX, gridY;
  public int GridX => gridX;
  public int GridY => gridY;
}
