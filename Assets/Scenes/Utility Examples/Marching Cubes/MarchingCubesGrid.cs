using System.Collections.Generic;
using UnityEngine;

public class MarchingCubesGrid : MonoBehaviour
{
  // Public fields
  public GameObject Prefab;
  public RandomNoise_SO NoiseScriptableObject;
  public int Width;
  public int Height;
  public int Depth;
  public float Threshold;
  public bool IsRenderingPoints;
  public bool IsRenderingMesh;

  // Private fields
  private float[,] _noiseMap;
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
    GeneratePoints();
    UpdateMesh();
    SetGameRunning(true);
  }

  public void OnValidate()
  {
    if (_isGameRunning)
    {
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

    _noiseMap = NoiseScriptableObject.GenerateNoiseMap(Width + 2, Height + 2);
    for (int x = 0; x < Width; x++)
    {
      for (int y = 0; y < Height; y++)
      {
        for (int z = 0; z < Depth; z++)
        {
          float noiseValue = _noiseMap[x + 1, y + 1];

          GameObject point = _pointsPool.GetItem();
          point.transform.parent = transform;
          point.transform.position = new Vector3(x, y, z);

          MeshRenderer meshRenderer = point.GetComponent<MeshRenderer>();
          float tempThreshold = Threshold + z * 0.02f;
          meshRenderer.material.color = noiseValue > tempThreshold ? Color.black : Color.white;
        }
      }
    }
  }

  private Mesh GenerateMesh()
  {
    _noiseMap = NoiseScriptableObject.GenerateNoiseMap(Width, Height);

    Mesh mesh = new Mesh
    {
      name = $"Marching_Squares_{Width}x{Height}",
    };

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    for (int x = 0; x < Width - 1; x++)
    {
      for (int y = 0; y < Height - 1; y++)
      {
        for (int z = 0; z < Depth - 1; z++)
        {
          MarchingCubesSegment segment = GenerateMeshSection(x, y, z);

          for (int i = 0; i < segment.Triangles.Length; i++)
          {
            segment.Triangles[i] += vertices.Count;
          }

          vertices.AddRange(segment.Vertices);
          triangles.AddRange(segment.Triangles);
        }
      }
    }

    // Assign the generated data to the mesh.
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();

    return mesh;
  }

  private MarchingCubesSegment GenerateMeshSection(int x, int y, int z)
  {
    // Get scalar values for the cube's 8 vertices
    float[] scalarValues = new float[8];
    scalarValues[0] = _noiseMap[x, y];         // Front bottom left
    scalarValues[1] = _noiseMap[x + 1, y];     // Back bottom left
    scalarValues[2] = _noiseMap[x, y + 1];     // Front bottom right
    scalarValues[3] = _noiseMap[x + 1, y + 1]; // Back bottom right
    scalarValues[4] = _noiseMap[x, y];         // Front top left
    scalarValues[5] = _noiseMap[x + 1, y];     // Back top left
    scalarValues[6] = _noiseMap[x, y + 1];     // Front top right
    scalarValues[7] = _noiseMap[x + 1, y + 1]; // Back top right

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

      float t = (Threshold - value1) / (value2 - value1);
      vertices.Add(Vector3.Lerp(vertex1, vertex2, 0.25f));
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

  public struct MarchingCubesSegment
  {
    public Vector3[] Vertices;
    public int[] Triangles;

    public MarchingCubesSegment(
      Vector3[] vertices,
      int[] triangles)
    {
      Vertices = vertices;
      Triangles = triangles;
    }
  }
}
