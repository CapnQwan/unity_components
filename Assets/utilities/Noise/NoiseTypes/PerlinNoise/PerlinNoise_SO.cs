using Noise;
using UnityEngine;

/// <summary>
/// Enum representing the mode for normalizing Perlin noise values.
/// </summary>
public enum NormalizeMode
{
  /// <summary>
  /// Normalizes the noise values locally, within the context of the current map's min and max values.
  /// </summary>
  Local,

  /// <summary>
  /// Normalizes the noise values globally, considering a pre-determined maximum possible range.
  /// </summary>
  Global,
}

/// <summary>
/// A ScriptableObject class for configuring Perlin noise generation parameters.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/PerlinNoise", order = 2)]
public class PerlinNoise_SO : RandomNoise_SO
{
  /// <summary>
  /// The scale factor for the Perlin noise map, influencing the frequency of the noise pattern.
  /// </summary>
  [SerializeField]
  private float scale;

  /// <summary>
  /// Gets the scale factor for the Perlin noise map.
  /// </summary>
  public float Scale => this.scale;

  /// <summary>
  /// The lacunarity value, which controls the frequency multiplier between octaves.
  /// </summary>
  [SerializeField]
  private float lacunarity;

  /// <summary>
  /// Gets the lacunarity value for the Perlin noise map.
  /// </summary>
  public float Lacunarity => this.lacunarity;

  /// <summary>
  /// The persistence value, which determines the amplitude reduction between octaves.
  /// </summary>
  [SerializeField]
  private float persistance;

  /// <summary>
  /// Gets the persistence value for the Perlin noise map.
  /// </summary>
  public float Persistance => this.persistance;

  /// <summary>
  /// The number of octaves to use for generating multi-layered noise.
  /// </summary>
  [SerializeField]
  private int octaves;

  /// <summary>
  /// Gets the number of octaves for the Perlin noise map.
  /// </summary>
  public int Octaves => this.octaves;

  /// <summary>
  /// The mode for normalizing the Perlin noise values.
  /// </summary>
  [SerializeField]
  private NormalizeMode normalizeMode;

  /// <summary>
  /// Gets the normalization mode for the Perlin noise map.
  /// </summary>
  public NormalizeMode NormalizeMode => this.normalizeMode;

  /// <summary>
  /// Generates a Perlin noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Perlin noise map.</returns>
  public override float[,] GenerateNoiseMap(int width, int height)
  {
    return Noise.PerlinNoise.GeneratePerlinNoiseMap(width, height, this);
  }

  /// <summary>
  /// Generates a Perlin noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Perlin noise map.</returns>
  public override float[,] GenerateNoiseMap()
  {
    return Noise.PerlinNoise.GeneratePerlinNoiseMap(this);
  }
}
