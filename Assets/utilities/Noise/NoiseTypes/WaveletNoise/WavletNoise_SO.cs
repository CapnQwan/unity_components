using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Temp.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/WavletNoise", order = 2)]
public class WavletNoise_SO : RandomNoise_SO
{
  /// <summary>
  /// The scale factor for the Wavlet noise map, influencing the frequency of the noise pattern.
  /// </summary>
  [SerializeField]
  private float scale;

  /// <summary>
  /// Gets the scale factor for the Wavlet noise map.
  /// </summary>
  public float Scale => this.scale;

  /// <summary>
  /// The lacunarity value, which controls the frequency multiplier between octaves.
  /// </summary>
  [SerializeField]
  private float lacunarity;

  /// <summary>
  /// Gets the lacunarity value for the Wavlet noise map.
  /// </summary>
  public float Lacunarity => this.lacunarity;

  /// <summary>
  /// The persistence value, which determines the amplitude reduction between octaves.
  /// </summary>
  [SerializeField]
  private float persistance;

  /// <summary>
  /// Gets the persistence value for the Wavlet noise map.
  /// </summary>
  public float Persistance => this.persistance;

  /// <summary>
  /// The number of octaves to use for generating multi-layered noise.
  /// </summary>
  [SerializeField]
  private int octaves;

  /// <summary>
  /// Gets the number of octaves for the Wavlet noise map.
  /// </summary>
  public int Octaves => this.octaves;

  /// <summary>
  /// Generates a Wavlet noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Wavlet noise map.</returns>
  public override float[,] GenerateNoiseMap()
  {
    return Noise.WaveletNoise.GenerateWaveletNoiseMap(this);
  }
}
