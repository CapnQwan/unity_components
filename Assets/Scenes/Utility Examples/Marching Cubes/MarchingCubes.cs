using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class MarchingCubes
{
  public static Mesh GenerateMesh(
    float[,,] scalarMap,
    float threshold)
  {
    int width = scalarMap.GetLength(0) - 1;
    int height = scalarMap.GetLength(1) - 1;
    int depth = scalarMap.GetLength(2) - 1;

    Mesh mesh = new Mesh
    {
      name = $"Marching_Squares_{width}x{height}",
    };

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    // Use a thread-safe collection for vertices and triangles
    object lockObject = new object();

    // Create tasks for each slice of the grid
    List<Task> tasks = new List<Task>();

    for (int x = 0; x < width; x++)
    {
      int xCopy = x; // Avoid closure issues
      tasks.Add(Task.Run(() =>
      {
        List<Vector3> localVertices = new List<Vector3>();
        List<int> localTriangles = new List<int>();
        List<Vector2> localUVs = new List<Vector2>();

        for (int y = 0; y < height; y++)
        {
          for (int z = 0; z < depth; z++)
          {
            GenerateCellPoly(
              xCopy,
              y,
              z,
              width,
              depth,
              scalarMap,
              threshold,
              localVertices,
              localTriangles,
              localUVs);
          }
        }

        // Lock and add local results to the main lists
        lock (lockObject)
        {
          int vertexOffset = vertices.Count;
          vertices.AddRange(localVertices);
          uvs.AddRange(localUVs);

          foreach (int triangle in localTriangles)
          {
            triangles.Add(triangle + vertexOffset);
          }
        }
      }));
    }

    // Wait for all tasks to complete
    Task.WaitAll(tasks.ToArray());

    // Assign the generated data to the mesh
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();

    // Automatically calculate normals
    mesh.RecalculateNormals();

    return mesh;
  }

  private static void GenerateCellPoly(
    int x,
    int y,
    int z,
    int width,
    int depth,
    float[,,] scalarMap,
    float threshold,
    List<Vector3> localVertices,
    List<int> localTriangles,
    List<Vector2> localUVs)
  {
    float[] scalarValues = GetScalarValues(scalarMap, x, y, z);

    for (int i = 0; i < MarchingCubesLookupTable.PolygonsIndices.Length; i++)
    {
      int[] polygonIndices = MarchingCubesLookupTable.PolygonsIndices[i];
      float[] polygonScalarValues = new float[polygonIndices.Length];

      for (int j = 0; j < polygonIndices.Length; j++)
      {
        polygonScalarValues[j] = scalarValues[polygonIndices[j]];
      }

      int numTriangles = PolygoniseTri(
        localVertices,
        polygonIndices,
        polygonScalarValues,
        threshold);

      for (int j = 0; j < numTriangles; j++)
      {
        int triangleOffset = numTriangles * 3;
        localTriangles.Add(localVertices.Count - triangleOffset);
        localTriangles.Add(localVertices.Count - triangleOffset + 1);
        localTriangles.Add(localVertices.Count - triangleOffset + 2);
      }
    }
  }

  private static int PolygoniseTri(
    List<Vector3> localVertices,
    int[] polygonIndices,
    float[] scalarValues,
    float threshold)
  {
    int triCount = 0;
    int triCase = 0;

    Vector3 v0 = MarchingCubesLookupTable.Verticies[polygonIndices[0]];
    Vector3 v1 = MarchingCubesLookupTable.Verticies[polygonIndices[1]];
    Vector3 v2 = MarchingCubesLookupTable.Verticies[polygonIndices[2]];
    Vector3 v3 = MarchingCubesLookupTable.Verticies[polygonIndices[3]];

    float s0 = scalarValues[0];
    float s1 = scalarValues[1];
    float s2 = scalarValues[2];
    float s3 = scalarValues[3];

    if (s0 > threshold) triCase |= 1;
    if (s1 > threshold) triCase |= 2;
    if (s2 > threshold) triCase |= 4;
    if (s3 > threshold) triCase |= 8;

    float t = threshold;

    switch (triCase)
    {
      case 0x00:
      case 0x0F:
        break; // No vertices to create
      case 0x0E:
      case 0x01:
        localVertices.Add(GetInterpolateVertex(v0, v1, s0, s1, t));
        localVertices.Add(GetInterpolateVertex(v0, v2, s0, s2, t));
        localVertices.Add(GetInterpolateVertex(v0, v3, s0, s3, t));
        triCount++;
        break;
      case 0x0D:
      case 0x02:
        localVertices.Add(GetInterpolateVertex(v1, v0, s1, s0, t));
        localVertices.Add(GetInterpolateVertex(v1, v3, s1, s3, t));
        localVertices.Add(GetInterpolateVertex(v1, v2, s1, s2, t));
        triCount++;
        break;
      case 0x0C:
      case 0x03:
        localVertices.Add(GetInterpolateVertex(v0, v3, s0, s3, t));
        localVertices.Add(GetInterpolateVertex(v0, v2, s0, s2, t));
        localVertices.Add(GetInterpolateVertex(v1, v3, s1, s3, t));
        triCount++;
        localVertices.Add(GetInterpolateVertex(v1, v3, s1, s3, t));
        localVertices.Add(GetInterpolateVertex(v1, v2, s1, s2, t));
        localVertices.Add(GetInterpolateVertex(v0, v2, s0, s2, t));
        triCount++;
        break;
      case 0x0B:
      case 0x04:
        localVertices.Add(GetInterpolateVertex(v2, v0, s2, s0, t));
        localVertices.Add(GetInterpolateVertex(v2, v1, s2, s1, t));
        localVertices.Add(GetInterpolateVertex(v2, v3, s2, s3, t));
        triCount++;
        break;
      case 0x0A:
      case 0x05:
        localVertices.Add(GetInterpolateVertex(v0, v1, s0, s1, t));
        localVertices.Add(GetInterpolateVertex(v2, v3, s2, s3, t));
        localVertices.Add(GetInterpolateVertex(v0, v3, s0, s3, t));
        triCount++;
        localVertices.Add(GetInterpolateVertex(v0, v1, s0, s1, t));
        localVertices.Add(GetInterpolateVertex(v1, v2, s1, s2, t));
        localVertices.Add(GetInterpolateVertex(v2, v3, s2, s3, t));
        triCount++;
        break;
      case 0x09:
      case 0x06:
        localVertices.Add(GetInterpolateVertex(v0, v1, s0, s1, t));
        localVertices.Add(GetInterpolateVertex(v1, v3, s1, s3, t));
        localVertices.Add(GetInterpolateVertex(v2, v3, s2, s3, t));
        triCount++;
        localVertices.Add(GetInterpolateVertex(v0, v1, s0, s1, t));
        localVertices.Add(GetInterpolateVertex(v0, v2, s0, s2, t));
        localVertices.Add(GetInterpolateVertex(v2, v3, s2, s3, t));
        triCount++;
        break;
      case 0x07:
      case 0x08:
        localVertices.Add(GetInterpolateVertex(v3, v0, s3, s0, t));
        localVertices.Add(GetInterpolateVertex(v3, v2, s3, s2, t));
        localVertices.Add(GetInterpolateVertex(v3, v1, s3, s1, t));
        triCount++;
        break;
    }

    return triCount;
  }

  /// <summary>
  /// Get the scalar values for the vertices of a cube in the scalar map.
  /// The cube is defined by its bottom-left-front corner (x, y, z) and has a
  /// size of 2x2x2.
  /// </summary>
  /// <param name="scalarMap">The scalar map values from 0 to 1 that determine the position of the vertices.</param>
  /// <param name="x">The x-coordinate of the cube's bottom-left-front corner.</param>
  /// <param name="y">The y-coordinate of the cube's bottom-left-front corner.</param>
  /// <param name="z">The z-coordinate of the cube's bottom-left-front corner.</param>
  /// <returns>
  /// The scalar values for the vertices of a cube in the scalar map.
  /// </returns>
  public static float[] GetScalarValues(
    float[,,] scalarMap,
    int x,
    int y,
    int z)
  {
    return new float[]
    {
      scalarMap[x, y, z],             // Front bottom left
      scalarMap[x + 1, y, z],         // Front bottom right
      scalarMap[x, y + 1, z],         // Front top left
      scalarMap[x + 1, y + 1, z],     // Front top right
      scalarMap[x, y, z + 1],         // Back bottom left
      scalarMap[x + 1, y, z + 1],     // Back bottom right
      scalarMap[x, y + 1, z + 1],     // Back top left
      scalarMap[x + 1, y + 1, z + 1], // Back top right
    };
  }

  private static Vector3 GetInterpolateVertex(
    Vector3 v1,
    Vector3 v2,
    float value1,
    float value2,
    float threshold)
  {
    if (Mathf.Approximately(value1, value2))
    {
      return v1;
    }

    float t = (threshold - value1) / (value2 - value1);
    return Vector3.Lerp(v1, v2, t);
  }
}
