namespace Noise
{
  using System;
  using UnityEngine;

  /// <summary>
  /// A static class for generating Perlin noise maps using customizable parameters.
  /// </summary>
  public static class PerlinNoise3D
  {
    /// <summary>Default width of the generated noise map.</summary>
    public const int DEFAULT_WIDTH = 50;

    /// <summary>Default height of the generated noise map.</summary>
    public const int DEFAULT_HEIGHT = 50;

    /// <summary>Default depth of the generated noise map.</summary>
    public const int DEFAULT_DEPTH = 50;

    /// <summary>Default seed for the random number generator.</summary>
    public const int DEFAULT_SEED = 0;

    /// <summary>Default scale factor for the noise map.</summary>
    public const float DEFAULT_SCALE = 0.5f;

    /// <summary>Default lacunarity value for controlling frequency.</summary>
    public const float DEFAULT_LACUNARITY = 0.5f;

    /// <summary>Default persistence value for controlling amplitude reduction.</summary>
    public const float DEFAULT_PERSISTANCE = 0.5f;

    /// <summary>Default number of octaves for generating multi-layer noise.</summary>
    public const int DEFAULT_OCTAVES = 3;

    /// <summary>Default normalization mode for noise values.</summary>
    public const NormalizeMode DEFAULT_NORMALIZE_MODE = NormalizeMode.Local;

    /// <summary>Default offset applied to the noise map coordinates.</summary>
    public static readonly Vector2 DEFAULT_OFFSET = new Vector2(0f, 0f);

    /// <summary>Range for generating random offsets in the noise map.</summary>
    private const int OFFSET_RANGE = 100000;

    /// <summary>
    /// Generates a Perlin noise map using a <see cref="PerlinNoise3D_SO"/> scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    public static float[,,] GeneratePerlinNoiseMap(int width, int height, int depth, PerlinNoise3D_SO noiseScriptableObject)
    {
      return GeneratePerlinNoiseMap(
        width,
        height,
        depth,
        noiseScriptableObject.Seed,
        noiseScriptableObject.Octaves,
        noiseScriptableObject.Scale,
        noiseScriptableObject.Lacunarity,
        noiseScriptableObject.Persistance,
        noiseScriptableObject.NormalizeMode,
        noiseScriptableObject.Offset);
    }

    /// <summary>
    /// Generates a Perlin noise map using a <see cref="PerlinNoise3D_SO"/> scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    public static float[,,] GeneratePerlinNoiseMap(PerlinNoise3D_SO noiseScriptableObject)
    {
      return GeneratePerlinNoiseMap(
        noiseScriptableObject.Width,
        noiseScriptableObject.Height,
        noiseScriptableObject.Depth,
        noiseScriptableObject.Seed,
        noiseScriptableObject.Octaves,
        noiseScriptableObject.Scale,
        noiseScriptableObject.Lacunarity,
        noiseScriptableObject.Persistance,
        noiseScriptableObject.NormalizeMode,
        noiseScriptableObject.Offset);
    }

    /// <summary>
    /// Generates a Perlin noise map using <see cref="PerlinNoise3DParameters"/>.
    /// </summary>
    /// <param name="perlinNoiseParameters">A struct containing all the parameters for the noise generation.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    public static float[,,] GeneratePerlinNoiseMap(PerlinNoise3DParameters perlinNoiseParameters)
    {
      return GeneratePerlinNoiseMap(
        perlinNoiseParameters.Width,
        perlinNoiseParameters.Height,
        perlinNoiseParameters.Depth,
        perlinNoiseParameters.Seed,
        perlinNoiseParameters.Octaves,
        perlinNoiseParameters.Scale,
        perlinNoiseParameters.Lacunarity,
        perlinNoiseParameters.Persistance,
        perlinNoiseParameters.NormalizeMode,
        perlinNoiseParameters.Offset);
    }

    /// <summary>
    /// Generates a Perlin noise map with specified parameters.
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="depth">Depth of the noise map.</param>
    /// <param name="seed">Seed for the random number generator.</param>
    /// <param name="octaves">Number of octaves to use for multi-layered noise.</param>
    /// <param name="scale">Scale factor for the noise map.</param>
    /// <param name="lacunarity">Controls the frequency of each octave.</param>
    /// <param name="persistance">Controls the amplitude reduction of each octave.</param>
    /// <param name="normalizeMode">Specifies how to normalize the noise values.</param>
    /// <param name="offset">Offset applied to the noise map coordinates.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    /// <exception cref="ArgumentException">Thrown if width, height, octaves, or lacunarity are invalid.</exception>
    public static float[,,] GeneratePerlinNoiseMap(
      int width = DEFAULT_WIDTH,
      int height = DEFAULT_HEIGHT,
      int depth = DEFAULT_DEPTH,
      int seed = DEFAULT_SEED,
      int octaves = DEFAULT_OCTAVES,
      float scale = DEFAULT_SCALE,
      float lacunarity = DEFAULT_LACUNARITY,
      float persistance = DEFAULT_PERSISTANCE,
      NormalizeMode normalizeMode = DEFAULT_NORMALIZE_MODE,
      Vector2 offset = default)
    {
      if (width <= 0 || height <= 0)
      {
        throw new ArgumentException("Width and height must be greater than zero.");
      }

      if (octaves <= 0)
      {
        throw new ArgumentException("Octaves must be greater than zero.");
      }

      if (lacunarity <= 0)
      {
        throw new ArgumentException("Lacunarity must be greater than zero.");
      }

      float[,,] noiseMap = new float[width, height, depth];

      System.Random prng = new System.Random(seed);
      Vector2[] octaveOffsets = new Vector2[octaves];

      float maxPossibleHeight = 0;
      float amplitude = 1;

      for (int i = 0; i < octaves; i++)
      {
        float offsetX = prng.Next(-OFFSET_RANGE, OFFSET_RANGE) + offset.x;
        float offsetY = prng.Next(-OFFSET_RANGE, OFFSET_RANGE) - offset.y;
        octaveOffsets[i] = new Vector2(offsetX, offsetY);

        maxPossibleHeight += amplitude;
        amplitude *= persistance;
      }

      if (scale <= 0)
      {
        scale = 0.0001f;
      }

      float maxLocalNoiseHeight = float.MinValue;
      float minLocalNoiseHeight = float.MaxValue;

      float halfWidth = width * 0.5f;
      float halfHeight = height * 0.5f;

      // Thread-safe variables for min/max values
      object lockObject = new object();

      _ = System.Threading.Tasks.Parallel.For(0, height, y =>
      {
        float localMax = float.MinValue;
        float localMin = float.MaxValue;

        for (int x = 0; x < width; x++)
        {
          for (int z = 0; z < depth; z++)
          {
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;

            for (int i = 0; i < octaves; i++)
            {
              float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
              float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

              float perlinValue = (Mathf.PerlinNoise(sampleX, sampleY) * 2) - 1;
              noiseHeight += perlinValue * amplitude;

              amplitude *= persistance;
              frequency *= lacunarity;
            }

            localMax = Mathf.Max(localMax, noiseHeight);
            localMin = Mathf.Min(localMin, noiseHeight);

            noiseMap[x, y, z] = noiseHeight;
          }
        }

        // Update global min/max safely
        lock (lockObject)
        {
          maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight, localMax);
          minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight, localMin);
        }
      });

      _ = System.Threading.Tasks.Parallel.For(0, height, y =>
      {
        for (int x = 0; x < width; x++)
        {
          for (int z = 0; x < depth; z++)
          {

            if (normalizeMode == NormalizeMode.Local)
            {
              noiseMap[x, y, z] = NormalizeValue(noiseMap[x, y, z], minLocalNoiseHeight, maxLocalNoiseHeight);
            }
            else
            {
              float normalizedHeight = (noiseMap[x, y, z] + 1) / maxPossibleHeight;
              noiseMap[x, y, z] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
            }
          }
        }
      });

      return noiseMap;
    }

    private static float NormalizeValue(float value, float min, float max)
    {
      return Mathf.InverseLerp(min, max, value);
    }
  }
}
