using Noise;
using UnityEngine;

/// <summary>
/// Provides static methods for generating and manipulating noise maps using various algorithms and scaling methods.
/// </summary>
public static class NoiseGeneration
{
  /// <summary>
  /// Generates a random noise map using parameters defined in a <see cref="RandomNoise_SO"/> ScriptableObject.
  /// </summary>
  /// <param name="noiseScriptableObject">The ScriptableObject containing parameters for the random noise map generation.</param>
  /// <returns>A 2D array of floats representing the generated noise map.</returns>
  public static float[,] GenerateRandomNoiseMap(RandomNoise_SO noiseScriptableObject)
  {
    return Noise.RandomNoise.GenerateRandomNoiseMap(noiseScriptableObject);
  }

  /// <summary>
  /// Generates a random noise map using a <see cref="RandomNoiseParameters"/> struct.
  /// </summary>
  /// <param name="noiseParameters">The parameters for the random noise map generation.</param>
  /// <returns>A 2D array of floats representing the generated noise map.</returns>
  public static float[,] GenerateRandomNoiseMap(RandomNoiseParameters noiseParameters)
  {
    return Noise.RandomNoise.GenerateRandomNoiseMap(noiseParameters);
  }

  /// <summary>
  /// Generates a Perlin noise map using parameters defined in a <see cref="PerlinNoise_SO"/> ScriptableObject.
  /// </summary>
  /// <param name="noiseScriptableObject">The ScriptableObject containing parameters for the Perlin noise map generation.</param>
  /// <returns>A 2D array of floats representing the generated Perlin noise map.</returns>
  public static float[,] GeneratePerlinNoiseMap(PerlinNoise_SO noiseScriptableObject)
  {
    return Noise.PerlinNoise.GeneratePerlinNoiseMap(noiseScriptableObject);
  }

  /// <summary>
  /// Generates a Perlin noise map using a <see cref="PerlinNoiseParameters"/> struct.
  /// </summary>
  /// <param name="noiseParameters">The parameters for the Perlin noise map generation.</param>
  /// <returns>A 2D array of floats representing the generated Perlin noise map.</returns>
  public static float[,] GeneratePerlinNoiseMap(PerlinNoiseParameters noiseParameters)
  {
    return Noise.PerlinNoise.GeneratePerlinNoiseMap(noiseParameters);
  }

  /// <summary>
  /// Generates a cellular noise map using parameters defined in a <see cref="CellularNoise_SO"/> ScriptableObject.
  /// </summary>
  /// <param name="noiseScriptableObject">The ScriptableObject containing parameters for the cellular noise map generation.</param>
  /// <returns>A 2D array of floats representing the generated cellular noise map.</returns>
  public static float[,] GenerateCellularNoiseMap(CellularNoise_SO noiseScriptableObject)
  {
    return Noise.CellularNoise.GenerateCellularNoiseMap(noiseScriptableObject);
  }

  /// <summary>
  /// Generates a cellular noise map using a <see cref="CellularNoiseParameters"/> struct.
  /// </summary>
  /// <param name="noiseParameters">The parameters for the cellular noise map generation.</param>
  /// <returns>A 2D array of floats representing the generated cellular noise map.</returns>
  public static float[,] GenerateCellularNoiseMap(CellularNoiseParameters noiseParameters)
  {
    return Noise.CellularNoise.GenerateCellularNoiseMap(noiseParameters);
  }

  /// <summary>
  /// Scales a given noise map to a new size using bilinear interpolation.
  /// </summary>
  /// <param name="originalMap">The original 2D noise map to be scaled.</param>
  /// <param name="targetWidth">The desired width of the scaled noise map.</param>
  /// <param name="targetHeight">The desired height of the scaled noise map.</param>
  /// <returns>A 2D array of floats representing the scaled noise map.</returns>
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
