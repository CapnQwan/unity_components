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
  }

  /// <summary>
  /// Generates a cellular noise map
  /// </summary>
  /// <param name="cellularNoiseData">The data for generating the cellular noise</param>
  /// <returns>A 2D array representing the generated noise map.</returns>
  /// <remarks>
  /// Cellular noise is generated by creating a grid and defining a point within each grid square
  /// the iterating over each point in the noise map and giving it a value based on the distance between
  /// the two closest points
  /// </remarks>
  public static float[,] GenerateCellularNoiseMap(CellularNoise_SO cellularNoiseData)
  {
    // Setup map and grid
    float[,] noiseMap = new float[cellularNoiseData.Width, cellularNoiseData.Height];
    Vector2Int[,] cellGrid = new Vector2Int[
      cellularNoiseData.CellCountX + 2,
      cellularNoiseData.CellCountY + 2
      ];


    // Generate the grids points
    int cellWidth = cellularNoiseData.Width / cellularNoiseData.CellCountX;
    int cellHeight = cellularNoiseData.Height / cellularNoiseData.CellCountY;

    for (int x = -1; x <= cellularNoiseData.CellCountX; x++)
    {
      for (int y = -1; y <= cellularNoiseData.CellCountY; y++)
      {
        Vector3 position = new Vector3((x + 2) / 0.539f, (y + 2) / 0.539f, 0);
        float xOffset = GenerateNoisePoint(position, cellularNoiseData.Seed, cellularNoiseData.Offset);
        float yOffset = GenerateNoisePoint(position, cellularNoiseData.Seed + 1, cellularNoiseData.Offset);
        int xPoint = Mathf.RoundToInt(cellWidth * xOffset);
        int yPoint = Mathf.RoundToInt(cellHeight * yOffset);

        cellGrid[x + 1, y + 1] = new Vector2Int(cellWidth * x + xPoint, cellHeight * y + yPoint);
      }
    }

    // Generate the values of the map based on these grid points
    for (int x = 0; x < cellularNoiseData.Width; x++)
    {
      for (int y = 0; y < cellularNoiseData.Height; y++)
      {
        int cellX = Mathf.FloorToInt(x / cellWidth) + 1;
        int cellY = Mathf.FloorToInt(y / cellHeight) + 1;

        float value = getPixelValueFromCells(
          cellGrid,
          cellX,
          cellY,
          x,
          y
          );

        noiseMap[x, y] = value;
      }
    }

    return noiseMap;
  }

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
