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
        uvs.AddRange(segment.Uvs);
        normals.AddRange(segment.Vertices);
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
    float t1 = _noiseMap[x, y];
    float t2 = _noiseMap[x + 1, y];
    float b1 = _noiseMap[x, y + 1];
    float b2 = _noiseMap[x + 1, y + 1];

    // convert the values to a binary based number where each bool repusents a bit
    int number = (t1 > Threshold ? 1 : 0)
               | (t2 > Threshold ? 1 : 0) << 1
               | (b1 > Threshold ? 1 : 0) << 2
               | (b2 > Threshold ? 1 : 0) << 3;

    return number switch
    {
      0 => CreateSegment0(),
      1 => CreateSegment1(x, y),
      2 => CreateSegment2(x, y),
      3 => CreateSegment3(x, y),
      4 => CreateSegment4(x, y),
      5 => CreateSegment5(x, y),
      6 => CreateSegment6(x, y),
      7 => CreateSegment7(x, y),
      8 => CreateSegment8(x, y),
      9 => CreateSegment9(x, y),
      10 => CreateSegment10(x, y),
      11 => CreateSegment11(x, y),
      12 => CreateSegment12(x, y),
      13 => CreateSegment13(x, y),
      14 => CreateSegment14(x, y),
      15 => CreateSegment15(x, y),
      _ => CreateSegment0(),
    };
  }

  private MarchingSquaresSegment CreateSegment0()
  {
    return new MarchingSquaresSegment(
      new Vector3[0],
      new int[0],
      new Vector2[0],
      new Vector3[0]);
  }

  private MarchingSquaresSegment CreateSegment1(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x, 0f, y + 0.5f),
      },
      new int[] { 0, 2, 1 },
      new Vector2[] { },
      new Vector3[]
      {
        Vector3.up,
        Vector3.up,
        Vector3.up,
      });
  }

  private MarchingSquaresSegment CreateSegment2(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x + 1f, 0f, y),
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x + 1f, 0f, y + 0.5f),
      },
      new int[] { 0, 1, 2 },
      new Vector2[] { },
      new Vector3[]
      {
        Vector3.up,
        Vector3.up,
        Vector3.up,
      });
  }

  private MarchingSquaresSegment CreateSegment3(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x, 0f, y + 0.5f),
        new Vector3(x + 1f, 0f, y),
        new Vector3(x + 1f, 0f, y + 0.5f),
      },
      new int[] { 0, 1, 2, 2, 1, 3 },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment4(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y + 1f),
        new Vector3(x, 0f, y + 0.5f),
      },
      new int[] { 0, 1, 2 },
      new Vector2[] { },
      new Vector3[]
      {
        Vector3.up,
        Vector3.up,
        Vector3.up,
      });
  }

  private MarchingSquaresSegment CreateSegment5(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x + 0.5f, 0f, y + 1f),
      },
      new int[] { 0, 1, 2, 2, 1, 3 },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment6(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y + 0.5f),
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x + 1f, 0f, y),
        new Vector3(x + 1f, 0f, y + 0.5f),
      },
      new int[] { 0, 1, 2, 3, 5, 4 },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment7(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x + 1f, 0f, y),
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y + 0.5f),
        new Vector3(x + 0.5f, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y + 0.5f),
      },
      new int[]
      {
        0, 2, 1,
        1, 3, 5,
        2, 4, 3,
        3, 4, 5
      },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment8(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x + 0.5f, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y + 0.5f),
      },
      new int[] { 0, 1, 2, },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment9(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x, 0f, y + 0.5f),
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x + 0.5f, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y + 0.5f),
      },
      new int[] { 0, 1, 2, 3, 4, 5, },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment10(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x + 0.5f, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y),
        new Vector3(x + 1f, 0f, y + 1f),
      },
      new int[] { 0, 1, 2, 1, 3, 2, },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment11(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x + 1f, 0f, y),
        new Vector3(x + 1f, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y + 0.5f),
        new Vector3(x + 0.5f, 0f, y + 1f),
        new Vector3(x, 0f, y + 0.5f),
      },
      new int[]
      {
        0, 2, 1,
        0, 5, 3,
        2, 3, 4,
        3, 5, 4
      },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment12(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y + 0.5f),
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y + 0.5f),
        new Vector3(x + 1f, 0f, y + 1f),
      },
      new int[] { 0, 1, 2, 1, 3, 2, },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment13(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y + 0.5f),
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x + 1f, 0f, y + 0.5f),
      },
      new int[]
      {
        0, 1, 2,
        0, 3, 4,
        2, 5, 3,
        3, 5, 4
      },
      new Vector2[] { },
      new Vector3[] { });
  }

  private MarchingSquaresSegment CreateSegment14(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x + 1f, 0f, y),
        new Vector3(x + 1f, 0f, y + 1f),
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 0.5f, 0f, y + 0.5f),
        new Vector3(x + 0.5f, 0f, y),
        new Vector3(x, 0f, y + 0.5f),
      },
      new int[]
      {
        0, 2, 1,
        0, 4, 3,
        2, 3, 5,
        3, 4, 5
      },
      new Vector2[] { },
      new Vector3[] { });
  }


  private MarchingSquaresSegment CreateSegment15(int x, int y)
  {
    return new MarchingSquaresSegment(
      new Vector3[]
      {
        new Vector3(x, 0f, y),
        new Vector3(x, 0f, y + 1f),
        new Vector3(x + 1f, 0f, y),
        new Vector3(x + 1f, 0f, y + 1f),
      },
      new int[] { 0, 1, 2, 1, 3, 2, },
      new Vector2[] { },
      new Vector3[] { });
  }


  public struct MarchingSquaresSegment
  {
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] Uvs;
    public Vector3[] Normals;

    public MarchingSquaresSegment(
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
}
