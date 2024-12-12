using Noise;
using UnityEngine;

/// <summary>
/// A ScriptableObject for configuring and storing cellular noise parameters.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/CellularNoise", order = 3)]
public class CellularNoise_SO : RandomNoise_SO
{
  /// <summary>
  /// The number of cells along the X-axis for the cellular noise grid.
  /// </summary>
  [SerializeField]
  private int cellsCountX;

  /// <summary>
  /// The number of cells along the Y-axis for the cellular noise grid.
  /// </summary>
  [SerializeField]
  private int cellsCountY;

  /// <summary>
  /// Gets the number of cells along the X-axis for the cellular noise grid.
  /// </summary>
  public int CellCountX => this.cellsCountX;

  /// <summary>
  /// Gets the number of cells along the Y-axis for the cellular noise grid.
  /// </summary>
  public int CellCountY => this.cellsCountY;

  /// <summary>
  /// Generates a Cellular noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Perlin noise map.</returns>
  public override float[,] GenerateNoiseMap()
  {
    return Noise.CellularNoise.GenerateCellularNoiseMap(this);
  }

  /// <summary>
  /// Generates a Cellular noise map based on the configured parameters.
  /// </summary>
  /// <returns>A 2D array of float values representing the Perlin noise map.</returns>
  public override float[,] GenerateNoiseMap(int width, int height)
  {
    Debug.Log("GENERATE NOISE MAP WITH EXTERNAL WIDTH AND HEIGHT");
    return Noise.CellularNoise.GenerateCellularNoiseMap(width, height, this);
  }
}
