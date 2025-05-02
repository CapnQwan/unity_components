using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MarchingCubesGrid : MonoBehaviour
{
  // Public fields
  public GameObject Prefab;
  public PerlinNoise3D_SO NoiseScriptableObject;
  public int Width;
  public int Height;
  public int Depth;
  public float Threshold;
  public bool IsRenderingPoints;
  public bool IsRenderingMesh;

  // Private fields
  private float[,,] _noiseMap;
  private bool _isGameRunning = false;
  private GameObjectPool _pointsPool;
  private MeshFilter _meshFilter;
  private MeshRenderer _meshRenderer;
  private Mesh _mesh;

  // Public methods
  public void Awake()
  {
    _pointsPool = new GameObjectPool(Prefab, 50);
  }

  public void Start()
  {
    SetupRendering();
    UpdateNoiseMap();
    GeneratePoints();
    UpdateMesh();
    SetGameRunning(true);
  }

  public void OnValidate()
  {
    if (_isGameRunning)
    {
      UpdateNoiseMap();
      GeneratePoints();
      UpdateMesh();
    }
  }

  public void SetGameRunning(bool isRunning)
  {
    _isGameRunning = isRunning;
  }

  // Private methods
  private void SetupRendering()
  {
    if (_meshFilter == null)
    {
      MeshFilter meshFilter = GetComponent<MeshFilter>();
      _meshFilter = meshFilter == null ? gameObject.AddComponent<MeshFilter>() : meshFilter;
    }

    if (_meshRenderer == null)
    {
      MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

      _meshRenderer = meshRenderer == null ? gameObject.AddComponent<MeshRenderer>() : meshRenderer;
    }

    if (_meshRenderer != null)
    {
      _meshRenderer.material = new Material(Shader.Find("Standard"));
    }
  }

  private void UpdateNoiseMap()
  {
    _noiseMap = NoiseScriptableObject.GenerateNoiseMap(Width + 1, Height + 1, Depth + 1);
  }

  private void UpdateMesh()
  {
    if (!IsRenderingMesh)
    {
      return;
    }

    _mesh = GenerateMesh();
    _meshFilter.mesh = _mesh;
  }

  private void GeneratePoints()
  {
    if (!IsRenderingPoints)
    {
      return;
    }

    for (int x = 0; x < Width + 1; x++)
    {
      for (int y = 0; y < Height + 1; y++)
      {
        for (int z = 0; z < Depth + 1; z++)
        {
          float noiseValue = _noiseMap[x, y, z];

          GameObject point = _pointsPool.GetItem();
          point.transform.parent = transform;
          point.transform.position = new Vector3(x, y, z);

          MeshRenderer meshRenderer = point.GetComponent<MeshRenderer>();
          meshRenderer.material.color = noiseValue > Threshold ? Color.black : Color.white;
        }
      }
    }
  }

  private Mesh GenerateMesh()
  {
    Mesh mesh = new Mesh
    {
      name = $"Marching_Squares_{Width}x{Height}",
    };

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    // Use a thread-safe collection for vertices and triangles
    object lockObject = new object();

    // Create tasks for each slice of the grid
    List<Task> tasks = new List<Task>();
    for (int x = 0; x < Width; x++)
    {
      int xCopy = x; // Avoid closure issues
      tasks.Add(Task.Run(() =>
      {
        List<Vector3> localVertices = new List<Vector3>();
        List<int> localTriangles = new List<int>();
        List<Vector2> localUVs = new List<Vector2>();

        for (int y = 0; y < Height; y++)
        {
          for (int z = 0; z < Depth; z++)
          {
            MarchingCubesSegment segment = GenerateMeshSection(xCopy, y, z);

            for (int i = 0; i < segment.Triangles.Length; i++)
            {
              segment.Triangles[i] += localVertices.Count;
            }

            foreach (Vector3 vertex in segment.Vertices)
            {
              localUVs.Add(new Vector2(vertex.x / Width, vertex.z / Depth)); // Example UV mapping
            }

            localVertices.AddRange(segment.Vertices);
            localTriangles.AddRange(segment.Triangles);
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

  private MarchingCubesSegment GenerateMeshSection(int x, int y, int z)
  {
    // Get scalar values for the cube's 8 vertices
    float[] scalarValues = new float[8];
    scalarValues[0] = _noiseMap[x, y, z];             // Front bottom left
    scalarValues[1] = _noiseMap[x + 1, y, z];         // Front bottom right
    scalarValues[2] = _noiseMap[x, y + 1, z];         // Front left left
    scalarValues[3] = _noiseMap[x + 1, y + 1, z];     // Front left right
    scalarValues[4] = _noiseMap[x, y, z + 1];         // Back bottom left
    scalarValues[5] = _noiseMap[x + 1, y, z + 1];     // Back bottom right
    scalarValues[6] = _noiseMap[x, y + 1, z + 1];     // Back top left
    scalarValues[7] = _noiseMap[x + 1, y + 1, z + 1]; // Back top right

    // Calculate the case index
    int caseIndex = 0;
    for (int i = 0; i < 8; i++)
    {
      if (scalarValues[i] > Threshold)
      {
        caseIndex |= 1 << i;
      }
    }

    // Get the triangle configuration for this case
    int[] triangleEdges = MarchingCubesLookupTable.TriangleTable[caseIndex];
    if (triangleEdges[0] == -1)
    {
      // No triangles for this case
      return new MarchingCubesSegment(new Vector3[0], new int[0]);
    }

    // Interpolate vertices along intersected edges
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

      float t = Mathf.Clamp01((Threshold - value1) / (value2 - value1));
      vertices.Add(Vector3.Lerp(vertex1, vertex2, t));
    }

    // Create triangles
    List<int> triangles = new List<int>();
    for (int i = 0; i < triangleEdges.Length; i += 3)
    {
      if (triangleEdges[i] == -1) break;
      triangles.Add(i);
      triangles.Add(i + 1);
      triangles.Add(i + 2);
    }

    return new MarchingCubesSegment(vertices.ToArray(), triangles.ToArray());
  }
}
