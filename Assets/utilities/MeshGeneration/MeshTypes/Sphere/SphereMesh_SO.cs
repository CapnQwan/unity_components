using UnityEngine;

/// <summary>
/// A scriptable object for storing parameters and generating a sphere.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mesh/Sphere", order = 3)]
public class SphereMesh_SO : Mesh_SO
{
  /// <summary>
  /// The radius of the sphere.
  /// </summary>
  [SerializeField]
  private float radius = 1f;

  /// <summary>
  /// The number of sections (Tris) that make up the sphere.
  /// </summary>
  [SerializeField]
  private int sections = 1;

  /// <summary>
  /// Gets the radius of the sphere.
  /// </summary>
  public float Radius => this.radius;

  /// <summary>
  /// Gets number of sections (Tris) that make up the sphere.
  /// </summary>
  public int Sections => this.sections;

  /// <summary>
  /// Generates a simple quad mesh.
  /// </summary>
  /// <returns>A simple quad mesh.</returns>
  public override Mesh GenerateMesh()
  {
    return SphereMeshGenerator.GenerateSphere(this);
  }
}
