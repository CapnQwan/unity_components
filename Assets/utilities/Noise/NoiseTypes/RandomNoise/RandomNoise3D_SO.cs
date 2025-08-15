using UnityEngine;

public class RandomNoise3D_SO : RandomNoise_SO
{
  /// <summary>
  /// The width of the noise map.
  /// </summary>
  [SerializeField]
  private int width;

  /// <summary>
  /// The height of the noise map.
  /// </summary>
  [SerializeField]
  private int height;

  /// <summary>
  /// The depth of the noise map.
  /// </summary>
  [SerializeField]
  private int depth;

  /// <summary>
  /// The seed for the random number generator used in noise generation.
  /// </summary>
  [SerializeField]
  private int seed;

  [SerializeField]
  private bool invert;

  /// <summary>
  /// The offset applied to the noise map.
  /// </summary>
  [SerializeField]
  private Vector3 offset;

  /// <summary>
  /// Gets the depth of the noise map.
  /// </summary>
  public int Depth => this.depth;

  /// <summary>
  /// Gets the offset applied to the noise map.
  /// </summary>
  public new Vector3 Offset => this.offset;

  /// <summary>
  /// Generates a noise map using the parameters defined in this scriptable object.
  /// </summary>
  /// <returns>A 2D array of floats representing the generated noise map.</returns>
  public virtual float[,,] GenerateNoiseMap(int width, int height, int depth)
  {
    return Noise.RandomNoise3D.GenerateRandomNoiseMap(width, height, depth, this);
  }

  /// <summary>
  /// Generates a noise map using the parameters defined in this scriptable object.
  /// </summary>
  /// <returns>A 2D array of floats representing the generated noise map.</returns>
  public virtual float[,,] GenerateNoiseMap(int width, int height, int depth, Vector3 offset)
  {
    return Noise.RandomNoise3D.GenerateRandomNoiseMap(width, height, depth, offset, this);
  }

  /// <summary>
  /// Generates a noise map using the parameters defined in this scriptable object.
  /// </summary>
  /// <returns>A 2D array of floats representing the generated noise map.</returns>
  public virtual float[,,] GenerateNoiseMap()
  {
    return Noise.RandomNoise3D.GenerateRandomNoiseMap(this);
  }
}
