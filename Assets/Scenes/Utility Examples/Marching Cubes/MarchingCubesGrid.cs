using TMPro;
using UnityEngine;

public class MarchingCubesGrid : MonoBehaviour
{
  // Public fields
  public GameObject PointPrefab;
  public GameObject TextPrefab;
  public PerlinNoise3D_SO NoiseScriptableObject;
  public int Width;
  public int Height;
  public int Depth;
  public float Threshold;
  public bool IsRenderingPoints;
  public bool IsRenderingText;
  public bool IsRenderingMesh;

  // Private fields
  private float[,,] _noiseMap;
  private bool _isGameRunning = false;
  private GameObjectPool _pointsPool;
  private GameObjectPool _textPool;
  private MeshFilter _meshFilter;
  private MeshRenderer _meshRenderer;
  private Mesh _mesh;

  // Public methods
  public void Awake()
  {
    _pointsPool = new GameObjectPool(PointPrefab, 50);
    _textPool = new GameObjectPool(TextPrefab, 50);
  }

  public void Start()
  {
    SetupRendering();
    UpdateNoiseMap();
    GeneratePoints();
    GenerateCaseIndexIndicator();
    UpdateMesh();
    SetGameRunning(true);
  }

  public void OnValidate()
  {
    if (_isGameRunning)
    {
      UpdateNoiseMap();
      GeneratePoints();
      GenerateCaseIndexIndicator();
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

  private void GenerateCaseIndexIndicator()
  {
    if (!IsRenderingText)
    {
      _textPool.FinishedUsingAllItems();
      return;
    }

    for (int x = 0; x < Width; x++)
    {
      for (int y = 0; y < Height; y++)
      {
        for (int z = 0; z < Depth; z++)
        {
          float[] scalarValues = MarchingCubes.GetScalarValues(_noiseMap, x, y, z);
          int caseIndex = MarchingCubes.GetCaseIndex(scalarValues, Threshold);

          GameObject textObject = _textPool.GetItem();
          textObject.transform.parent = transform;
          textObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);

          TextMeshPro textMeshPro = textObject.GetComponent<TextMeshPro>();
          textMeshPro.text = caseIndex.ToString();
        }
      }
    }
  }

  private Mesh GenerateMesh()
  {
    return MarchingCubes.GenerateMesh(_noiseMap, Threshold);
  }
}
