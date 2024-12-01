using UnityEngine;
using SimplexNoise = Noise.SimplexNoise;

/// <summary>
/// Encapsulates the parameters used for generating a simplex noise map.
/// </summary>
public struct SimplexNoiseParameters
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
  /// Initializes a new instance of the <see cref="SimplexNoiseParameters"/> struct with the specified parameters.
  /// </summary>
  /// <param name="width">Width of the noise map.</param>
  /// <param name="height">Height of the noise map.</param>
  /// <param name="seed">Seed for the random generator.</param>
  /// <param name="scale">Scale factor for the noise map.</param>
  /// <param name="lacunarity">Frequency multiplier for each octave.</param>
  /// <param name="persistance">Amplitude reduction for each octave.</param>
  /// <param name="octaves">Number of noise layers.</param>
  /// <param name="normalizeMode">Normalization method.</param>
  /// <param name="offset">Offset to apply to the map coordinates.</param>
  public SimplexNoiseParameters(
      int width = SimplexNoise.DEFAULT_WIDTH,
      int height = SimplexNoise.DEFAULT_HEIGHT,
      int seed = SimplexNoise.DEFAULT_SEED,
      float scale = SimplexNoise.DEFAULT_SCALE,
      float lacunarity = SimplexNoise.DEFAULT_LACUNARITY,
      float persistance = SimplexNoise.DEFAULT_PERSISTANCE,
      int octaves = SimplexNoise.DEFAULT_OCTAVES,
      NormalizeMode normalizeMode = SimplexNoise.DEFAULT_NORMALIZE_MODE,
      Vector2? offset = null)
  {
    this.Width = width;
    this.Height = height;
    this.Seed = seed;
    this.Scale = scale;
    this.Lacunarity = lacunarity;
    this.Persistance = persistance;
    this.Octaves = octaves;
    this.NormalizeMode = normalizeMode;
    this.Offset = offset ?? SimplexNoise.DEFAULT_OFFSET;
  }
}