namespace Noise
{
  using System;
  using UnityEngine;

  public static class WaveletNoise
  {
    private static readonly float[] waveletCoefficients = {
      0.000334f,-0.001528f, 0.000410f, 0.003545f,-0.000938f,-0.008233f, 0.002172f, 0.019120f,
      -0.005040f,-0.044412f, 0.011655f, 0.103311f,-0.025936f,-0.243780f, 0.033979f, 0.655340f,
      0.655340f, 0.033979f,-0.243780f,-0.025936f, 0.103311f, 0.011655f,-0.044412f,-0.005040f,
      0.019120f, 0.002172f,-0.008233f,-0.000938f, 0.003545f, 0.000410f,-0.001528f, 0.000334f };

    public static float[,] GenerateWaveletNoiseMap(WavletNoise_SO noiseScriptableObject)
    {
      return GenerateWaveletNoiseMap(
        noiseScriptableObject.Width,
        noiseScriptableObject.Height,
        noiseScriptableObject.Seed,
        noiseScriptableObject.Scale,
        noiseScriptableObject.Octaves,
        noiseScriptableObject.Persistance,
        noiseScriptableObject.Lacunarity);
    }

    /// <summary>
    /// Generates a 2D wavelet noise map.
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="scale">Scale of the noise features.</param>
    /// <returns>A 2D float array representing the wavelet noise map.</returns>
    public static float[,] GenerateWaveletNoiseMap(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity)
    {
      float[,] noiseMap = new float[width, height];
      System.Random rand = new System.Random(seed);
      float offsetX = (float)rand.NextDouble() * 1000f;
      float offsetY = (float)rand.NextDouble() * 1000f;

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          float amplitude = 1f;
          float frequency = 1f;
          float noiseValue = 0f;

          for (int octave = 0; octave < octaves; octave++)
          {
            float sampleX = (x + offsetX) / scale * frequency;
            float sampleY = (y + offsetY) / scale * frequency;

            noiseValue += WaveletNoisePoint(sampleX, sampleY) * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
          }

          noiseMap[x, y] = noiseValue;
        }
      }

      return NormalizeNoiseMap(noiseMap, width, height);
    }

    /// <summary>
    /// Generates wavelet noise for a single point.
    /// </summary>
    private static float WaveletNoisePoint(float x, float y)
    {
      // Determine grid cell coordinates
      int x0 = (int)Math.Floor(x);
      int x1 = x0 + 1;
      int y0 = (int)Math.Floor(y);
      int y1 = y0 + 1;

      // Interpolation weights
      float wx = x - x0;
      float wy = y - y0;

      // Compute wavelet contributions from 4 corners
      float value = 0.0f;
      value += Wavelet(wx) * Wavelet(wy) * Noise(x0, y0);
      value += Wavelet(wx - 1) * Wavelet(wy) * Noise(x1, y0);
      value += Wavelet(wx) * Wavelet(wy - 1) * Noise(x0, y1);
      value += Wavelet(wx - 1) * Wavelet(wy - 1) * Noise(x1, y1);

      return value;
    }

    private static float Wavelet(float t)
    {
      // A simple wavelet basis function (Haar Wavelet example)
      t = Math.Abs(t);
      if (t < 1.0f) return 1.0f - t;
      return 0.0f;
    }

    // Simple hash-based pseudorandom function for grid noise
    private static float Noise(int x, int y)
    {
      int n = x + y * 57;
      n = (n << 13) ^ n;
      return (1.0f - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
    }

    /// <summary>
    /// Normalizes a 2D noise map to fit within [0, 1].
    /// </summary>
    private static float[,] NormalizeNoiseMap(float[,] noiseMap, int width, int height)
    {
      float minValue = float.MaxValue;
      float maxValue = float.MinValue;

      foreach (var value in noiseMap)
      {
        if (value < minValue) minValue = value;
        if (value > maxValue) maxValue = value;
      }

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          noiseMap[x, y] = (noiseMap[x, y] - minValue) / (maxValue - minValue);
        }
      }

      return noiseMap;
    }
  }
}
