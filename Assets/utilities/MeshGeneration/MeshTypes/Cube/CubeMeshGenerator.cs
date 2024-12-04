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
        // Front face
        new Vector3(-halfWidth, -halfHeight, -halfDepth), // Bottom-left
        new Vector3(halfWidth, -halfHeight, -halfDepth),  // Bottom-right
        new Vector3(-halfWidth, halfHeight, -halfDepth),  // Top-left
        new Vector3(halfWidth, halfHeight, -halfDepth),   // Top-right

        // Back face
        new Vector3(-halfWidth, -halfHeight, halfDepth),  // Bottom-left
        new Vector3(halfWidth, -halfHeight, halfDepth),   // Bottom-right
        new Vector3(-halfWidth, halfHeight, halfDepth),   // Top-left
        new Vector3(halfWidth, halfHeight, halfDepth),    // Top-right
    };

    // Define the triangles for the quad.
    int[] triangles = new int[]
    {
        // Front face
        0, 2, 1,
        1, 2, 3,

        // Back face
        5, 6, 4,
        5, 7, 6,

        // Left face
        4, 6, 0,
        0, 6, 2,

        // Right face
        1, 3, 5,
        5, 3, 7,

        // Top face
        2, 6, 3,
        3, 6, 7,

        // Bottom face
        4, 0, 5,
        5, 0, 1
    };

    // Define the UV mapping coordinates for the quad.
    Vector2[] uvs = new Vector2[]
    {
        // Front face
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(0, 1), new Vector2(1, 1),

        // Back face
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(0, 1), new Vector2(1, 1),

        // Left face
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(0, 1), new Vector2(1, 1),

        // Right face
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(0, 1), new Vector2(1, 1),
    };

    // Define the normals for the quad (all facing backward).
    Vector3[] normals = new Vector3[]
    {
        // Front face
        Vector3.one * -1f, Vector3.one * -1f, Vector3.one * -1f, Vector3.one * -1f,

        // Back face
        Vector3.one, Vector3.one, Vector3.one, Vector3.one,
    };

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;
    mesh.normals = normals;

    return mesh;
  }
}
