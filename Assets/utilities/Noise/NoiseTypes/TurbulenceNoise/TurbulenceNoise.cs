using UnityEngine;

public static class TurbulenceNoise
{
  /// <summary>
  /// Generates a turbulence noise map using absolute-value Perlin noise layers.
  /// </summary>
  /// <param name="width">Width of the noise map.</param>
  /// <param name="height">Height of the noise map.</param>
  /// <param name="scale">Scale of the base Perlin noise.</param>
  /// <param name="octaves">Number of noise layers (octaves).</param>
  /// <param name="persistence">Amplitude scaling factor for each octave.</param>
  /// <param name="lacunarity">Frequency scaling factor for each octave.</param>
  /// <returns>A 2D float array representing the turbulence noise map.</returns>
  public static float[,] GenerateTurbulenceNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity)
  {
    float[,] turbulenceMap = new float[width, height];
    float maxAmplitude = 0f;
    float amplitude = 1f;

    for (int octave = 0; octave < octaves; octave++)
    {
      float frequency = Mathf.Pow(lacunarity, octave);
      float[,] octaveNoise = GeneratePerlinNoiseMap(width, height, frequency * scale);

      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          turbulenceMap[x, y] += Mathf.Abs(octaveNoise[x, y] * amplitude);
        }
      }

      maxAmplitude += amplitude;
      amplitude *= persistence;
    }

    // Normalize the map to the range [0, 1]
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        turbulenceMap[x, y] /= maxAmplitude;
      }
    }

    return turbulenceMap;
  }

  /// <summary>
  /// Generates a single octave of Perlin noise.
  /// </summary>
  private static float[,] GeneratePerlinNoiseMap(int width, int height, float scale)
  {
    float[,] noiseMap = new float[width, height];

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        float sampleX = x / scale;
        float sampleY = y / scale;
        noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f; // Normalize Perlin to range [-1, 1]
      }
    }

    return noiseMap;
  }
}
