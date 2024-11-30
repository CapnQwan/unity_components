using UnityEngine;
using RandomNoise = Noise.RandomNoise;

/// <summary>
/// Encapsulates the parameters used for generating a noise map.
/// </summary>
public struct RandomNoiseParameters
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
  public int Seed { get; }

  /// <summary>
  /// Gets the offset applied to the noise map.
  /// </summary>
  public Vector2 Offset { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="RandomNoiseParameters"/> struct with specified values.
  /// </summary>
  /// <param name="width">The width of the noise map. Defaults to <see cref="RandomNoise.DEFAULT_WIDTH"/>.</param>
  /// <param name="height">The height of the noise map. Defaults to <see cref="RandomNoise.DEFAULT_HEIGHT"/>.</param>
  /// <param name="seed">The seed for the random number generator. Defaults to <see cref="RandomNoise.DEFAULT_SEED"/>.</param>
  /// <param name="offset">The offset applied to the noise map. Defaults to <see cref="RandomNoise.DEFAULT_OFFSET"/>.</param>
  public RandomNoiseParameters(
      int width = RandomNoise.DEFAULT_WIDTH,
      int height = RandomNoise.DEFAULT_HEIGHT,
      int seed = RandomNoise.DEFAULT_SEED,
      Vector2? offset = null)
  {
    this.Width = width;
    this.Height = height;
    this.Seed = seed;
    this.Offset = offset ?? RandomNoise.DEFAULT_OFFSET;
  }
}
