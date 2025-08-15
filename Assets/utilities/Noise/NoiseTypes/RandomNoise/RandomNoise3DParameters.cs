using Noise;
using UnityEngine;

/// <summary>
/// Encapsulates the parameters used for generating a noise map.
/// </summary>
public struct RandomNoise3DParameters
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
  public int Seed { get; }

  /// <summary>
  /// Gets the offset applied to the noise map.
  /// </summary>
  public Vector3 Offset { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="RandomNoise3DParameters"/> struct with specified values.
  /// </summary>
  /// <param name="width">The width of the noise map. Defaults to <see cref="RandomNoise3D.DEFAULT_WIDTH"/>.</param>
  /// <param name="height">The height of the noise map. Defaults to <see cref="RandomNoise3D.DEFAULT_HEIGHT"/>.</param>
  /// <param name="depth">The depth of the noise map. Defaults to <see cref="RandomNoise3D.DEFAULT_DEPTH"/>.</param>
  /// <param name="seed">The seed for the random number generator. Defaults to <see cref="RandomNoise3D.DEFAULT_SEED"/>.</param>
  /// <param name="offset">The offset applied to the noise map. Defaults to <see cref="RandomNoise3D.DEFAULT_OFFSET"/>.</param>
  public RandomNoise3DParameters(
      int width = RandomNoise3D.DEFAULT_WIDTH,
      int height = RandomNoise3D.DEFAULT_HEIGHT,
      int depth = RandomNoise3D.DEFAULT_DEPTH,
      int seed = RandomNoise3D.DEFAULT_SEED,
      Vector3? offset = null)
  {
    this.Width = width;
    this.Height = height;
    this.Depth = depth;
    this.Seed = seed;
    this.Offset = offset ?? RandomNoise3D.DEFAULT_OFFSET;
  }
}
