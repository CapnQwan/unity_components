using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Temp.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/BlueNoise", order = 2)]
public class BlueNoise_SO : RandomNoise_SO
{
  /// <summary>
  /// Temp.
  /// </summary>
  [SerializeField]
  private float minDistance;

  /// <summary>
  /// Temp.
  /// </summary>
  public float MinDistance => this.minDistance;

  /// <summary>
  /// Temp.
  /// </summary>
  [SerializeField]
  private int maxAttempts;

  /// <summary>
  /// Temp.
  /// </summary>
  public int MaxAttempts => this.maxAttempts;

  /// <summary>
  /// Generates a Pink noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Perlin noise map.</returns>
  public override float[,] GenerateNoiseMap()
  {
    return Noise.BlueNoise.GenerateBlueNoiseMap(this);
  }
}
