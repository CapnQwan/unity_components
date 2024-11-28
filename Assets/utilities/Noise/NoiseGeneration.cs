using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGeneration
{

  /// <summary>
  /// Generates a noise map based on the type of scriptable object passed to it
  /// </summary>
  /// <param name="noiseScriptableObject">The data relating to the noise that will be generated</param>
  /// <returns>A 2D array representing the generated noise map.</returns>
  public static float[,] GenerateNoiseMap(Noise_SO noiseScriptableObject)
  {
    if (noiseScriptableObject is CellularNoise_SO cellularNoiseData)
    {
      return GenerateCellularNoiseMap(cellularNoiseData);
    }
    if (noiseScriptableObject is PerlinNoise_SO perlinNoiseData)
    {
      return GeneratePerlinNoiseMap(perlinNoiseData);
    }
    if (noiseScriptableObject is SimplexNoise_SO simplexNoiseData)
    {
      return GenerateSimplexNoiseMap(simplexNoiseData);
    }
    if (noiseScriptableObject is ValueNoise_SO valueNoiseData)
    {
      return GenerateValueNoiseMap(valueNoiseData);
    }
    if (noiseScriptableObject is BlueNoise_SO blueNoiseData)
    {
      return GenerateBlueNoiseMap(blueNoiseData);
    }
    if (noiseScriptableObject is PinkNoise_SO pinkNoiseData)
    {
      return GeneratePinkNoiseMap(pinkNoiseData);
    }
    if (noiseScriptableObject is TurbulenceNoise_SO turbulenceNoiseData)
    {
      return GenerateTurbulenceNoiseMap(turbulenceNoiseData);
    }
    if (noiseScriptableObject is RidgedMultiFractalNoise_SO ridgedNoiseData)
    {
      return GenerateRidgedMultiFractalNoiseMap(ridgedNoiseData);
    }
    if (noiseScriptableObject is WavletNoise_SO wavletNoiseData)
    {
      return GenerateWavletNoiseMap(wavletNoiseData);
    }

    return GenerateRandomNoiseMap(noiseScriptableObject);

  // This function finds the two closest cells and returns a value
  // based on the points distance between the two cells


  /// <summary>
  /// Get the value of a pixel based on it's two closest neighbors
  /// </summary>
  /// <param name="gridcells">The array of grid cells positions</param>
  /// <param name="cellPositionX">The horizontal position of the cell in the grid</param>
  /// <param name="cellPositionY">The vertical position of the cell in the grid</param>
  /// <param name="pointPositionX">The horizontal position of the point or pixel</param>
  /// <param name="pointPositionY">The vertical position of the point or pixel</param>
  /// <returns>
  /// A float value based on the distance to the closest point and the second closest point
  /// </returns>
  /// <remarks>
  /// This function finds the cell and iterates over all of its neighbors ro find the closest
  /// two cells then generates a value based on the distance to the closest cell compared to 
  /// the second closest cell
  /// </remarks>
  private static float getPixelValueFromCells(
    Vector2Int[,] gridcells,
    int cellPositionX,
    int cellPositionY,
    int pointPositionX,
    int pointPositionY
    )
  {
    //THis 
    Vector2Int cell = gridcells[cellPositionX, cellPositionY];
    Vector2Int[] closestNeighbors = new Vector2Int[3];
    Vector2Int[] neighbors = new Vector2Int[8];
    Vector2Int point = new Vector2Int(pointPositionX, pointPositionY);


    for (int i = 0; i < closestNeighbors.Length; i++)
    {
      closestNeighbors[i] = new Vector2Int(-1000000, -1000000);
    }

    for (int x = -1, i = 0; x <= 1; x++)
    {
      for (int y = -1; y <= 1; y++, i++)
      {
        if (x == 0 && y == 0)
        {
          i--;
          continue;
        }

        neighbors[i] = gridcells[cellPositionX + x, cellPositionY + y];

        for (int n = 0; n < closestNeighbors.Length; n++)
        {
          if (Vector2Int.Distance(neighbors[i], point) < Vector2Int.Distance(closestNeighbors[n], point))
          {
            for (int cn = n; cn < closestNeighbors.Length - 1; cn++)
            {
              closestNeighbors[cn + 1] = closestNeighbors[cn];
            }
            closestNeighbors[n] = neighbors[i];
            break;
          }
        }
      }
    }
    float neigborCellDistanceA = Vector2Int.Distance(closestNeighbors[0], point);
    float neigborCellDistanceB = Vector2Int.Distance(closestNeighbors[1], point);
    float cellDistance = Vector2Int.Distance(cell, point);

    float[] distances = { neigborCellDistanceA, neigborCellDistanceB, cellDistance };
    Array.Sort(distances);

    float minDistance = Mathf.Min(distances[0], distances[1]);
    float maxDistance = Mathf.Max(distances[0], distances[1]);

    return minDistance / maxDistance;
  }



  public static float[,] GenerateSimplexNoiseMap(SimplexNoise_SO noiseScriptableObject)
  {
    return new float[1, 1];
  }

  public static float[,] GenerateValueNoiseMap(ValueNoise_SO noiseScriptableObject)
  {
    return new float[1, 1];
  }

  public static float[,] GenerateBlueNoiseMap(BlueNoise_SO noiseScriptableObject)
  {
    return new float[1, 1];
  }

  public static float[,] GeneratePinkNoiseMap(PinkNoise_SO noiseScriptableObject)
  {
    return new float[1, 1];
  }

  public static float[,] GenerateTurbulenceNoiseMap(TurbulenceNoise_SO noiseScriptableObject)
  {
    return new float[1, 1];
  }

  public static float[,] GenerateRidgedMultiFractalNoiseMap(RidgedMultiFractalNoise_SO noiseScriptableObject)
  {
    return new float[1, 1];
  }

  public static float[,] GenerateWavletNoiseMap(WavletNoise_SO noiseScriptableObject)
  {
    return new float[1, 1];
  }

  public static float[,] GenerateDomainWrapingNoiseMap()
  {
    return new float[1, 1];
  }


  public static float GenerateNoisePoint(int x, int y, int Seed, Vector2 Offset)
  {
    return GetNoisePoint(x, y, Seed, Offset);
  }

  public static float GenerateNoisePoint(Vector2 pos, int Seed, Vector2 Offset)
  {
    return GetNoisePoint(pos.x, pos.y, Seed, Offset);
  }

  public static float GenerateNoisePoint(Vector3 pos, int Seed, Vector2 Offset)
  {
    return GetNoisePoint(pos.x, pos.y, Seed, Offset);
  }

  public static float GetNoisePoint(float x, float y, int Seed, Vector2 Offset)
  {
    System.Random prng = new System.Random(Seed);

    float OffsetX = prng.Next(-100000, 100000) + Offset.x;
    float OffsetY = prng.Next(-100000, 100000) + Offset.y;

    float sampleX = (x + OffsetX);
    float sampleY = (y + OffsetY);

    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
    return perlinValue;
  }

  public static float[,] GenerateFractalNoise(NoiseLayer[] noiseLayers)
  {
    int mapSizeX = 0;
    int mapSizeY = 0;

    for (int i = 0; i < noiseLayers.Length; i++)
    {
      if (noiseLayers[i].NoiseScriptableObject.Width > mapSizeX)
      {
        mapSizeX = noiseLayers[i].NoiseScriptableObject.Width;
      }
      if (noiseLayers[i].NoiseScriptableObject.Height > mapSizeY)
      {
        mapSizeY = noiseLayers[i].NoiseScriptableObject.Height;
      }
    }

    float[,] fractalNoiseMap = new float[mapSizeX, mapSizeY];

    for (int i = 0; i < noiseLayers.Length; i++)
    {
      float[,] noiseMap = GenerateNoiseMap(noiseLayers[i].NoiseScriptableObject);
      noiseMap = ScaleNoiseMap(
        noiseMap,
        noiseLayers[i].NoiseScriptableObject.Width,
        noiseLayers[i].NoiseScriptableObject.Height,
        mapSizeX,
        mapSizeY
        );

      for (int x = 0; x < mapSizeX; x++)
      {
        for (int y = 0; y < mapSizeY; y++)
        {
          fractalNoiseMap[x, y] += noiseMap[x, y] * noiseLayers[i].Amplitude / noiseLayers.Length;
        }
      }
    }

    return fractalNoiseMap;
  }

  public static float[,] ScaleNoiseMap(float[,] originalMap, int originalWidth, int originalHeight, int targetWidth, int targetHeight)
  {
    float[,] scaledMap = new float[targetWidth, targetHeight];

    float xRatio = (float)(originalWidth - 1) / targetWidth;
    float yRatio = (float)(originalHeight - 1) / targetHeight;

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
