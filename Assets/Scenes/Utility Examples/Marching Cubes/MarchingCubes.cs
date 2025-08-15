using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class MarchingCubes
{
  public static Mesh GenerateMesh(
    float[,,] scalarMap,
    float threshold,
    bool isRenderingEdges = false)
  {
    Vector3Int dimensions = new Vector3Int(scalarMap.GetLength(0) - 1, scalarMap.GetLength(1) - 1, scalarMap.GetLength(2) - 1);

    Mesh mesh = new Mesh
    {
      name = $"Marching_Squares_{dimensions.x}x{dimensions.y}x{dimensions.z}",
    };

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    // Use a thread-safe collection for vertices and triangles
    object lockObject = new object();

    // Create tasks for each slice of the grid
    List<Task> tasks = new List<Task>();

    for (int x = 0; x < dimensions.x; x++)
    {
      int xCopy = x; // Avoid closure issues
      tasks.Add(Task.Run(() =>
      {
        List<Vector3> localVertices = new List<Vector3>();
        List<int> localTriangles = new List<int>();
        List<Vector2> localUVs = new List<Vector2>();

        for (int y = 0; y < dimensions.y; y++)
        {
          for (int z = 0; z < dimensions.z; z++)
          {
            IsoGridCell cell = new IsoGridCell(new Vector3(xCopy, y, z), new float[8]
            {
              scalarMap[xCopy, y, z],
              scalarMap[xCopy + 1, y, z],
              scalarMap[xCopy + 1, y + 1, z],
              scalarMap[xCopy, y + 1, z],
              scalarMap[xCopy, y, z + 1],
              scalarMap[xCopy + 1, y, z + 1],
              scalarMap[xCopy + 1, y + 1, z + 1],
              scalarMap[xCopy, y + 1, z + 1],
            });

            GenerateMeshSlice(cell, dimensions, threshold, localVertices, localTriangles, localUVs, isRenderingEdges);
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

  private static void GenerateMeshSlice(
    IsoGridCell cell,
    Vector3Int dimensions,
    float threshold,
    List<Vector3> localVertices,
    List<int> localTriangles,
    List<Vector2> localUVs,
    bool isRenderingEdges)
  {
    // Calculate the case index
    int caseIndex = GetCaseIndex(cell, threshold);

    // Interpolate vertices along intersected edges
    List<Vector3> vertices = GetVerticies(caseIndex, cell, threshold);
    List<int> triangles = CreateTriangleIndices(vertices.Count, localVertices.Count);
    CreateUVs(localUVs, vertices, dimensions.x, dimensions.z);

    localVertices.AddRange(vertices);
    localTriangles.AddRange(triangles);

    if (isRenderingEdges)
    {
      // Generate edge vertices if rendering edges
      List<Vector3> edgeVertices = GenerateEdgeVerticies(cell, dimensions, threshold);
      List<int> edgeTriangles = generateEdgeTriangles(localVertices, edgeVertices, dimensions);
      localVertices.AddRange(edgeVertices);
      localTriangles.AddRange(edgeTriangles);
    }
  }

  private static List<Vector3> GetVerticies(
    int caseIndex,
    IsoGridCell cell,
    float threshold)
  {
    List<Vector3> vertices = new List<Vector3>();
    for (int i = 0; i <= 12; i += 3)
    {
      int triIndexA = MarchingCubesLookupTable.Triangulation[caseIndex, i];
      int triIndexB = MarchingCubesLookupTable.Triangulation[caseIndex, i + 1];
      int triIndexC = MarchingCubesLookupTable.Triangulation[caseIndex, i + 2];

      if (triIndexA == -1) break;

      vertices.Add(InterpolateVertex(
        cell.Vertices[MarchingCubesLookupTable.CornerIndexAFromEdge[triIndexA]],
        cell.Vertices[MarchingCubesLookupTable.CornerIndexBFromEdge[triIndexA]],
        cell.IsoValues[MarchingCubesLookupTable.CornerIndexAFromEdge[triIndexA]],
        cell.IsoValues[MarchingCubesLookupTable.CornerIndexBFromEdge[triIndexA]],
        threshold));

      vertices.Add(InterpolateVertex(
        cell.Vertices[MarchingCubesLookupTable.CornerIndexAFromEdge[triIndexB]],
        cell.Vertices[MarchingCubesLookupTable.CornerIndexBFromEdge[triIndexB]],
        cell.IsoValues[MarchingCubesLookupTable.CornerIndexAFromEdge[triIndexB]],
        cell.IsoValues[MarchingCubesLookupTable.CornerIndexBFromEdge[triIndexB]],
        threshold));

      vertices.Add(InterpolateVertex(
        cell.Vertices[MarchingCubesLookupTable.CornerIndexAFromEdge[triIndexC]],
        cell.Vertices[MarchingCubesLookupTable.CornerIndexBFromEdge[triIndexC]],
        cell.IsoValues[MarchingCubesLookupTable.CornerIndexAFromEdge[triIndexC]],
        cell.IsoValues[MarchingCubesLookupTable.CornerIndexBFromEdge[triIndexC]],
        threshold));
    }

    return vertices;
  }

  private static List<Vector3> GenerateEdgeVerticies(IsoGridCell cell, Vector3Int dimensions, float threshold)
  {
    List<Vector3> edgeVertices = new List<Vector3>();

    for (int i = 0; i < cell.Vertices.Length; i++)
    {
      if (cell.IsoValues[i] >= threshold) continue;

      if (cell.Vertices[i].x % dimensions.x == 0 ||
          cell.Vertices[i].y % dimensions.y == 0 ||
          cell.Vertices[i].z % dimensions.z == 0)
      {
        edgeVertices.Add(cell.Vertices[i]);
      }
    }

    return edgeVertices;
  }

  private static List<int> generateEdgeTriangles(List<Vector3> cellVertices, List<Vector3> edgeVertices, Vector3Int dimensions)
  {
    if (edgeVertices.Count <= 0) return new List<int>();

    List<Vector3> cellEdgeVertices = new List<Vector3>();
    List<int> edgeTriangles = new List<int>();

    for (int i = 0; i < cellVertices.Count; i++)
    {
      if (cellVertices[i].x % dimensions.x == 0 ||
          cellVertices[i].y % dimensions.y == 0 ||
          cellVertices[i].z % dimensions.z == 0)
      {
        cellEdgeVertices.Add(cellVertices[i]);
      }
    }

    int offset = cellVertices.Count;

    Debug.Log($"Edge Vertices Count: {edgeVertices.Count}");
    Debug.Log($"Cell Edge Vertices Count: {cellEdgeVertices.Count}, Offset: {offset}");

    for (int i = 0; i < edgeVertices.Count; i += 2)
    {

    }

    return edgeTriangles;
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

  private static List<int> CreateTriangleIndices(int vertexCount, int offset)
  {
    List<int> triangles = new List<int>();

    for (int i = 0; i < vertexCount; i += 3)
    {
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