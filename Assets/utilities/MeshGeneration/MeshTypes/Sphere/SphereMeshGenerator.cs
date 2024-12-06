using System;
using Unity.Mathematics;
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

    Mesh mesh = new Mesh
    {
      name = $"Sphere_{radius}R_{sections}S",
    };

    float FullCircleRadians = (float)Math.PI * 2;
    float SectionAngleInRadians = FullCircleRadians / sections;

    int columns = sections;
    int rows = sections - 2;

    int vertexCount = columns * rows;
    int triCount = ((rows * sections * 2) + (sections * 2)) * 3;

    Vector3[] vertices = new Vector3[vertexCount];
    int[] triangles = new int[triCount];
    Vector2[] uvs = new Vector2[vertexCount];
    Vector3[] normals = new Vector3[vertexCount];

    vertices[0] = new Vector3(0f, 0f, 1f * radius);
    vertices[vertexCount - 1] = new Vector3(0f, 0f, -1f * radius);
    uvs[0] = Vector2.one * 0.5f;
    uvs[vertexCount - 1] = Vector2.one * 0.5f;
    normals[0] = Vector3.up;
    normals[vertexCount - 1] = Vector3.down;

    for (int r = 1, i = 1, t = -sections + 1; r <= rows; r++)
    {
      int indexOffset = (r - 1) * sections + 1;
      int triOffset = (r - 1) * sections * 2;
      float zPosition = Mathf.Cos(rows - r * SectionAngleInRadians) * radius;

      for (int c = 1; c <= sections; c++, i++, t += 3)
      {
        float xPosition = Mathf.Cos(c * SectionAngleInRadians) * radius;
        float yPosition = Mathf.Sin(c * SectionAngleInRadians) * radius;

        vertices[indexOffset + c] = new Vector3(xPosition, yPosition, zPosition);



        triangles[indexOffset + i * 3] = math.clamp(t, 0, triCount) + triOffset + 0;
        triangles[indexOffset + i * 3 + 1] = c == sections ? triOffset + 1 : triOffset + i + 2;
        triangles[indexOffset + i * 3 + 2] = triOffset + i + 1;

        uvs[indexOffset + c] = new Vector2(
          Mathf.Lerp(-radius, radius, xPosition),
          Mathf.Lerp(-radius, radius, yPosition));

        normals[indexOffset + c] = Vector3.back;
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
