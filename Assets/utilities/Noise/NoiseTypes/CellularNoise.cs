using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellularNoise {

  public static float[,] GenerateCellularNoiseMap(CellularNoise_SO cellularNoiseData) {
		
	}

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

}
