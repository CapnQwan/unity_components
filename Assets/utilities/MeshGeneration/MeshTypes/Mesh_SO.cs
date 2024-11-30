using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for generating meshes.
/// </summary>
public class Mesh_SO : ScriptableObject
{
  /// <summary>
  /// Generates a simple quad mesh.
  /// </summary>
  /// <returns>A simple quad mesh.</returns>
  public virtual Mesh GenerateMesh()
  {
    return QuadMeshGenerator.GenerateSimpleQuad();
  }
}
