using System;
using Unity.Mathematics;
using UnityEngine;

public static class CircleMeshGenerator
{
  /// <summary>
  /// Cached instance of a standard Sphere mesh with 20 sections and a radius of 1f. Prevents generating the same mesh repeatedly.
  /// </summary>
  private static Mesh _cachedMesh;

  /// <summary>
  /// Generates a simple Sphere mesh with 20 sections and a radius of 1f.
  /// This method uses caching to avoid generating multiple identical meshes.
  /// </summary>
  /// <returns>A Unity <see cref="Mesh"/> representing a cricle with a radius of 1.</returns>
  public static Mesh GenerateSimpleCircle()
  {
    if (!_cachedMesh)
    {
      _cachedMesh = GenerateCircle(1f, 20);
    }

    return _cachedMesh;
  }

  public static Mesh GenerateCircle(CircleMesh_SO meshScriptableObject)
  {
    return GenerateCircle(meshScriptableObject.Radius, meshScriptableObject.Sections);
  }

  public static Mesh GenerateCircle(float radius, int sections)
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
      name = $"Circle_{radius}R_{sections}S",
    };

    Vector3[] vertices = new Vector3[sections + 1];
    int[] triangles = new int[sections * 3];
    Vector2[] uvs = new Vector2[sections + 1];
    Vector3[] normals = new Vector3[sections + 1];

    vertices[0] = Vector3.zero;
    uvs[0] = Vector2.one * 0.5f;
    normals[0] = Vector3.back;

    for (int i = 0, v = 1; i < sections; v++, i++)
    {
      float x = Mathf.Cos(v * SectionAngleInRadians) * radius;
      float y = Mathf.Sin(v * SectionAngleInRadians) * radius;

      vertices[v] = new Vector3(x, y, 0f);

      triangles[i * 3] = 0;
      triangles[i * 3 + 1] = v == sections ? 1 : i + 2;
      triangles[i * 3 + 2] = i + 1;

      uvs[v] = new Vector2(Mathf.Lerp(-radius, radius, x), Mathf.Lerp(-radius, radius, y));
      normals[v] = Vector3.back;
    }

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    //mesh.uv = uvs;
    mesh.normals = normals;

    return mesh;
  }
}
