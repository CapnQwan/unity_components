using Noise;
using UnityEngine;

/// <summary>
/// A scriptable object for storing parameters used to generate a noise map.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/NoiseBase", order = 1)]
public class RandomNoise_SO : ScriptableObject
{
  /// <summary>
  /// The width of the noise map.
  /// </summary>
  [SerializeField]
  private readonly int width;

  /// <summary>
  /// The height of the noise map.
  /// </summary>
  [SerializeField]
  private readonly int height;

  /// <summary>
  /// The seed for the random number generator used in noise generation.
  /// </summary>
  [SerializeField]
  private readonly int seed;

  /// <summary>
  /// The offset applied to the noise map.
  /// </summary>
  [SerializeField]
  private Vector2 offset;

  /// <summary>
  /// Gets the width of the noise map.
  /// </summary>
  public int Width => this.width;

  /// <summary>
  /// Gets the height of the noise map.
  /// </summary>
  public int Height => this.height;

  /// <summary>
  /// Gets the seed for the random number generator used in noise generation.
  /// </summary>
  public int Seed => this.seed;

  /// <summary>
  /// Gets the offset applied to the noise map.
  /// </summary>
  public Vector2 Offset => this.offset;

  /// <summary>
  /// Generates a noise map using the parameters defined in this scriptable object.
  /// </summary>
  /// <returns>A 2D array of floats representing the generated noise map.</returns>
  public virtual float[,] GenerateNoiseMap()
  {
    return Noise.RandomNoise.GenerateRandomNoiseMap(this);
  }
}
