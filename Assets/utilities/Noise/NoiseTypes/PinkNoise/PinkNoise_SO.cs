using UnityEngine;

/// <summary>
/// Temp.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/PinkNoise", order = 2)]
public class PinkNoise_SO : RandomNoise_SO
{
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
  /// The scale factor for the Perlin noise map, influencing the frequency of the noise pattern.
  /// </summary>
  [SerializeField]
  private float scale;

  /// <summary>
  /// Gets the scale factor for the Perlin noise map.
  /// </summary>
  public float Scale => this.scale;

  /// <summary>
  /// Generates a Pink noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Perlin noise map.</returns>
  public override float[,] GenerateNoiseMap(int width, int height)
  {
    return Noise.PinkNoise.GeneratePinkNoiseMap(width, height, this);
  }

  /// <summary>
  /// Generates a Pink noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Perlin noise map.</returns>
  public override float[,] GenerateNoiseMap()
  {
    return Noise.PinkNoise.GeneratePinkNoiseMap(this);
  }
}
