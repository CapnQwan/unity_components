using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/NoiseBase", order = 1)]
public class Noise_SO : ScriptableObject
{
  [SerializeField]
  private int width, height, seed;
  public int Width => width;
  public int Height => height;
  public int Seed => seed;

  [SerializeField]
  private Vector2 offset;
  public Vector2 Offset => offset;
}
