using UnityEngine;

public static class NoiseUtils
{
  public static float[,,] Convert2DTo3D(float[,] noiseMap, int height)
  {
    int width = noiseMap.GetLength(0);
    int depth = noiseMap.GetLength(1);
    float[,,] noise3D = new float[width, height, depth];

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        for (int z = 0; z < depth; z++)
        {
          noise3D[x, y, z] = Mathf.Clamp((height * noiseMap[x, z]) - y, 0, 1);
        }
      }
    }

    return noise3D;
  }
}
