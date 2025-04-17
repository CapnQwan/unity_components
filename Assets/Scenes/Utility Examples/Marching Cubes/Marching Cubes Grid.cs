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
    // TODO: update noise map to be a 3d noise map with a z value

    float bl1 = _noiseMap[x, y]; // Front bottom left
    float bl2 = _noiseMap[x + 1, y]; // Back bottom left
    float br1 = _noiseMap[x, y + 1]; // Front bottom right
    float br2 = _noiseMap[x + 1, y + 1]; // Back bottom right
    float tl1 = _noiseMap[x, y]; // Front top left
    float tl2 = _noiseMap[x + 1, y]; // Back top left
    float tr1 = _noiseMap[x, y + 1]; // Front top right
    float tr2 = _noiseMap[x + 1, y + 1]; // Back top right

    //TODO: Used z to determine if the value truely is above the threshold

    float tempThresholdA = Threshold + z * 0.02f;
    float tempThresholdB = Threshold + (z + 1) * 0.02f;

    // convert the values to a binary based number where each bool repusents a bit
    int caseIndex = (bl1 > tempThresholdA ? 1 : 0)
           | (bl2 > tempThresholdA ? 1 : 0) << 1
           | (br1 > tempThresholdA ? 1 : 0) << 2
           | (br2 > tempThresholdA ? 1 : 0) << 3
           | (tl1 > tempThresholdB ? 1 : 0) << 4
           | (tl2 > tempThresholdB ? 1 : 0) << 5
           | (tr1 > tempThresholdB ? 1 : 0) << 6
           | (tr2 > tempThresholdB ? 1 : 0) << 7;

    HashSet<int> setupIndicies = new() { 0, 1, 2, 3, 4, 5, 17, 21, 34, 51, 55, 85, 119, 170, 187, 221, 238, 255 };
    if (!setupIndicies.Contains(caseIndex))
    {
      Debug.Log(caseIndex);
    }

    HashSet<int> validCaseIndices = new() { 223 };
    if (!validCaseIndices.Contains(caseIndex))
    {
      return CreateSegment(x, y, z, 0);
    }

    return CreateSegment(x, y, z, caseIndex);
  }

  private MarchingCubesSegment CreateSegment(int x, int y, int z, int caseIndex)
  {
    (int[] triangles, float[] vertexOffsets) = SegmentConfigs[caseIndex];

    // Generate Vertices
    Vector3[] vertices = new Vector3[vertexOffsets.Length / 3];
    for (int i = 0; i < vertexOffsets.Length; i += 3)
    {
      vertices[i / 3] = new Vector3(x + vertexOffsets[i], y + vertexOffsets[i + 1], z + vertexOffsets[i + 2]);
    }

    int[] tris = new int[triangles.Length];
    for (int i = 0; i < triangles.Length; i++)
    {
      tris[i] = triangles[i];
    }

    return new MarchingCubesSegment(vertices, tris);
  }

  public void SetGameRunning(bool isRunning)
  {
    _isGameRunning = isRunning;
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

  private static readonly Dictionary<int, (int[], float[])> SegmentConfigs = new()
  {
    { 0, (new int[0], new float[0]) },
    {
      1, (new int[] { 0, 1, 2 },
          new float[]
          {
            0.5f, 0f, 0f,
            0f, 0.5f, 0f,
            0f, 0f, 0.5f,
          })
    },
    {
      2, (new int[] { 0, 1, 2 },
          new float[]
          {
            1f, 0f, 0.5f,
            1f, 0.5f, 0f,
            0.5f, 0f, 0f,
          })
    },
    {
      3, (new int[] { 0, 1, 2, 0, 2, 3 },
          new float[]
          {
            0f, 0.5f, 0f,
            0f, 0f, 0.5f,
            1f, 0f, 0.5f,
            1f, 0.5f, 0f,
          })
    },
    {
      4, (new int[] { 0, 1, 2 },
          new float[]
          {
            0f, 1f, 0.5f,
            0f, 0.5f, 0f,
            0.5f, 1f, 0f,
          })
    },
    {
      5, (new int[] { 0, 2, 1, 1, 2, 3 },
          new float[]
          {
            0f, 0f, 0.5f,
            0f, 1f, 0.5f,
            0.5f, 0f, 0f,
            0.5f, 1f, 0f,
          })
    },
    { 6, (new int[0], new float[0]) },
    { 7, (new int[0], new float[0]) },
    { 8, (new int[0], new float[0]) },
    { 9, (new int[0], new float[0]) },
    { 10, (new int[0], new float[0]) },
    { 11, (new int[0], new float[0]) },
    { 12, (new int[0], new float[0]) },
    { 13, (new int[0], new float[0]) },
    { 14, (new int[0], new float[0]) },
    { 15, (new int[0], new float[0]) },
    { 16, (new int[0], new float[0]) },
    {
      17, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0.5f, 0f, 0f,
            0f, 0.5f, 0f,
            0.5f, 0f, 1f,
            0f, 0.5f, 1f,
          })
    },
    { 18, (new int[0], new float[0]) },
    { 19, (new int[0], new float[0]) },
    { 20, (new int[0], new float[0]) },
    {
      21, (new int[] { 0, 1, 2, 2, 3, 4 },
          new float[]
          {
            0.5f, 0f, 0f,
            0f, 0.5f, 0f,
            0f, 0f, 0.5f,
            0f, 0.5f, 1f,
            0.5f, 0f, 1f,
          })
    },
    { 22, (new int[0], new float[0]) },
    { 23, (new int[0], new float[0]) },
    { 24, (new int[0], new float[0]) },
    { 25, (new int[0], new float[0]) },
    { 26, (new int[0], new float[0]) },
    { 27, (new int[0], new float[0]) },
    { 28, (new int[0], new float[0]) },
    { 29, (new int[0], new float[0]) },
    { 30, (new int[0], new float[0]) },
    { 31, (new int[0], new float[0]) },
    { 32, (new int[0], new float[0]) },
    { 33, (new int[0], new float[0]) },
    {
      34, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            1f, 0.5f, 0f,
            0.5f, 0f, 0f,
            1f, 0.5f, 1f,
            0.5f, 0f, 1f,
          })
    },
    { 35, (new int[0], new float[0]) },
    { 36, (new int[0], new float[0]) },
    { 37, (new int[0], new float[0]) },
    { 38, (new int[0], new float[0]) },
    { 39, (new int[0], new float[0]) },
    { 40, (new int[0], new float[0]) },
    { 41, (new int[0], new float[0]) },
    { 42, (new int[0], new float[0]) },
    { 43, (new int[0], new float[0]) },
    { 44, (new int[0], new float[0]) },
    { 45, (new int[0], new float[0]) },
    { 46, (new int[0], new float[0]) },
    { 47, (new int[0], new float[0]) },
    { 48, (new int[0], new float[0]) },
    { 49, (new int[0], new float[0]) },
    { 50, (new int[0], new float[0]) },
    {
      51, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0f, 0.5f, 0f,
            0f, 0.5f, 1f,
            1f, 0.5f, 0f,
            1f, 0.5f, 1f,
          })
    },
    { 52, (new int[0], new float[0]) },
    { 53, (new int[0], new float[0]) },
    { 54, (new int[0], new float[0]) },
    {
      55, (new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 },
          new float[]
          {
            0.5f, 0.5f, 0f,
            0f, 1f, 0f,
            0f, 0.5f, 0.5f,
            0f, 0.5f, 1f,
            1f, 0.5f, 1f,
            1f, 0.5f, 0f,
          })
    },
    { 56, (new int[0], new float[0]) },
    { 57, (new int[0], new float[0]) },
    { 58, (new int[0], new float[0]) },
    { 59, (new int[0], new float[0]) },
    { 60, (new int[0], new float[0]) },
    { 61, (new int[0], new float[0]) },
    { 62, (new int[0], new float[0]) },
    { 63, (new int[0], new float[0]) },
    { 64, (new int[0], new float[0]) },
    { 65, (new int[0], new float[0]) },
    { 66, (new int[0], new float[0]) },
    { 67, (new int[0], new float[0]) },
    {
      68, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0f, 0.5f, 0f,
            0.5f, 1f, 0f,
            0f, 0.5f, 1f,
            0.5f, 1f, 1f,
          })
    },
    { 69, (new int[0], new float[0]) },
    { 70, (new int[0], new float[0]) },
    { 71, (new int[0], new float[0]) },
    { 72, (new int[0], new float[0]) },
    { 73, (new int[0], new float[0]) },
    { 74, (new int[0], new float[0]) },
    { 75, (new int[0], new float[0]) },
    { 76, (new int[0], new float[0]) },
    { 77, (new int[0], new float[0]) },
    { 78, (new int[0], new float[0]) },
    { 79, (new int[0], new float[0]) },
    { 80, (new int[0], new float[0]) },
    { 81, (new int[0], new float[0]) },
    { 82, (new int[0], new float[0]) },
    { 83, (new int[0], new float[0]) },
    { 84, (new int[0], new float[0]) },
    {
      85, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0.5f, 0f, 0f,
            0.5f, 1f, 0f,
            0.5f, 0f, 1f,
            0.5f, 1f, 1f,
          })
    },
    { 86, (new int[0], new float[0]) },
    { 87, (new int[0], new float[0]) },
    { 88, (new int[0], new float[0]) },
    { 89, (new int[0], new float[0]) },
    { 90, (new int[0], new float[0]) },
    { 91, (new int[0], new float[0]) },
    { 92, (new int[0], new float[0]) },
    { 93, (new int[0], new float[0]) },
    { 94, (new int[0], new float[0]) },
    { 95, (new int[0], new float[0]) },
    { 96, (new int[0], new float[0]) },
    { 97, (new int[0], new float[0]) },
    { 98, (new int[0], new float[0]) },
    { 99, (new int[0], new float[0]) },
    { 100, (new int[0], new float[0]) },
    { 101, (new int[0], new float[0]) },
    { 102, (new int[0], new float[0]) },
    { 103, (new int[0], new float[0]) },
    { 104, (new int[0], new float[0]) },
    { 105, (new int[0], new float[0]) },
    { 106, (new int[0], new float[0]) },
    { 107, (new int[0], new float[0]) },
    { 108, (new int[0], new float[0]) },
    { 109, (new int[0], new float[0]) },
    { 110, (new int[0], new float[0]) },
    { 111, (new int[0], new float[0]) },
    { 112, (new int[0], new float[0]) },
    { 113, (new int[0], new float[0]) },
    { 114, (new int[0], new float[0]) },
    { 115, (new int[0], new float[0]) },
    { 116, (new int[0], new float[0]) },
    { 117, (new int[0], new float[0]) },
    { 118, (new int[0], new float[0]) },
    {
      119, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            1f, 0.5f, 0f,
            0.5f, 1f, 0f,
            1f, 0.5f, 1f,
            0.5f, 1f, 1f,
          })
    },
    { 120, (new int[0], new float[0]) },
    { 121, (new int[0], new float[0]) },
    { 122, (new int[0], new float[0]) },
    { 123, (new int[0], new float[0]) },
    { 124, (new int[0], new float[0]) },
    { 125, (new int[0], new float[0]) },
    { 126, (new int[0], new float[0]) },
    { 127, (new int[0], new float[0]) },
    { 128, (new int[0], new float[0]) },
    { 129, (new int[0], new float[0]) },
    { 130, (new int[0], new float[0]) },
    { 131, (new int[0], new float[0]) },
    { 132, (new int[0], new float[0]) },
    { 133, (new int[0], new float[0]) },
    { 134, (new int[0], new float[0]) },
    { 135, (new int[0], new float[0]) },
    { 136, (new int[0], new float[0]) },
    { 137, (new int[0], new float[0]) },
    { 138, (new int[0], new float[0]) },
    { 139, (new int[0], new float[0]) },
    { 140, (new int[0], new float[0]) },
    { 141, (new int[0], new float[0]) },
    { 142, (new int[0], new float[0]) },
    { 143, (new int[0], new float[0]) },
    { 144, (new int[0], new float[0]) },
    { 145, (new int[0], new float[0]) },
    { 146, (new int[0], new float[0]) },
    { 147, (new int[0], new float[0]) },
    { 148, (new int[0], new float[0]) },
    { 149, (new int[0], new float[0]) },
    { 150, (new int[0], new float[0]) },
    { 151, (new int[0], new float[0]) },
    { 152, (new int[0], new float[0]) },
    { 153, (new int[0], new float[0]) },
    { 154, (new int[0], new float[0]) },
    { 155, (new int[0], new float[0]) },
    { 156, (new int[0], new float[0]) },
    { 157, (new int[0], new float[0]) },
    { 158, (new int[0], new float[0]) },
    { 159, (new int[0], new float[0]) },
    { 160, (new int[0], new float[0]) },
    { 161, (new int[0], new float[0]) },
    { 162, (new int[0], new float[0]) },
    { 163, (new int[0], new float[0]) },
    { 164, (new int[0], new float[0]) },
    { 165, (new int[0], new float[0]) },
    { 166, (new int[0], new float[0]) },
    { 167, (new int[0], new float[0]) },
    { 168, (new int[0], new float[0]) },
    { 169, (new int[0], new float[0]) },
    {
      170, (new int[] { 0, 2, 1, 1, 2, 3 },
          new float[]
          {
            0.5f, 0f, 0f,
            0.5f, 1f, 0f,
            0.5f, 0f, 1f,
            0.5f, 1f, 1f,
          })
    },
    { 171, (new int[0], new float[0]) },
    { 172, (new int[0], new float[0]) },
    { 173, (new int[0], new float[0]) },
    { 174, (new int[0], new float[0]) },
    { 175, (new int[0], new float[0]) },
    { 176, (new int[0], new float[0]) },
    { 177, (new int[0], new float[0]) },
    { 178, (new int[0], new float[0]) },
    { 179, (new int[0], new float[0]) },
    { 180, (new int[0], new float[0]) },
    { 181, (new int[0], new float[0]) },
    { 182, (new int[0], new float[0]) },
    { 183, (new int[0], new float[0]) },
    { 184, (new int[0], new float[0]) },
    { 185, (new int[0], new float[0]) },
    { 186, (new int[0], new float[0]) },
    {
      187, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0.5f, 1f, 0f,
            0f, 0.5f, 0f,
            0.5f, 1f, 1f,
            0f, 0.5f, 1f,
          })
    },
    { 188, (new int[0], new float[0]) },
    { 189, (new int[0], new float[0]) },
    { 190, (new int[0], new float[0]) },
    { 191, (new int[0], new float[0]) },
    { 192, (new int[0], new float[0]) },
    { 193, (new int[0], new float[0]) },
    { 194, (new int[0], new float[0]) },
    { 195, (new int[0], new float[0]) },
    { 196, (new int[0], new float[0]) },
    { 197, (new int[0], new float[0]) },
    { 198, (new int[0], new float[0]) },
    { 199, (new int[0], new float[0]) },
    { 200, (new int[0], new float[0]) },
    { 201, (new int[0], new float[0]) },
    { 202, (new int[0], new float[0]) },
    { 203, (new int[0], new float[0]) },
    { 204, (new int[0], new float[0]) },
    { 205, (new int[0], new float[0]) },
    { 206, (new int[0], new float[0]) },
    { 207, (new int[0], new float[0]) },
    { 208, (new int[0], new float[0]) },
    { 209, (new int[0], new float[0]) },
    { 210, (new int[0], new float[0]) },
    { 211, (new int[0], new float[0]) },
    { 212, (new int[0], new float[0]) },
    { 213, (new int[0], new float[0]) },
    { 214, (new int[0], new float[0]) },
    { 215, (new int[0], new float[0]) },
    { 216, (new int[0], new float[0]) },
    { 217, (new int[0], new float[0]) },
    { 218, (new int[0], new float[0]) },
    { 219, (new int[0], new float[0]) },
    { 220, (new int[0], new float[0]) },
    {
      221, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0.5f, 0f, 0f,
            1f, 0.5f, 0f,
            0.5f, 0f, 1f,
            1f, 0.5f, 1f,
          })
    },
    { 222, (new int[0], new float[0]) },
    {
      223, (new int[] { 0, 2, 1 },
          new float[]
          {
            1f, 0f, 0.5f,
            0.5f, 0f, 1f,
            1f, 0.5f, 1f,
          })
    },
    { 224, (new int[0], new float[0]) },
    { 225, (new int[0], new float[0]) },
    { 226, (new int[0], new float[0]) },
    { 227, (new int[0], new float[0]) },
    { 228, (new int[0], new float[0]) },
    { 229, (new int[0], new float[0]) },
    { 230, (new int[0], new float[0]) },
    { 231, (new int[0], new float[0]) },
    { 232, (new int[0], new float[0]) },
    { 233, (new int[0], new float[0]) },
    { 234, (new int[0], new float[0]) },
    { 235, (new int[0], new float[0]) },
    { 236, (new int[0], new float[0]) },
    { 237, (new int[0], new float[0]) },
    {
      238, (new int[] { 0, 1, 2, 2, 1, 3 },
          new float[]
          {
            0f, 0.5f, 0f,
            0.5f, 0f, 0f,
            0f, 0.5f, 1f,
            0.5f, 0f, 1f,
          })
    },
    { 239, (new int[0], new float[0]) },
    { 240, (new int[0], new float[0]) },
    { 241, (new int[0], new float[0]) },
    { 242, (new int[0], new float[0]) },
    { 243, (new int[0], new float[0]) },
    { 244, (new int[0], new float[0]) },
    { 245, (new int[0], new float[0]) },
    { 246, (new int[0], new float[0]) },
    { 247, (new int[0], new float[0]) },
    { 248, (new int[0], new float[0]) },
    { 249, (new int[0], new float[0]) },
    { 250, (new int[0], new float[0]) },
    { 251, (new int[0], new float[0]) },
    { 252, (new int[0], new float[0]) },
    { 253, (new int[0], new float[0]) },
    { 254, (new int[0], new float[0]) },
    { 255, (new int[0], new float[0]) },
  };
};
