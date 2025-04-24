using UnityEngine;
using PerlinNoise3D = Noise.PerlinNoise3D;

/// <summary>
/// Encapsulates the parameters used for generating a perlin noise map.
/// </summary>
public struct PerlinNoise3DParameters
{
  /// <summary>
  /// Gets the width of the noise map.
  /// </summary>
  public int Width { get; }

  /// <summary>
  /// Gets the height of the noise map.
  /// </summary>
  public int Height { get; }

  /// <summary>
  /// Gets the depth of the noise map.
  /// </summary>
  public int Depth { get; }

  /// <summary>
  /// Gets the seed for the random number generator.
  /// </summary>

  /// <summary>Gets the Seed for the random number generator.</summary>
  public int Seed { get; }

  /// <summary>Gets the Scale factor for the noise map.</summary>
  public float Scale { get; }

  /// <summary>Gets the Controls frequency of each octave.</summary>
  public float Lacunarity { get; }

  /// <summary>Gets the Controls amplitude reduction of each octave.</summary>
  public float Persistance { get; }

  /// <summary>Gets the Number of octaves for generating layered noise.</summary>
  public int Octaves { get; }

  /// <summary>Gets the Normalization mode for the noise values.</summary>
  public NormalizeMode NormalizeMode { get; }

  /// <summary>
  /// Gets the offset applied to the noise map.
  /// </summary>
  public Vector2 Offset { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="PerlinNoise3DParameters"/> struct with the specified parameters.
  /// </summary>
  /// <param name="width">Width of the noise map.</param>
  /// <param name="height">Height of the noise map.</param>
  /// <param name="depth">Height of the noise map.</param>
  /// <param name="seed">Seed for the random generator.</param>
  /// <param name="scale">Scale factor for the noise map.</param>
  /// <param name="lacunarity">Frequency multiplier for each octave.</param>
  /// <param name="persistance">Amplitude reduction for each octave.</param>
  /// <param name="octaves">Number of noise layers.</param>
  /// <param name="normalizeMode">Normalization method.</param>
  /// <param name="offset">Offset to apply to the map coordinates.</param>
  public PerlinNoise3DParameters(
      int width = PerlinNoise3D.DEFAULT_WIDTH,
      int height = PerlinNoise3D.DEFAULT_HEIGHT,
      int depth = PerlinNoise3D.DEFAULT_DEPTH,
      int seed = PerlinNoise3D.DEFAULT_SEED,
      float scale = PerlinNoise3D.DEFAULT_SCALE,
      float lacunarity = PerlinNoise3D.DEFAULT_LACUNARITY,
      float persistance = PerlinNoise3D.DEFAULT_PERSISTANCE,
      int octaves = PerlinNoise3D.DEFAULT_OCTAVES,
      NormalizeMode normalizeMode = PerlinNoise3D.DEFAULT_NORMALIZE_MODE,
      Vector2? offset = null)
  {
    this.Width = width;
    this.Height = height;
    this.Depth = depth;
    this.Seed = seed;
    this.Scale = scale;
    this.Lacunarity = lacunarity;
    this.Persistance = persistance;
    this.Octaves = octaves;
    this.NormalizeMode = normalizeMode;
    this.Offset = offset ?? PerlinNoise3D.DEFAULT_OFFSET;
  }
}
