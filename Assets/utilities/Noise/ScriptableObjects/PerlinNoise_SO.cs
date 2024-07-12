using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NormalizeMode
{
  Local,
  Global
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/PerlinNoise", order = 2)]
public class PerlinNoise_SO : Noise_SO
{
  [SerializeField]
  private float scale, lacunarity, persistance;
  public float Scale => scale;
  public float Lacunarity => lacunarity;
  public float Persistance => persistance;
  [SerializeField]
  private int octaves;
  public int Octaves => octaves;
  [SerializeField]
  private NormalizeMode normalizeMode;
  public NormalizeMode NormalizeMode => normalizeMode;
}
