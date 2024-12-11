using System;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
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
    if (sections < 2)
    {
      throw new ArgumentException("Sections must be 2 or greater.");
    }

    Mesh mesh = new Mesh
    {
      name = $"Sphere_{radius}R_{sections}S",
    };

    int columns = sections * 2;
    int rows = sections - 1;

    float FullCircleRadians = (float)Math.PI * 2;
    float SectionAngleInRadians = FullCircleRadians / columns;

    int vertexCount = columns * rows + 2;
    int triCount = rows * columns * 6;

    Vector3[] vertices = new Vector3[vertexCount];
    int[] triangles = new int[triCount];
    Vector2[] uvs = new Vector2[vertexCount];
    Vector3[] normals = new Vector3[vertexCount];

    vertices[0] = new Vector3(0f, 1f * radius, 0f);
    vertices[vertexCount - 1] = new Vector3(0f, -1f * radius, 0f);
    uvs[0] = new Vector2(0, 0);
    uvs[vertexCount - 1] = new Vector2(0, 1);
    normals[0] = Vector3.forward;
    normals[vertexCount - 1] = Vector3.forward;

    for (int r = 1, i = 0, t = 1 - columns; r <= rows; r++)
    {
      float yCos = Mathf.Clamp(Mathf.Cos((r - (sections * 0.5f)) * SectionAngleInRadians), -1f, 1f);
      float yPosition = Mathf.Clamp(Mathf.Cos(r * SectionAngleInRadians), -1f, 1f) * radius;

      _ = Parallel.For(1, columns + 1, c =>
      {
        int localI = i + c - 1;
        int localT = t + c - 1;

        float xPosition = Mathf.Clamp(Mathf.Cos(c * SectionAngleInRadians), -1f, 1f) * yCos * radius;
        float zPosition = Mathf.Clamp(Mathf.Sin(c * SectionAngleInRadians), -1f, 1f) * yCos * radius;

        vertices[localI + 1] = new Vector3(xPosition, yPosition, zPosition);

        triangles[localI * 6] = math.clamp(localT, 0, triCount);
        triangles[localI * 6 + 1] = WNMathUtils.WrapInt(localT + columns + 1, (r - 1) * columns + 1, r * columns);
        triangles[localI * 6 + 2] = localT + columns;
        triangles[localI * 6 + 3] = WNMathUtils.WrapInt(localT + columns - 1, (r - 1) * columns + 1, r * columns);
        triangles[localI * 6 + 4] = WNMathUtils.WrapInt(localT + columns, localT + columns, localT + columns * 2 - 1);
        triangles[localI * 6 + 5] = math.clamp(localT + columns * 2, 0, vertexCount - 1);

        uvs[localI + 1] = new Vector2((c - 1) / (columns - 1), (r - 1) / (rows - 1));
        normals[localI + 1] = Vector3.forward;
      });

      i += columns;
      t += columns;
    }

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;
    mesh.normals = normals;

    return mesh;
  }
}
