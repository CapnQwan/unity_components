using UnityEngine;

public static class PerlinNoise
{
  private const int OFFSET_RANGE = 100000;

  /// <summary>
  /// Generates a Perlin Noise map using parameters from a ScriptableObject.
  /// </summary>
  public static float[,] GeneratePerlinNoiseMap(PerlinNoise_SO noiseScriptableObject)
  {
    return GeneratePerlinNoiseMap(
      noiseScriptableObject.Width,
      noiseScriptableObject.Height,
      noiseScriptableObject.Seed,
      noiseScriptableObject.Octaves,
      noiseScriptableObject.Scale,
      noiseScriptableObject.Lacunarity,
      noiseScriptableObject.Persistance,
      noiseScriptableObject.NormalizeMode,
      noiseScriptableObject.Offset);
  }

  /// <summary>
  /// Generates a Perlin Noise map using the specified parameters.
  /// </summary>
  /// <param name="width">Width of the noise map.</param>
  /// <param name="height">Height of the noise map.</param>
  /// <param name="seed">Seed for random generation.</param>
  /// <param name="octaves">Number of octaves for noise layers.</param>
  /// <param name="scale">Scale of the noise.</param>
  /// <param name="lacunarity">Change in frequency between octaves.</param>
  /// <param name="persistance">Change in amplitude between octaves.</param>
  /// <param name="normalizeMode">Normalization mode for noise values.</param>
  /// <param name="offset">Offset to apply to the noise map.</param>
  public static float[,] GeneratePerlinNoiseMap(
    int width,
    int height,
    int seed,
    int octaves,
    float scale,
    float lacunarity,
    float persistance,
    NormalizeMode normalizeMode,
    Vector2 offset)
  {
    float[,] noiseMap = new float[width, height];

    System.Random prng = new System.Random(seed);
    Vector2[] octaveOffsets = new Vector2[octaves];

    float maxPossibleHeight = 0;
    float amplitude = 1;
    float frequency;

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

    float halfWidth = width / 2f;
    float halfHeight = height / 2f;

    // TODO: add threading to handle each pixel
    for (int y = 0; y < height; y++)
    {
      for (int x = 0; x < width; x++)
      {
        amplitude = 1;
        frequency = 1;
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

        if (noiseHeight > maxLocalNoiseHeight)
        {
          maxLocalNoiseHeight = noiseHeight;
        }
        else if (noiseHeight < minLocalNoiseHeight)
        {
          minLocalNoiseHeight = noiseHeight;
        }

        noiseMap[x, y] = noiseHeight;
      }
    }

    for (int y = 0; y < height; y++)
    {
      for (int x = 0; x < width; x++)
      {
        if (normalizeMode == NormalizeMode.Local)
        {
          noiseMap[x, y] = NormalizeValue(noiseMap[x, y], minLocalNoiseHeight, maxLocalNoiseHeight);
        }
        else
        {
          float normalizedHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
          noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
        }
      }
    }

    return noiseMap;
  }

  private static float NormalizeValue(float value, float min, float max)
  {
    return Mathf.InverseLerp(min, max, value);
  }
}
