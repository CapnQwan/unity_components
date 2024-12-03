using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaveletNoise
{
  private static readonly float[] waveletCoefficients = { 0.125f, 0.375f, 0.375f, 0.125f };

  /// <summary>
  /// Generates a 2D wavelet noise map.
  /// </summary>
  /// <param name="width">Width of the noise map.</param>
  /// <param name="height">Height of the noise map.</param>
  /// <param name="scale">Scale of the noise features.</param>
  /// <returns>A 2D float array representing the wavelet noise map.</returns>
  public static float[,] GenerateWaveletNoiseMap(int width, int height, float scale)
  {
    float[,] noiseMap = new float[width, height];

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        float sampleX = x / scale;
        float sampleY = y / scale;
        noiseMap[x, y] = GenerateWaveletNoise(sampleX, sampleY);
      }
    }

    // Normalize to [0, 1] range
    NormalizeNoiseMap(noiseMap);

    return noiseMap;
  }

  /// <summary>
  /// Generates wavelet noise for a single point.
  /// </summary>
  private static float GenerateWaveletNoise(float x, float y)
  {
    int x0 = Mathf.FloorToInt(x);
    int y0 = Mathf.FloorToInt(y);

    float dx = x - x0;
    float dy = y - y0;

    float result = 0f;

    // Wavelet coefficients determine the blend from neighboring points
    for (int i = -1; i <= 2; i++)
    {
      for (int j = -1; j <= 2; j++)
      {
        float contribution = waveletCoefficients[i + 1] * waveletCoefficients[j + 1];
        result += contribution * HashGrid(x0 + i, y0 + j);
      }
    }

    return Mathf.Lerp(0, 1, result); // Clamp to range
  }

  /// <summary>
  /// Hash function for pseudo-random values.
  /// </summary>
  private static float HashGrid(int x, int y)
  {
    int seed = (x * 73856093) ^ (y * 19349663); // Simple hash function
    seed = (seed ^ (seed >> 13)) * 15731 + 789221;
    return Mathf.Abs((seed & 0x7fffffff) / (float)0x7fffffff); // Normalize to [0, 1]
  }

  /// <summary>
  /// Normalizes a 2D noise map to fit within [0, 1].
  /// </summary>
  private static void NormalizeNoiseMap(float[,] noiseMap)
  {
    int width = noiseMap.GetLength(0);
    int height = noiseMap.GetLength(1);
    float min = float.MaxValue;
    float max = float.MinValue;

    // Find min and max
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        if (noiseMap[x, y] < min) min = noiseMap[x, y];
        if (noiseMap[x, y] > max) max = noiseMap[x, y];
      }
    }

    // Normalize to [0, 1]
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        noiseMap[x, y] = Mathf.InverseLerp(min, max, noiseMap[x, y]);
      }
    }
  }
}
