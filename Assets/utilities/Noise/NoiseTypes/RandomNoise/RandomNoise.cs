namespace Noise
{
  using System;
  using UnityEngine;

  /// <summary>
  /// Provides utility methods for generating random noise maps using Perlin noise.
  /// </summary>
  public static class RandomNoise
  {
    /// <summary>
    /// Default width of the noise map.
    /// </summary>
    public const int DEFAULT_WIDTH = 50;

    /// <summary>
    /// Default height of the noise map.
    /// </summary>
    public const int DEFAULT_HEIGHT = 50;

    /// <summary>
    /// Default seed for the random number generator.
    /// </summary>
    public const int DEFAULT_SEED = 0;

    /// <summary>
    /// Default offset for the noise map.
    /// </summary>
    public static readonly Vector2 DEFAULT_OFFSET = new Vector2(0f, 0f);

    /// <summary>
    /// Range for generating random offsets in the noise map.
    /// </summary>
    private const int OFFSET_RANGE = 100000;

    /// <summary>
    /// Generates a random noise map using parameters from a Noise_SO scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    public static float[,] GenerateRandomNoiseMap(RandomNoise_SO noiseScriptableObject)
    {
      return GenerateRandomNoiseMap(
          noiseScriptableObject.Width,
          noiseScriptableObject.Height,
          noiseScriptableObject.Seed,
          noiseScriptableObject.Offset);
    }

    /// <summary>
    /// Generates a random noise map using a NoiseParameters struct.
    /// </summary>
    /// <param name="noiseParameters">A struct containing noise generation parameters.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    public static float[,] GenerateRandomNoiseMap(RandomNoiseParameters noiseParameters)
    {
      return GenerateRandomNoiseMap(
          noiseParameters.Width,
          noiseParameters.Height,
          noiseParameters.Seed,
          noiseParameters.Offset);
    }

    /// <summary>
    /// Generates a random noise map using specified parameters.
    /// </summary>
    /// <param name="width">The width of the noise map.</param>
    /// <param name="height">The height of the noise map.</param>
    /// <param name="seed">The seed for the random number generator.</param>
    /// <param name="offset">The offset applied to the noise map.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    /// <exception cref="ArgumentException">Thrown if width or height is less than or equal to zero.</exception>
    public static float[,] GenerateRandomNoiseMap(
        int width = DEFAULT_WIDTH,
        int height = DEFAULT_HEIGHT,
        int seed = DEFAULT_SEED,
        Vector2 offset = default)
    {
      if (width <= 0 || height <= 0)
      {
        throw new ArgumentException("RandomNoise - Width and height must be greater than zero.");
      }

      float[,] noiseMap = new float[width, height];

      System.Random prng = new System.Random(seed);

      float randomOffsetX = prng.Next(-OFFSET_RANGE, OFFSET_RANGE);
      float randomOffsetY = prng.Next(-OFFSET_RANGE, OFFSET_RANGE);
      float offsetX = randomOffsetX + offset.x;
      float offsetY = randomOffsetY + offset.y;

      Vector2 noiseOffset = new Vector2(offsetX, offsetY);

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          float sampleX = (x + noiseOffset.x) / 0.539f; // Is this 0.539f needed
          float sampleY = (y + noiseOffset.y) / 0.539f; // Is this 0.539f needed

          noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
        }
      }

      return noiseMap;
    }
  }
}
