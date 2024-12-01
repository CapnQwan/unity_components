namespace Noise
{
  using System;
  using System.Threading.Tasks;
  using Unity.VisualScripting;
  using UnityEditor;
  using UnityEngine;

  /// <summary>
  /// A static class for generating cellular noise maps.
  /// </summary>
  /// <remarks>
  /// Cellular noise is a procedural texture generation technique that divides the space into cells and assigns values
  /// based on distances or other relationships between points within these cells. This class provides methods for
  /// generating cellular noise maps with customizable parameters.
  /// </remarks>
  public static class CellularNoise
  {
    /// <summary>
    /// Default width of the noise map.
    /// </summary>
    public const int DEFAULT_WIDTH = 50;

    /// <summary>
    /// Default height of the noise map.
    /// </summary>
    public const int DEFAULT_HEIGHT = 50;

    /// <summary>
    /// Default number of cells in the x axis.
    /// </summary>
    public const int DEFAULT_CELL_COUNT_X = 5;

    /// <summary>
    /// Default number of cells in the Y axis.
    /// </summary>
    public const int DEFAULT_CELL_COUNT_Y = 5;

    /// <summary>
    /// Default seed for the random number generator.
    /// </summary>
    public const int DEFAULT_SEED = 0;

    /// <summary>Default offset applied to the noise map coordinates.</summary>
    public static readonly Vector2 DEFAULT_OFFSET = new Vector2(0f, 0f);

    /// <summary>
    /// Generates a cellular noise map using parameters defined in a <see cref="CellularNoise_SO"/> scriptable object.
    /// </summary>
    /// <param name="cellularNoiseData">The scriptable object containing cellular noise parameters.</param>
    /// <returns>A 2D array of float values representing the generated cellular noise map.</returns>
    public static float[,] GenerateCellularNoiseMap(CellularNoise_SO cellularNoiseData)
    {
      return GenerateCellularNoiseMap(
          cellularNoiseData.Width,
          cellularNoiseData.Height,
          cellularNoiseData.CellCountX,
          cellularNoiseData.CellCountY,
          cellularNoiseData.Seed,
          cellularNoiseData.Offset);
    }

    /// <summary>
    /// Generates a cellular noise map using parameters defined in a <see cref="CellularNoiseParameters"/> structure.
    /// </summary>
    /// <param name="cellularNoiseParameters">The structure containing cellular noise parameters.</param>
    /// <returns>A 2D array of float values representing the generated cellular noise map.</returns>
    public static float[,] GenerateCellularNoiseMap(CellularNoiseParameters cellularNoiseParameters)
    {
      return GenerateCellularNoiseMap(
          cellularNoiseParameters.Width,
          cellularNoiseParameters.Height,
          cellularNoiseParameters.CellCountX,
          cellularNoiseParameters.CellCountY,
          cellularNoiseParameters.Seed,
          cellularNoiseParameters.Offset);
    }

    /// <summary>
    /// Generates a cellular noise map based on the specified parameters.
    /// </summary>
    /// <param name="width">The width of the noise map.</param>
    /// <param name="height">The height of the noise map.</param>
    /// <param name="cellCountX">The number of cells along the X-axis.</param>
    /// <param name="cellCountY">The number of cells along the Y-axis.</param>
    /// <param name="seed">The seed for the random number generator, ensuring deterministic results.</param>
    /// <returns>A 2D array of float values representing the generated cellular noise map.</returns>
    public static float[,] GenerateCellularNoiseMap(int width, int height, int cellCountX, int cellCountY, int seed, Vector2 offset)
    {
      System.Random prng = new System.Random(seed);

      // TODO: improve performance
      // Setup map and grid
      float[,] noiseMap = new float[width, height];
      Vector2Int[,] cellGrid = new Vector2Int[cellCountX + 2, cellCountY + 2];

      // Generate the grids points
      int cellWidth = width / cellCountX;
      int cellHeight = height / cellCountY;

      for (int x = -1; x <= cellCountX; x++)
      {
        for (int y = -1; y <= cellCountY; y++)
        {
          float xOffset = (float)prng.NextDouble() + offset.x;
          float yOffset = (float)prng.NextDouble() + offset.y;
          int xPoint = Mathf.RoundToInt(cellWidth * xOffset);
          int yPoint = Mathf.RoundToInt(cellHeight * yOffset);

          cellGrid[x + 1, y + 1] = new Vector2Int((cellWidth * x) + xPoint, (cellHeight * y) + yPoint);
        }
      }

      // Generate the values of the map based on these grid points
      Parallel.For(0, width, x =>
      {
        for (int y = 0; y < height; y++)
        {
          int cellX = Mathf.FloorToInt(x / cellWidth) + 1;
          int cellY = Mathf.FloorToInt(y / cellHeight) + 1;
          noiseMap[x, y] = GetPixelValueFromCells(cellGrid, cellX, cellY, x, y);
        }
      });

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
    private static float GetPixelValueFromCells(
      Vector2Int[,] gridcells,
      int cellPositionX,
      int cellPositionY,
      int pointPositionX,
      int pointPositionY)
    {
      HeapCompareable<CellNeighbour> neighboursHeap = new HeapCompareable<CellNeighbour>();

      Vector2Int cell = gridcells[cellPositionX, cellPositionY];
      Vector2Int point = new Vector2Int(pointPositionX, pointPositionY);

      for (int x = -1, i = 0; x <= 1; x++)
      {
        for (int y = -1; y <= 1; y++, i++)
        {
          if (x == 0 && y == 0)
          {
            i--;
            continue;
          }

          Vector2Int neighbour = gridcells[cellPositionX + x, cellPositionY + y];
          float distance = WNMathUtils.SqrDistance(neighbour, point);
          CellNeighbour cellNeighbour = new CellNeighbour(neighbour, distance);
          neighboursHeap.Insert(cellNeighbour);
        }
      }

      CellNeighbour closestNeighbor = neighboursHeap.ExtractMin();
      CellNeighbour secondClosestNeighbor = neighboursHeap.ExtractMin();
      float cellDistance = WNMathUtils.SqrDistance(cell, point);

      float[] distances = { closestNeighbor.Distance, secondClosestNeighbor.Distance, cellDistance };
      Array.Sort(distances);

      float minDistance = Mathf.Min(distances[0], distances[1]);
      float maxDistance = Mathf.Max(distances[0], distances[1]);

      return minDistance / maxDistance;
    }
  }

  /// <summary>
  /// Encapsulates the parameters used for generating a cellular noise map.
  /// </summary>
  public struct CellularNoiseParameters
  {
    /// <summary>
    /// Gets the width of the noise map.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the noise map.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the number of cells in the x axis.
    /// </summary>
    public int CellCountX { get; }

    /// <summary>
    /// Gets the number of cells in the y axis.
    /// </summary>
    public int CellCountY { get; }

    /// <summary>
    /// Gets the seed for the random number generator.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Gets the offset applied to the noise map.
    /// </summary>
    public Vector2 Offset { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CellularNoiseParameters"/> struct with specified values.
    /// </summary>
    /// <param name="width">The width of the noise map. Defaults to <see cref="CellularNoise.DEFAULT_WIDTH"/>.</param>
    /// <param name="height">The height of the noise map. Defaults to <see cref="CellularNoise.DEFAULT_HEIGHT"/>.</param>
    /// <param name="cellCountX">The number of cells on the x axis that it generates based off <see cref="CellularNoise.DEFAULT_CELL_COUNT_X"/>.</param>
    /// <param name="cellCountY">The number of cells on the y axis that it generates based off <see cref="CellularNoise.DEFAULT_CELL_COUNT_Y"/>.</param>
    /// <param name="seed">The seed for the random number generator. Defaults to <see cref="CellularNoise.DEFAULT_SEED"/>.</param>
    /// <param name="offset">Offset to apply to the map coordinates.</param>
    public CellularNoiseParameters(
        int width = CellularNoise.DEFAULT_WIDTH,
        int height = CellularNoise.DEFAULT_HEIGHT,
        int cellCountX = CellularNoise.DEFAULT_CELL_COUNT_X,
        int cellCountY = CellularNoise.DEFAULT_CELL_COUNT_Y,
        int seed = CellularNoise.DEFAULT_SEED,
        Vector2? offset = null)
    {
      this.Width = width;
      this.Height = height;
      this.CellCountX = cellCountX;
      this.CellCountY = cellCountY;
      this.Seed = seed;
      this.Offset = offset ?? CellularNoise.DEFAULT_OFFSET;
    }
  }

  struct CellNeighbour : IComparable<CellNeighbour>
  {
    public Vector2Int Position { get; }
    public float Distance { get; }

    public CellNeighbour(Vector2Int position, float distance)
    {
      Position = position;
      Distance = distance;
    }

    public int CompareTo(CellNeighbour other)
    {
      return Distance.CompareTo(other.Distance);
    }
  }
}
