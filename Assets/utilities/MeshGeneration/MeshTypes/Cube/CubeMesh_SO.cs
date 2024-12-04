using UnityEngine;

/// <summary>
/// A scriptable object for storing parameters and generating a cube.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mesh/Cube", order = 4)]
public class CubeMesh_SO : Mesh_SO
{
  /// <summary>
  /// The width of the quad.
  /// </summary>
  [SerializeField]
  private float width = 1f;

  /// <summary>
  /// The height of the quad.
  /// </summary>
  [SerializeField]
  private float height = 1f;

  /// <summary>
  /// The depth of the quad.
  /// </summary>
  [SerializeField]
  private float depth = 1f;

  /// <summary>
  /// Gets the width of the quad.
  /// </summary>
  public float Width => this.width;

  /// <summary>
  /// Gets the height of the quad.
  /// </summary>
  public float Height => this.height;

  /// <summary>
  /// Gets the depth of the quad.
  /// </summary>
  public float Depth => this.depth;

  /// <summary>
  /// Generates a simple quad mesh.
  /// </summary>
  /// <returns>A simple quad mesh.</returns>
  public override Mesh GenerateMesh()
  {
    return CubeMeshGenerator.GenerateCube(this);
  }
}
