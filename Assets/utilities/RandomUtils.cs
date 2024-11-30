using UnityEngine;

/// <summary>
/// A utility class providing methods for generating seeded random values and smoothed random values using Perlin noise.
/// </summary>
public static class RandomUtils
{
  // The current seed value used for the random number generator.
  private static int _seed = 0;

  // The pseudo-random number generator (PRNG) instance.
  private static System.Random _prng = new System.Random(_seed);

  /// <summary>
  /// Generates a seeded random float value between 0.0 and 1.0.
  /// </summary>
  /// <param name="seed">The seed value to initialize the random number generator.</param>
  /// <returns>A random float value between 0.0 and 1.0.</returns>
  public static float GetRandomSeededFloat(int seed)
  {
    UpdateSeed(seed);
    return (float)_prng.NextDouble();
  }

  /// <summary>
  /// Generates a seeded random float value within a specified range.
  /// </summary>
  /// <param name="seed">The seed value to initialize the random number generator.</param>
  /// <param name="min">The minimum value of the range.</param>
  /// <param name="max">The maximum value of the range.</param>
  /// <returns>A random float value between <paramref name="min"/> and <paramref name="max"/>.</returns>
  public static float GetRandomSeededfloatInRange(int seed, float min, float max)
  {
    UpdateSeed(seed);
    return ((float)_prng.NextDouble() * (max - min)) + min;
  }

  /// <summary>
  /// Generates a smoothed random float value using Perlin noise, based on input coordinates and a seed.
  /// </summary>
  /// <param name="x">The x-coordinate for the Perlin noise sample.</param>
  /// <param name="y">The y-coordinate for the Perlin noise sample.</param>
  /// <param name="seed">The seed value to initialize the random number generator.</param>
  /// <param name="offset">An offset vector added to the input coordinates.</param>
  /// <returns>A smoothed float value generated from Perlin noise.</returns>
  public static float GetRandomSmoothedFloatValue(float x, float y, int seed, Vector2 offset)
  {
    UpdateSeed(seed);

    // Generate random offsets for the Perlin noise coordinates
    float offsetX = ((float)_prng.NextDouble() * 200000) - 100000 + offset.x;
    float offsetY = ((float)_prng.NextDouble() * 200000) - 100000 + offset.y;

    // Apply offsets to the input coordinates
    float sampleX = x + offsetX;
    float sampleY = y + offsetY;

    // Generate and return the Perlin noise value
    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
    return perlinValue;
  }

  /// <summary>
  /// Updates the random number generator with a new seed, if it differs from the current seed.
  /// </summary>
  /// <param name="seed">The new seed value.</param>
  private static void UpdateSeed(int seed)
  {
    if (_seed != seed)
    {
      _seed = seed;
      _prng = new System.Random(_seed);
    }
  }
}
