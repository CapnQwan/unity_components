using System;
using UnityEngine;

public static class CellularNoise
{

  public static float[,] GenerateCellularNoiseMap(CellularNoise_SO cellularNoiseData)
  {
    return GenerateCellularNoiseMap(
      cellularNoiseData.Width,
      cellularNoiseData.Height,
      cellularNoiseData.CellCountX,
      cellularNoiseData.CellCountY,
      cellularNoiseData.Seed);
  }

  public static float[,] GenerateCellularNoiseMap(int width, int height, int cellCountX, int cellCountY, int seed)
  {
    // Setup map and grid
    float[,] noiseMap = new float[width, height];
    Vector2Int[,] cellGrid = new Vector2Int[
      cellCountX + 2,
      cellCountY + 2
    ];

    // Generate the grids points
    int cellWidth = width / cellCountX;
    int cellHeight = height / cellCountY;

    for (int x = -1; x <= cellCountX; x++)
    {
      for (int y = -1; y <= cellCountY; y++)
      {
        float xOffset = RandomUtils.GetRandomSeededFloat(seed);
        float yOffset = RandomUtils.GetRandomSeededFloat(seed);
        int xPoint = Mathf.RoundToInt(cellWidth * xOffset);
        int yPoint = Mathf.RoundToInt(cellHeight * yOffset);

        cellGrid[x + 1, y + 1] = new Vector2Int((cellWidth * x) + xPoint, (cellHeight * y) + yPoint);
      }
    }

    // Generate the values of the map based on these grid points
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
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
  /// Get the value of a pixel based on it's two closest neighbors.
  /// </summary>
  /// <param name="gridcells">The array of grid cells positions.</param>
  /// <param name="cellPositionX">The horizontal position of the cell in the grid.</param>
  /// <param name="cellPositionY">The vertical position of the cell in the grid.</param>
  /// <param name="pointPositionX">The horizontal position of the point or pixel.</param>
  /// <param name="pointPositionY">The vertical position of the point or pixel.</param>
  /// <returns>
  /// A float value based on the distance to the closest point and the second closest point.
  /// </returns>
  /// <remarks>
  /// This function finds the cell and iterates over all of its neighbors ro find the closest
  /// two cells then generates a value based on the distance to the closest cell compared to
  /// the second closest cell.
  /// </remarks>
  private static float getPixelValueFromCells(
    Vector2Int[,] gridcells,
    int cellPositionX,
    int cellPositionY,
    int pointPositionX,
    int pointPositionY)
  {
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
}
