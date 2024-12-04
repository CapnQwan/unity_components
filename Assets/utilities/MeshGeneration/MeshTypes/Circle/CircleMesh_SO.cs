using UnityEngine;

/// <summary>
/// A scriptable object for storing parameters and generating a circle.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mesh/Circle", order = 3)]
public class CircleMesh_SO : Mesh_SO
{
  /// <summary>
  /// The radius of the circle.
  /// </summary>
  [SerializeField]
  private float radius = 1f;

  /// <summary>
  /// The number of sections (Tris) that make up the circle.
  /// </summary>
  [SerializeField]
  private int sections = 3;

  /// <summary>
  /// Gets the radius of the circle.
  /// </summary>
  public float Radius => this.radius;

  /// <summary>
  /// Gets number of sections (Tris) that make up the circle.
  /// </summary>
  public int Sections => this.sections;

  /// <summary>
  /// Ensures sections value cannot be below 3.
  /// </summary>
  private void OnValidate()
  {
    if (radius <= 0)
    {
      radius = 0.01f;
    }
    if (sections < 3)
    {
      sections = 3;
    }
  }

  /// <summary>
  /// Generates a simple quad mesh.
  /// </summary>
  /// <returns>A simple quad mesh.</returns>
  public override Mesh GenerateMesh()
  {
    return CircleMeshGenerator.GenerateCircle(this);
  }
}
