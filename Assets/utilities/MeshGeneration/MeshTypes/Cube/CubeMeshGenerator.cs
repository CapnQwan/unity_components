using System;
using UnityEngine;

public static class CubeMeshGenerator
{
  /// <summary>
  /// Cached instance of a standard Cube mesh (1x1x1 size). Prevents generating the same mesh repeatedly.
  /// </summary>
  private static Mesh _cachedMesh;

  /// <summary>
  /// Generates a simple Cube mesh with default dimensions of 1x1.
  /// This method uses caching to avoid generating multiple identical meshes.
  /// </summary>
  /// <returns>A Unity <see cref="Mesh"/> representing a 1x1x1 Cube.</returns>
  public static Mesh GenerateSimpleCube()
  {
    if (!_cachedMesh)
    {
      _cachedMesh = GenerateCube(1f, 1f, 1f);
    }

    return _cachedMesh;
  }

  public static Mesh GenerateCube(CubeMesh_SO meshScriptableObject)
  {
    return GenerateCube(meshScriptableObject.Width, meshScriptableObject.Height, meshScriptableObject.Depth);
  }

  public static Mesh GenerateCube(float width, float height, float depth)
  {
    if (width <= 0 || height <= 0 || depth <= 0)
    {
      throw new ArgumentException("Width and height must be greater than 0.");
    }

    Mesh mesh = new Mesh
    {
      name = $"Tri_{width}x{height}",
    };

    float halfWidth = width * 0.5f;
    float halfHeight = height * 0.5f;
    float halfDepth = depth * 0.5f;

    Vector3[] vertices = new Vector3[]
    {
      new Vector3(-halfWidth, -halfHeight, -halfDepth), // Front-Bottom-left
      new Vector3(halfWidth, -halfHeight, -halfDepth),  // Front-Bottom-right
      new Vector3(-halfWidth, halfHeight, -halfDepth),  // Front-Top-left
      new Vector3(halfWidth, halfHeight, -halfDepth),  // Front-Top-left
      new Vector3(-halfWidth, -halfHeight, halfDepth), // Back-Bottom-left
      new Vector3(halfWidth, -halfHeight, halfDepth),  // Back-Bottom-right
      new Vector3(-halfWidth, halfHeight, halfDepth),  // Back-Top-left
      new Vector3(halfWidth, halfHeight, halfDepth),  // Back-Top-left
    };

    // Define the triangles for the quad.
    int[] triangles = new int[]
    {
      0, 2, 1, // Bottom-left triangle
      1, 2, 3, // Top-right triangle
      4, 2, 0,
      6, 2, 4,
      6, 3, 2,
      7, 3, 6,
      7, 1, 3,
      7, 5, 1,
    };

    // Define the UV mapping coordinates for the quad.
    Vector2[] uvs = new Vector2[]
    {
      new Vector2(0, 0), // Bottom-left
      new Vector2(1, 0), // Bottom-right
      new Vector2(0, 1), // Top-left
      new Vector2(1, 1), // Top-right
    };

    // Define the normals for the quad (all facing backward).
    Vector3[] normals = new Vector3[]
    {
        Vector3.back,
        Vector3.back,
        Vector3.back,
        Vector3.back,
    };

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;
    mesh.normals = normals;

    return mesh;
  }
}
