using System;
using UnityEngine;
public class TriMeshGenerator
{
  /// <summary>
  /// Cached instance of a standard tri mesh (1x1 size). Prevents generating the same mesh repeatedly.
  /// </summary>
  private static Mesh _cachedMesh;

  /// <summary>
  /// Generates a simple tri mesh with default dimensions of 1x1.
  /// This method uses caching to avoid generating multiple identical meshes.
  /// </summary>
  /// <returns>A Unity <see cref="Mesh"/> representing a 1x1 tri.</returns>
  public static Mesh GenerateSimpleQuad()
  {
    if (!_cachedMesh)
    {
      _cachedMesh = GenerateTri(1f, 1f);
    }

    return _cachedMesh;
  }

  public static Mesh GenerateTri(TriMesh_SO meshScriptableObject)
  {
    return GenerateTri(meshScriptableObject.Width, meshScriptableObject.Height);
  }

  /// <summary>
  /// Generates a tri mesh with the specified width and height.
  /// </summary>
  /// <param name="width">The width of the quad. Must be greater than 0.</param>
  /// <param name="height">The height of the quad. Must be greater than 0.</param>
  /// <returns>A Unity <see cref="Mesh"/> representing the specified tri.</returns>
  /// <exception cref="ArgumentException">Thrown when width or height is less than or equal to 0.</exception>
  public static Mesh GenerateTri(float width, float height)
  {
    if (width <= 0 || height <= 0)
    {
      throw new ArgumentException("Width and height must be greater than 0.");
    }

    Mesh mesh = new Mesh
    {
      name = $"Tri_{width}x{height}",
    };

    float halfWidth = width * 0.5f;
    float halfHeight = height * 0.5f;

    // Define the vertices of the quad.
    Vector3[] vertices = new Vector3[]
    {
      new Vector3(-halfWidth, -halfHeight, 0), // Bottom-left
      new Vector3(halfWidth, -halfHeight, 0),  // Bottom-right
      new Vector3(-halfWidth, halfHeight, 0),  // Top-left
    };

    // Define the triangles for the quad.
    int[] triangles = new int[]
    {
      0, 2, 1, // Bottom-left triangle
    };

    // Define the UV mapping coordinates for the quad.
    Vector2[] uvs = new Vector2[]
    {
      new Vector2(0, 0), // Bottom-left
      new Vector2(1, 0), // Bottom-right
      new Vector2(0, 1), // Top-left
    };

    // Define the normals for the quad (all facing backward).
    Vector3[] normals = new Vector3[]
    {
        Vector3.back,
        Vector3.back,
        Vector3.back,
    };

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;
    mesh.normals = normals;

    // Recalculate normals for proper lighting effects.
    mesh.RecalculateNormals();

    return mesh;
  }
}
