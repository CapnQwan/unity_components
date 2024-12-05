using System;
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
  public static Mesh GenerateSimpleSphere()
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
    if (radius <= 0)
    {
      throw new ArgumentException("Sections must be greater than 0.");
    }
    if (sections < 3)
    {
      throw new ArgumentException("Sections must be 3 or greater.");
    }

    float FullCircleRadians = (float)Math.PI * 2;
    float SectionAngleInRadians = FullCircleRadians / sections;

    Mesh mesh = new Mesh
    {
      name = $"Sphere_{radius}R_{sections}S",
    };

    int vertexCount = (sections + 1) * sections;

    Vector3[] vertices = new Vector3[vertexCount];
    int[] triangles = new int[sections * sections * 3];
    Vector2[] uvs = new Vector2[vertexCount];
    Vector3[] normals = new Vector3[vertexCount];

    vertices[0] = Vector3.zero;
    uvs[0] = Vector2.one * 0.5f;
    normals[0] = Vector3.back;

    for (int y = 1; y <= sections; y++)
    {
      int indexOffset = (y - 1) * sections;
      int triOffset = (y - 1) * sections + (sections - 1);
      float zPosition = Mathf.Sin(y * SectionAngleInRadians) * radius;
      Debug.Log($"Z - {zPosition} | Y - {y} | SectionAngleInRadians - {SectionAngleInRadians}");

      for (int i = 0, v = 1; i < sections; v++, i++)
      {
        float xPosition = Mathf.Cos(v * SectionAngleInRadians) * radius;
        float yPosition = Mathf.Sin(v * SectionAngleInRadians) * radius;

        vertices[indexOffset + v] = new Vector3(xPosition, yPosition, zPosition);

        triangles[indexOffset + i * 3] = triOffset + 0;
        triangles[indexOffset + i * 3 + 1] = v == sections ? triOffset + 1 : triOffset + i + 2;
        triangles[indexOffset + i * 3 + 2] = triOffset + i + 1;

        uvs[indexOffset + v] = new Vector2(
          Mathf.Lerp(-radius, radius, xPosition),
          Mathf.Lerp(-radius, radius, yPosition));

        normals[indexOffset + v] = Vector3.back;
      }
    }

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;
    mesh.normals = normals;

    return mesh;
  }
}
