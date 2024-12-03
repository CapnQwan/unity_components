namespace Noise
{
  using UnityEngine;

  public static class PinkNoise
  {
    public static float[,] GeneratePinkNoiseMap(PinkNoise_SO noiseScriptableObject)
    {
      return GeneratePinkNoiseMap(
        noiseScriptableObject.Width,
        noiseScriptableObject.Height,
        noiseScriptableObject.Octaves,
        noiseScriptableObject.Persistance,
        noiseScriptableObject.Scale);
    }

    /// <summary>
    /// Generates a pink noise map using the fractional Brownian motion (fBm) approach.
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="octaves">Number of octaves (layers) for the noise generation.</param>
    /// <param name="persistence">Controls amplitude scaling between octaves.</param>
    /// <param name="scale">Overall scale of the noise.</param>
    /// <returns>A 2D float array representing the pink noise map.</returns>
    public static float[,] GeneratePinkNoiseMap(int width, int height, int octaves, float persistence, float scale)
    {
      float[,] noiseMap = new float[width, height];
      float maxAmplitude = 0f;
      float amplitude = 1f;

      // Iterate over each octave
      for (int octave = 0; octave < octaves; octave++)
      {
        // Generate Perlin noise for this octave
        float frequency = Mathf.Pow(2, octave);
        float[,] octaveNoise = GeneratePerlinNoiseMap(width, height, frequency * scale);

        // Add scaled octave noise to the pink noise map
        for (int x = 0; x < width; x++)
        {
          for (int y = 0; y < height; y++)
          {
            noiseMap[x, y] += octaveNoise[x, y] * amplitude;
          }
        }

        maxAmplitude += amplitude;
        amplitude *= persistence;
      }

      // Normalize the noise map to range [0, 1]
      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          noiseMap[x, y] /= maxAmplitude;
        }
      }

      return noiseMap;
    }

    /// <summary>
    /// Generates a Perlin noise map.
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
          noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
        }
      }

      return noiseMap;
    }
  }
}