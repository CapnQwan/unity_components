using UnityEngine;

public static class RandomNoise {
	  public static float[,] GenerateRandomNoiseMap(Noise_SO noiseScriptableObject)
  {
    float[,] noiseMap = new float[noiseScriptableObject.Width, noiseScriptableObject.Height];

    System.Random prng = new System.Random(noiseScriptableObject.Seed);

    float OffsetX = prng.Next(-100000, 100000) + noiseScriptableObject.Offset.x;
    float OffsetY = prng.Next(-100000, 100000) + noiseScriptableObject.Offset.y;
    Vector2 octaveOffsets = new Vector2(OffsetX, OffsetY);


    for (int y = 0; y < noiseScriptableObject.Height; y++)
    {
      for (int x = 0; x < noiseScriptableObject.Width; x++)
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
