using System.Collections.Generic;
using UnityEngine;

public class MarchingCubesGrid : MonoBehaviour
{
  public GameObject Prefab;
  public RandomNoise_SO NoiseScriptableObject;
  public int Width;
  public int Height;
  public int Depth;
  public float Threshold;
  private float[,] _noiseMap;
  private bool _isGameRunning = false;
  private GameObjectPool _pointsPool;
  private MeshFilter _meshFilter;
  private MeshRenderer _meshRenderer;
  private Mesh _mesh;
  public bool IsRenderingPoints;
  public bool IsRenderingMesh;

  void Awake()
  {
    _pointsPool = new GameObjectPool(Prefab, 50);
  }

  void Start()
  {
    setupRendering();
    GeneratePoints();
    updateMesh();
    SetGameRunning(true);
  }

  void OnValidate()
  {
    if (_isGameRunning)
    {
      GeneratePoints();
      updateMesh();
    }
  }

  private void setupRendering()
  {
    if (_meshFilter == null)
    {
      MeshFilter meshFilter = GetComponent<MeshFilter>();

      if (meshFilter == null)
      {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
      }
      else
      {
        _meshFilter = meshFilter;
      }
    }

    if (_meshRenderer == null)
    {
      MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

      if (meshRenderer == null)
      {
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
      }
      else
      {
        _meshRenderer = meshRenderer;
      }
    }

    if (_meshRenderer != null)
    {
      _meshRenderer.material = new Material(Shader.Find("Standard"));
    }
  }

  private void updateMesh()
  {
    if (!IsRenderingMesh)
    {
      return;
    }

    _mesh = GenerateMesh();
    _meshFilter.mesh = _mesh;
  }

  void GeneratePoints()
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
          meshRenderer.material.color = noiseValue > Threshold ? Color.black : Color.white;
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
    List<Vector2> uvs = new List<Vector2>();
    List<Vector3> normals = new List<Vector3>();

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
          uvs.AddRange(segment.Uvs);
          normals.AddRange(segment.Vertices);
        }
      }
    }

    // Assign the generated data to the mesh.
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.normals = normals.ToArray();

    return mesh;
  }

  private MarchingCubesSegment GenerateMeshSection(int x, int y, int z)
  {
    float bl1 = _noiseMap[x, y];
    float bl2 = _noiseMap[x + 1, y];
    float br1 = _noiseMap[x, y + 1];
    float br2 = _noiseMap[x + 1, y + 1];
    float tl1 = _noiseMap[x, y];
    float tl2 = _noiseMap[x + 1, y];
    float tr1 = _noiseMap[x, y + 1];
    float tr2 = _noiseMap[x + 1, y + 1];

    //TODO: Used z to determine if the value truely is above the threshold

    // convert the values to a binary based number where each bool repusents a bit
    int number = (bl1 > Threshold ? 1 : 0)
               | (bl2 > Threshold ? 1 : 0) << 1
               | (br1 > Threshold ? 1 : 0) << 2
               | (br2 > Threshold ? 1 : 0) << 3
               | (tl1 > Threshold ? 1 : 0) << 4
               | (tl2 > Threshold ? 1 : 0) << 5
               | (tr1 > Threshold ? 1 : 0) << 6
               | (tr2 > Threshold ? 1 : 0) << 7;

    return number switch
    {
      0 => CreateSegment0(),
      _ => CreateSegment0(),
    };
  }

  public struct MarchingCubesSegment
  {
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] Uvs;
    public Vector3[] Normals;

    public MarchingCubesSegment(
      Vector3[] vertices,
      int[] triangles,
      Vector2[] uvs,
      Vector3[] normals)
    {
      Vertices = vertices;
      Triangles = triangles;
      Uvs = uvs;
      Normals = normals;
    }
  }

  public void SetGameRunning(bool isRunning)
  {
    _isGameRunning = isRunning;
  }

  private static readonly Dictionary<int, (int[], float[])> SegmentConfigs = new()
{
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
    { 0, (new int[0], new float[0]) },
};
}