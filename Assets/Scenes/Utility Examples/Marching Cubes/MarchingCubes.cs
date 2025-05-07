using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class MarchingCubes
{
  public static Mesh GenerateMesh(
    int width,
    int height,
    int depth,
    float[,,] scalarMap,
    float threshold)
  {
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
            GenerateMeshSlice(
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

  private static void GenerateMeshSlice(
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
    // Get scalar values for the cube's 8 vertices
    float[] scalarValues = getScalarValues(scalarMap, x, y, z);

    // Calculate the case index
    int caseIndex = GetCaseIndex(scalarValues, threshold);

    // Get the triangle configuration for this case
    int[] triangleEdges = MarchingCubesLookupTable.TriangleTable[caseIndex];
    if (triangleEdges[0] == -1)
    {
      // No triangles for this case
      return;
    }

    // Interpolate vertices along intersected edges
    List<Vector3> vertices = GetVerticies(triangleEdges, scalarValues, x, y, z, threshold);
    List<int> triangles = CreateTriangleIndices(triangleEdges, localVertices.Count);
    CreateUVs(localUVs, vertices, width, depth);

    localVertices.AddRange(vertices);
    localTriangles.AddRange(triangles);
  }

  /// <summary>
  /// Get the scalar values for the vertices of a cube in the scalar map.
  /// The cube is defined by its bottom-left-front corner (x, y, z) and has a
  /// size of 2x2x2.
  /// </summary>
  /// <param name="scalarMap"></param>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <param name="z"></param>
  /// <returns>
  /// The scalar values for the vertices of a cube in the scalar map.
  /// </returns>
  /// TODO: convert to private
  public static float[] getScalarValues(
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

  /// <summary>
  /// Get the case index for the cube based on the scalar values and threshold.
  /// The case index is a bitmask where each bit represents whether the
  /// corresponding vertex is above the threshold.
  /// </summary>
  /// <param name="scalarValues"></param>
  /// <param name="threshold"></param>
  /// <returns>
  /// The case index for the cube based on the scalar values and threshold.
  /// </returns>
  /// TODO: convert to private
  public static int GetCaseIndex(float[] scalarValues, float threshold)
  {
    int caseIndex = 0;

    for (int i = 0; i < 8; i++)
    {
      if (scalarValues[i] > threshold)
      {
        caseIndex |= 1 << i;
      }
    }

    return caseIndex;
  }

  // TODO: convert to private
  public static List<Vector3> GetVerticies(
    int[] triangleEdges,
    float[] scalarValues,
    int x,
    int y,
    int z,
    float threshold)
  {
    List<Vector3> vertices = new List<Vector3>();
    for (int i = 0; i < triangleEdges.Length; i++)
    {
      if (triangleEdges[i] == -1) break;

      int edgeIndex = triangleEdges[i];
      int[] edgeVertices = MarchingCubesLookupTable.EdgeVertexIndices[edgeIndex];

      Vector3 vertex1 = new Vector3(x, y, z) + MarchingCubesLookupTable.VertexOffsets[edgeVertices[0]];
      Vector3 vertex2 = new Vector3(x, y, z) + MarchingCubesLookupTable.VertexOffsets[edgeVertices[1]];
      float value1 = scalarValues[edgeVertices[0]];
      float value2 = scalarValues[edgeVertices[1]];

      float t = Mathf.Clamp01((threshold - value1) / (value2 - value1));
      vertices.Add(Vector3.Lerp(vertex1, vertex2, t));
    }
    return vertices;
  }

  public static List<int> CreateTriangleIndices(int[] triangleEdges, int offset)
  {
    List<int> triangles = new List<int>();

    for (int i = 0; i < triangleEdges.Length; i += 3)
    {
      if (triangleEdges[i] == -1) break;
      triangles.Add(i + offset);
      triangles.Add(i + 1 + offset);
      triangles.Add(i + 2 + offset);
    }

    return triangles;
  }

  public static void CreateUVs(
    List<Vector2> localUVs,
    List<Vector3> vertices,
    int width,
    int depth)
  {
    foreach (Vector3 vertex in vertices)
    {
      localUVs.Add(new Vector2(vertex.x / width, vertex.z / depth));
    }
  }
}
