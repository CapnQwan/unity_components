using UnityEngine;

public class MarchingCubesGrid : MonoBehaviour
{
  // Public fields
  public GameObject PointPrefab;
  public RandomNoise_SO NoiseScriptableObject;
  public int Width;
  public int Height;
  public int Depth;
  [Range(0.0f, 1.0f)]
  public float Threshold;
  public bool IsRenderingPoints;
  public bool IsRenderingMesh;
  public bool IsRenderingEdges;

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
    _pointsPool = new GameObjectPool(PointPrefab, 50);
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

  // Private methods
  private void SetGameRunning(bool isRunning)
  {
    _isGameRunning = isRunning;
  }

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
    if (NoiseScriptableObject is RandomNoise3D_SO noise3DSO)
    {
      _noiseMap = noise3DSO.GenerateNoiseMap(Width + 1, Height + 1, Depth + 1);
      return;
    }

    float[,] noiseMap2d = NoiseScriptableObject.GenerateNoiseMap(
      Width + 1,
      Depth + 1);

    _noiseMap = NoiseUtils.Convert2DTo3D(noiseMap2d, Height + 1);
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
      _pointsPool.FinishedUsingAllItems();
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
    return MarchingCubes.GenerateMesh(_noiseMap, Threshold, IsRenderingEdges);
  }
}
