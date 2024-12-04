using UnityEngine;

public static class SphereMeshGenerator
{
  /// <summary>
  /// Cached instance of a standard Sphere mesh with 20 sections and a radius of 1f. Prevents generating the same mesh repeatedly.
  /// </summary>
  private static Mesh _cachedMesh;

  /// <summary>
  /// Generates a simple Sphere mesh with 20 sections and a radius of 1f.
  /// This method uses caching to avoid generating multiple identical meshes.
  /// </summary>
  /// <returns>A Unity <see cref="Mesh"/> representing a Spere with a radius of 1.</returns>
  public static Mesh GenerateSimpleCube()
  {
    if (!_cachedMesh)
    {
      _cachedMesh = GenerateSphere(1f, 20);
    }

    return _cachedMesh;
  }

  public static Mesh GenerateSphere(SphereMesh_SO meshScriptableObject)
  {
    return GenerateSphere(meshScriptableObject.Radius, meshScriptableObject.Sections);
  }

  public static Mesh GenerateSphere(float radius, int sections)
  {

    Mesh mesh = new Mesh
    {
      name = $"Sphere_{radius}R_{sections}S",
    };

    Vector3[] vertices = new Vector3[]
    {
        new Vector3(0,0,0),
        new Vector3(0, radius,0),
        new Vector3(radius, 0, 0),
    };

    int[] triangles = new int[]
    {
        // Front face
        2, 0, 1,
    };

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    //mesh.uv = uvs;
    //mesh.normals = normals;

    return mesh;
  }
}
