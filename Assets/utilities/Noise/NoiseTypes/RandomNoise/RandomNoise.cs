using UnityEngine;

public static class RandomNoise
{

  public static float[,] GenerateRandomNoiseMap(Noise_SO noiseScriptableObject)
  {
    return GenerateRandomNoiseMap(noiseScriptableObject.Width, noiseScriptableObject.Height, noiseScriptableObject.Seed, noiseScriptableObject.Offset);
  }

  public static float[,] GenerateRandomNoiseMap(int width, int height, int seed, Vector2 offset)
  {
    float[,] noiseMap = new float[width, height];

    System.Random prng = new System.Random(seed);

    float randomOffsetX = prng.Next(-100000, 100000);
    float randomOffsetY = prng.Next(-100000, 100000);
    float offsetX = randomOffsetX + offset.x;
    float offsetY = randomOffsetY + offset.y;

    Vector2 octaveOffsets = new Vector2(offsetX, offsetY);

    for (int y = 0; y < height; y++)
    {
      for (int x = 0; x < width; x++)
      {

        float noiseHeight = 0;

        float sampleX = (x + octaveOffsets.x) / 0.539f;
        float sampleY = (y + octaveOffsets.y) / 0.539f;

        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
        noiseHeight += perlinValue;

        noiseMap[x, y] = noiseHeight;
      }
    }
    return noiseMap;
  }
}
