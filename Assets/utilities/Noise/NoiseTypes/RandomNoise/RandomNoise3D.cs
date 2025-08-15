namespace Noise
{
  using System;
  using UnityEngine;

  public static class RandomNoise3D
  {
    public const int DEFAULT_WIDTH = 50;
    public const int DEFAULT_HEIGHT = 50;
    public const int DEFAULT_DEPTH = 50;
    public const int DEFAULT_SEED = 0;
    public static readonly Vector3 DEFAULT_OFFSET = new Vector3(0f, 0f, 0f);
    private const int OFFSET_RANGE = 100000;

    public static float[,,] GenerateRandomNoiseMap(int width, int height, int depth, Vector3 offset, int seed)
    {
      return GenerateRandomNoiseMap(
          width,
          height,
          depth,
          seed,
          offset);
    }

    /// <summary>
    /// Generates a random noise map using parameters from a Noise_SO scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    public static float[,,] GenerateRandomNoiseMap(int width, int height, int depth, Vector3 offset, RandomNoise_SO noiseScriptableObject)
    {
      return GenerateRandomNoiseMap(
          width,
          height,
          depth,
          noiseScriptableObject.Seed,
          offset);
    }

    /// <summary>
    /// Generates a random noise map using parameters from a Noise_SO scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    public static float[,,] GenerateRandomNoiseMap(int width, int height, int depth, RandomNoise_SO noiseScriptableObject)
    {
      return GenerateRandomNoiseMap(
          width,
          height,
          depth,
          noiseScriptableObject.Seed,
          noiseScriptableObject.Offset);
    }

    /// <summary>
    /// Generates a random noise map using parameters from a Noise_SO scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    public static float[,,] GenerateRandomNoiseMap(RandomNoise3D_SO noiseScriptableObject)
    {
      return GenerateRandomNoiseMap(
          noiseScriptableObject.Width,
          noiseScriptableObject.Height,
          noiseScriptableObject.Depth,
          noiseScriptableObject.Seed,
          noiseScriptableObject.Offset);
    }

    /// <summary>
    /// Generates a random noise map using a NoiseParameters struct.
    /// </summary>
    /// <param name="noiseParameters">A struct containing noise generation parameters.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    public static float[,,] GenerateRandomNoiseMap(RandomNoise3DParameters noiseParameters)
    {
      return GenerateRandomNoiseMap(
          noiseParameters.Width,
          noiseParameters.Height,
          noiseParameters.Depth,
          noiseParameters.Seed,
          noiseParameters.Offset);
    }

    /// <summary>
    /// Generates a random noise map using specified parameters.
    /// </summary>
    /// <param name="width">The width of the noise map.</param>
    /// <param name="height">The height of the noise map.</param>
    /// <param name="depth">The depth of the noise map.</param>
    /// <param name="seed">The seed for the random number generator.</param>
    /// <param name="offset">The offset applied to the noise map.</param>
    /// <returns>A 2D array of floats representing the noise map.</returns>
    /// <exception cref="ArgumentException">Thrown if width or height is less than or equal to zero.</exception>
    public static float[,,] GenerateRandomNoiseMap(
        int width = DEFAULT_WIDTH,
        int height = DEFAULT_HEIGHT,
        int depth = DEFAULT_DEPTH,
        int seed = DEFAULT_SEED,
        Vector3 offset = default)
    {
      if (width <= 0 || height <= 0 || depth <= 0)
      {
        throw new ArgumentException("RandomNoise - Width, height, and depth must be greater than zero.");
      }

      float[,,] noiseMap = new float[width, height, depth];

      System.Random prng = new System.Random(seed);

      float randomOffsetX = prng.Next(-OFFSET_RANGE, OFFSET_RANGE);
      float randomOffsetY = prng.Next(-OFFSET_RANGE, OFFSET_RANGE);
      float randomOffsetZ = prng.Next(-OFFSET_RANGE, OFFSET_RANGE);
      float offsetX = randomOffsetX + offset.x;
      float offsetY = randomOffsetY + offset.y;
      float offsetZ = randomOffsetZ + offset.z;

      for (int z = 0; z < depth; z++)
      {
        for (int y = 0; y < height; y++)
        {
          for (int x = 0; x < width; x++)
          {
            float sampleX = (x + offsetX) / 0.539f;
            float sampleY = (y + offsetY) / 0.539f;
            float sampleZ = (z + offsetZ) / 0.539f;

            float noiseValueXY = Mathf.PerlinNoise(sampleX, sampleY);
            float noiseValueXZ = Mathf.PerlinNoise(sampleX, sampleZ);
            float noiseValueYZ = Mathf.PerlinNoise(sampleY, sampleZ);

            float noiseValue = (noiseValueXY + noiseValueXZ + noiseValueYZ) / 3f;

            noiseMap[x, y, z] = noiseValue;
          }
        }
      }

      return noiseMap;
    }
  }
}
