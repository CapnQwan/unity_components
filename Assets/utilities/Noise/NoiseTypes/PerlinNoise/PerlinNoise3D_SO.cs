using UnityEngine;

/// <summary>
/// A ScriptableObject class for configuring Perlin noise generation parameters.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise3D/PerlinNoise3D", order = 2)]
public class PerlinNoise3D_SO : ScriptableObject
{

  /// <summary>
  /// The width of the noise map.
  /// </summary>
  [SerializeField]
  private int width;

  /// <summary>
  /// Gets the width of the noise map.
  /// </summary>
  public int Width => this.width;

  /// <summary>
  /// The height of the noise map.
  /// </summary>
  [SerializeField]
  private int height;

  /// <summary>
  /// Gets the height of the noise map.
  /// </summary>
  public int Height => this.height;

  /// <summary>
  /// The depth of the noise map.
  /// </summary>
  [SerializeField]
  private int depth;

  /// <summary>
  /// Gets the depth of the noise map.
  /// </summary>
  public int Depth => this.depth;

  /// <summary>
  /// The seed for the random number generator used in noise generation.
  /// </summary>
  [SerializeField]
  private int seed;

  /// <summary>
  /// Gets the seed for the random number generator used in noise generation.
  /// </summary>
  public int Seed => this.seed;

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
  /// The offset applied to the noise map.
  /// </summary>
  [SerializeField]
  private Vector2 offset;

  /// <summary>
  /// Gets the offset applied to the noise map.
  /// </summary>
  public Vector2 Offset => this.offset;

  /// <summary>
  /// Generates a Perlin noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 3D array of float values representing the Perlin noise map.</returns>
  public float[,,] GenerateNoiseMap(int width, int height, int depth)
  {
    return Noise.PerlinNoise3D.GeneratePerlinNoiseMap(width, height, depth, this);
  }

  /// <summary>
  /// Generates a Perlin noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 3D array of float values representing the Perlin noise map.</returns>
  public float[,,] GenerateNoiseMap(int width, int height, int depth, Vector3 offset)
  {
    return Noise.PerlinNoise3D.GeneratePerlinNoiseMap(width, height, depth, offset, this);
  }

  /// <summary>
  /// Generates a Perlin noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 3D array of float values representing the Perlin noise map.</returns>
  public float[,,] GenerateNoiseMap()
  {
    return Noise.PerlinNoise3D.GeneratePerlinNoiseMap(this);
  }
}
