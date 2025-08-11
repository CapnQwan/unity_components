using System.Collections.Generic;
using System.Threading.Tasks;
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
      name = $"Marching_Squares_{width}x{height}x{depth}",
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
            IsoGridCell cell = new IsoGridCell(new Vector3(xCopy, y, z), new int[8]
            {
              (int)scalarMap[xCopy, y, z],
              (int)scalarMap[xCopy + 1, y, z],
              (int)scalarMap[xCopy + 1, y + 1, z],
              (int)scalarMap[xCopy, y + 1, z],
              (int)scalarMap[xCopy, y, z + 1],
              (int)scalarMap[xCopy + 1, y, z + 1],
              (int)scalarMap[xCopy + 1, y + 1, z + 1],
              (int)scalarMap[xCopy, y + 1, z + 1],
            });

            GenerateMeshSlice(cell, threshold, localVertices, localTriangles, localUVs);
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
    IsoGridCell cell,
    float threshold,
    List<Vector3> localVertices,
    List<int> localTriangles,
    List<Vector2> localUVs)
  {
    // Calculate the case index
    int caseIndex = GetCaseIndex(cell, threshold);

    // Interpolate vertices along intersected edges
    List<Vector3> vertices = GetVerticies(caseIndex, cell, threshold);
    List<int> triangles = CreateTriangleIndices(caseIndex, localVertices.Count);
    CreateUVs(localUVs, vertices, width, depth);

    localVertices.AddRange(vertices);
    localTriangles.AddRange(triangles);
  }

  /// <summary>
  /// Get the case index for the cube based on the scalar values and threshold.
  /// The case index is a bitmask where each bit represents whether the
  /// corresponding vertex is above the threshold.
  /// </summary>
  /// <param name="cell">Positional and scalar information about the cube.</param>
  /// <param name="threshold">The threshold value used to determine the case index.</param>
  /// <returns>
  /// The case index for the cube based on the scalar values and threshold.
  /// </returns>
  public static int GetCaseIndex(IsoGridCell cell, float threshold)
  {
    int caseIndex = 0;

    if (cell.IsoValues[0] < threshold) caseIndex |= 1;
    if (cell.IsoValues[1] < threshold) caseIndex |= 2;
    if (cell.IsoValues[2] < threshold) caseIndex |= 4;
    if (cell.IsoValues[3] < threshold) caseIndex |= 8;
    if (cell.IsoValues[4] < threshold) caseIndex |= 16;
    if (cell.IsoValues[5] < threshold) caseIndex |= 32;
    if (cell.IsoValues[6] < threshold) caseIndex |= 64;
    if (cell.IsoValues[7] < threshold) caseIndex |= 128;

    return caseIndex;
  }

  private static List<Vector3> GetVerticies(
    int caseIndex,
    IsoGridCell cell,
    float threshold)
  {
    List<Vector3> vertices = new List<Vector3>();
    for (int i = 0; i < 16; i += 3)
    {
      int triA = MarchingCubesLookupTable.Triangulation[caseIndex, i];
      int triB = MarchingCubesLookupTable.Triangulation[caseIndex, i + 1];
      int triC = MarchingCubesLookupTable.Triangulation[caseIndex, i + 2];

      if (triA == -1) break;

      int edgeIndex = triangleEdges[i];

      int indexA = MarchingCubesLookupTable.CornerIndexAFromEdge[edgeIndex];
      int indexB = MarchingCubesLookupTable.CornerIndexBFromEdge[edgeIndex];

      vertices.Add(InterpolateVertex(
        position + MarchingCubesLookupTable.CellVertices[indexA],
        position + MarchingCubesLookupTable.CellVertices[indexB],
        scalarValues[indexA],
        scalarValues[indexB],
        threshold));
    }

    return vertices;
  }

  private static int IndexFromCoords(int x, int y, int z, int width, int height, int depth)
  {
    return x * width + y * height + z * depth;
  }

  private static Vector3 InterpolateVertex(
    Vector3 vertex1,
    Vector3 vertex2,
    float value1,
    float value2,
    float threshold)
  {
    float t = Mathf.Clamp01((threshold - value1) / (value2 - value1));
    return Vector3.Lerp(vertex1, vertex2, t);
  }

  private static List<int> CreateTriangleIndices(int[] triangleEdges, int offset)
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

  private static void CreateUVs(
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