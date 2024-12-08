using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WNMathUtils
{
  public static float SqrDistance(Vector2Int a, Vector2Int b)
  {
    return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
  }

  public static float WrapFloat(float value, float min, float max)
  {
    if (value > max)
    {
      return min;
    }

    if (value < min)
    {
      return max;
    }

    return value;
  }

  public static int WrapInt(int value, int min, int max)
  {
    if (value > max)
    {
      return min;
    }

    if (value < min)
    {
      return max;
    }

    return value;
  }
}
