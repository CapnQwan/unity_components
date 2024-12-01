using Noise;
using UnityEngine;

/// <summary>
/// Temp.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/SimplexNoise", order = 2)]
public class SimplexNoise_SO : PerlinNoise_SO
{
  public override float[,] GenerateNoiseMap()
  {
    return Noise.SimplexNoise.GenerateSimplexNoiseMap(this);
  }
}
