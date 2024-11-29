using UnityEngine;

public static class NoiseGeneration
{
  public static float[,] GenerateDomainWrapingNoiseMap()
  {
    return new float[1, 1];
  }

  // public static float[,] GenerateFractalNoise(NoiseLayer[] noiseLayers)
  // {
  //   int mapSizeX = 0;
  //   int mapSizeY = 0;

  //   for (int i = 0; i < noiseLayers.Length; i++)
  //   {
  //     if (noiseLayers[i].NoiseScriptableObject.Width > mapSizeX)
  //     {
  //       mapSizeX = noiseLayers[i].NoiseScriptableObject.Width;
  //     }
  //     if (noiseLayers[i].NoiseScriptableObject.Height > mapSizeY)
  //     {
  //       mapSizeY = noiseLayers[i].NoiseScriptableObject.Height;
  //     }
  //   }

  //   float[,] fractalNoiseMap = new float[mapSizeX, mapSizeY];

  //   for (int i = 0; i < noiseLayers.Length; i++)
  //   {
  //     float[,] noiseMap = GenerateNoiseMap(noiseLayers[i].NoiseScriptableObject);
  //     noiseMap = ScaleNoiseMap(
  //       noiseMap,
  //       mapSizeX,
  //       mapSizeY
  //       );

  //     for (int x = 0; x < mapSizeX; x++)
  //     {
  //       for (int y = 0; y < mapSizeY; y++)
  //       {
  //         fractalNoiseMap[x, y] += noiseMap[x, y] * noiseLayers[i].Amplitude / noiseLayers.Length;
  //       }
  //     }
  //   }

  //   return fractalNoiseMap;
  // }

  public static float[,] ScaleNoiseMap(float[,] originalMap, int targetWidth, int targetHeight)
  {
    float[,] scaledMap = new float[targetWidth, targetHeight];

    float xRatio = (float)(originalMap.GetLength(0) - 1) / targetWidth;
    float yRatio = (float)(originalMap.GetLength(1) - 1) / targetHeight;

    for (int x = 0; x < targetWidth; x++)
    {
      for (int y = 0; y < targetHeight; y++)
      {
        float gx = x * xRatio;
        float gy = y * yRatio;
        int gxi = (int)gx;
        int gyi = (int)gy;

        float c00 = originalMap[gxi, gyi];
        float c10 = originalMap[gxi + 1, gyi];
        float c01 = originalMap[gxi, gyi + 1];
        float c11 = originalMap[gxi + 1, gyi + 1];

        float tx = gx - gxi;
        float ty = gy - gyi;

        float lerp1 = Mathf.Lerp(c00, c10, tx);
        float lerp2 = Mathf.Lerp(c01, c11, tx);

        scaledMap[x, y] = Mathf.Lerp(lerp1, lerp2, ty);
      }
    }

    return scaledMap;
  }
}
