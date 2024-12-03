using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for storing parameters and generating a tri.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mesh/Tri", order = 1)]
public class TriMesh_SO : Mesh_SO
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
  /// Gets the width of the quad.
  /// </summary>
  public float Width => this.width;

  /// <summary>
  /// Gets the height of the quad.
  /// </summary>
  public float Height => this.height;

  /// <summary>
  /// Generates a simple quad mesh.
  /// </summary>
  /// <returns>A simple quad mesh.</returns>
  public override Mesh GenerateMesh()
  {
    return TriMeshGenerator.GenerateTri(this);
  }
}
