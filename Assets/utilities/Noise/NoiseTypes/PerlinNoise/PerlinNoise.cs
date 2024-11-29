using UnityEngine;

public static class PerlinNoise {
	  public static float[,] GeneratePerlinNoiseMap(PerlinNoise_SO noiseScriptableObject)
  {
    float[,] noiseMap = new float[noiseScriptableObject.Width, noiseScriptableObject.Height];

    System.Random prng = new System.Random(noiseScriptableObject.Seed);
    Vector2[] octaveOffsets = new Vector2[noiseScriptableObject.Octaves];

    float maxPossibleHeight = 0;
    float amplitude = 1;
    float frequency = 1;

    for (int i = 0; i < noiseScriptableObject.Octaves; i++)
    {
      float OffsetX = prng.Next(-100000, 100000) + noiseScriptableObject.Offset.x;
      float OffsetY = prng.Next(-100000, 100000) - noiseScriptableObject.Offset.y;
      octaveOffsets[i] = new Vector2(OffsetX, OffsetY);

      maxPossibleHeight += amplitude;
      amplitude *= noiseScriptableObject.Persistance;
    }

    float scale = noiseScriptableObject.Scale;

    if (scale <= 0)
    {
      scale = 0.0001f;
    }

    float maxLocalNoiseHeight = float.MinValue;
    float minLocalNoiseHeight = float.MaxValue;

    float halfWidth = noiseScriptableObject.Width / 2f;
    float halfHeight = noiseScriptableObject.Height / 2f;

    for (int y = 0; y < noiseScriptableObject.Height; y++)
    {
      for (int x = 0; x < noiseScriptableObject.Width; x++)
      {
        amplitude = 1;
        frequency = 1;
        float noiseHeight = 0;


        for (int i = 0; i < noiseScriptableObject.Octaves; i++)
        {
          float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
          float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

          float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
          noiseHeight += perlinValue * amplitude;

          amplitude *= noiseScriptableObject.Persistance;
          frequency *= noiseScriptableObject.Lacunarity;
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

    for (int y = 0; y < noiseScriptableObject.Height; y++)
    {
      for (int x = 0; x < noiseScriptableObject.Width; x++)
      {
        if (noiseScriptableObject.NormalizeMode == NormalizeMode.Local)
        {
          noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
        }
        else
        {
          float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
          noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
        }
      }
    }

    return noiseMap;
  }
}
