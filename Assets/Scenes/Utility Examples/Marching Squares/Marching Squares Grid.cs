using System.Collections.Generic;
using UnityEngine;

public class MarchingSquaresGrid : MonoBehaviour
{
  public GameObject Prefab;
  public RandomNoise_SO NoiseScriptableObject;
  public int Width;
  public int Height;
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
        float noiseValue = _noiseMap[x + 1, y + 1];

        GameObject point = _pointsPool.GetItem();
        point.transform.parent = transform;
        point.transform.position = new Vector3(x, 0f, y);

        MeshRenderer meshRenderer = point.GetComponent<MeshRenderer>();
        meshRenderer.material.color = noiseValue > Threshold ? Color.black : Color.white;
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
        MarchingSquaresSegment segment = GenerateMeshSection(x, y);

        for (int i = 0; i < segment.Triangles.Length; i++)
        {
          segment.Triangles[i] += vertices.Count;
        }

        vertices.AddRange(segment.Vertices);
        triangles.AddRange(segment.Triangles);
      }
    }

    // Assign the generated data to the mesh.
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.normals = normals.ToArray();

    return mesh;
  }

  private MarchingSquaresSegment GenerateMeshSection(int x, int y)
  {
    // convert the values to a binary based number where each bool repusents a bit
    int caseIndex = (_noiseMap[x, y] > Threshold ? 1 : 0)
               | (_noiseMap[x + 1, y] > Threshold ? 1 : 0) << 1
               | (_noiseMap[x, y + 1] > Threshold ? 1 : 0) << 2
               | (_noiseMap[x + 1, y + 1] > Threshold ? 1 : 0) << 3;

    return CreateSegment(x, y, caseIndex);
  }

  private MarchingSquaresSegment CreateSegment(int x, int y, int caseIndex)
  {
    (int[] triangles, float[] vertexOffsets) = SegmentConfigs[caseIndex];

    // Generate Vertices
    Vector3[] vertices = new Vector3[vertexOffsets.Length / 3];
    for (int i = 0; i < vertexOffsets.Length; i += 3)
    {
      vertices[i / 3] = new Vector3(x + vertexOffsets[i], vertexOffsets[i + 1], y + vertexOffsets[i + 2]);
    }

    return new MarchingSquaresSegment(vertices, triangles);
  }

  public struct MarchingSquaresSegment
  {
    public Vector3[] Vertices;
    public int[] Triangles;

    public MarchingSquaresSegment(
      Vector3[] vertices,
      int[] triangles)
    {
      Vertices = vertices;
      Triangles = triangles;
    }
  }

  private static readonly Dictionary<int, (int[], float[])> SegmentConfigs = new()
{
    { 0, (new int[0], new float[0]) },
    {
      1, (new int[] { 0, 2, 1 },
          new float[]
          {
            0f, 0f, 0f,
            0.5f, 0f, 0f,
            0f, 0f, 0.5f,
          })
    },
    {
      2, (new int[] { 0, 1, 2 },
          new float[]
          {
            1f, 0f, 0f,
            0.5f, 0f, 0f,
            1f, 0f, 0.5f,
          })
    },
    {
      3, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0f, 0f, 0f,
            0f, 0f, 0.5f,
            1f, 0f, 0f,
            1f, 0f, 0.5f,
          })
    },
    {
      4, (new int[] { 0, 1, 2 },
          new float[]
          {
            0f, 0f, 1f,
            0.5f, 0f, 1f,
            0f, 0f, 0.5f,
          })
    },
    {
      5, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0f, 0f, 0f,
            0f, 0f, 1f,
            0.5f, 0f, 0f,
            0.5f, 0f, 1f,
          })
    },
    {
      6, (new int[] { 0, 1, 2, 3, 5, 4 },
          new float[]
          {
            0f, 0f, 0.5f,
            0f, 0f, 1f,
            0.5f, 0f, 1f,
            0.5f, 0f, 0f,
            1f, 0f, 0f,
            1f, 0f, 0.5f,
          })
    },
    {
      7, (new int[] { 0, 2, 1, 1, 3, 5, 2, 4, 3, 3, 4, 5 },
          new float[]
          {
            0f, 0f, 0f,
            1f, 0f, 0f,
            0f, 0f, 1f,
            0.5f, 0f, 0.5f,
            0.5f, 0f, 1f,
            1f, 0f, 0.5f,
          })
    },
    {
      8, (new int[] { 0, 1, 2 },
          new float[]
          {
            0.5f, 0f, 1f,
            1f, 0f, 1f,
            1f, 0f, 0.5f,
          })
    },
    {
      9, (new int[] { 0, 1, 2, 3, 4, 5 },
          new float[]
          {
            0f, 0f, 0f,
            0f, 0f, 0.5f,
            0.5f, 0f, 0f,
            0.5f, 0f, 1f,
            1f, 0f, 1f,
            1f, 0f, 0.5f,
          })
    },
    {
      10, (new int[] { 0, 1, 2, 1, 3, 2 },
          new float[]
          {
            0.5f, 0f, 0f,
            0.5f, 0f, 1f,
            1f, 0f, 0f,
            1f, 0f, 1f,
          })
    },
    {
      11, (new int[] { 0, 2, 1, 0, 5, 3, 2, 3, 4, 3, 5, 4 },
          new float[]
          {
            0f, 0f, 0f,
            1f, 0f, 0f,
            1f, 0f, 1f,
            0.5f, 0f, 0.5f,
            0.5f, 0f, 1f,
            0f, 0f, 0.5f,
          })
    },
    {
      12, (new int[] { 0, 1, 2, 1, 3, 2 },
          new float[]
          {
            0f, 0f, 0.5f,
            0f, 0f, 1f,
            1f, 0f, 0.5f,
            1f, 0f, 1f,
          })
    },
    {
      13, (new int[] { 0, 1, 2, 0, 3, 4, 2, 5, 3, 3, 5, 4 },
          new float[]
          {
            0f, 0f, 0f,
            0f, 0f, 1f,
            1f, 0f, 1f,
            0.5f, 0f, 0.5f,
            0.5f, 0f, 0f,
            1f, 0f, 0.5f,
          })
    },
    {
       14, (new int[] { 0, 2, 1, 0, 4, 3, 2, 3, 5, 3, 4, 5 },
          new float[]
          {
            1f, 0f, 0f,
            1f, 0f, 1f,
            0f, 0f, 1f,
            0.5f, 0f, 0.5f,
            0.5f, 0f, 0f,
            0f, 0f, 0.5f,
          })
    },
    {
      15, (new int[] { 0, 1, 2, 1, 3, 2 },
          new float[]
          {
            0f, 0f, 0f,
            0f, 0f, 1f,
            1f, 0f, 0f,
            1f, 0f, 1f,
          })
    },
};

  public void SetGameRunning(bool isRunning)
  {
    _isGameRunning = isRunning;
  }
}
