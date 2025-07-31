using UnityEngine;

public class MarchingCubesChunk : MonoBehaviour
{
  [SerializeField]
  private int chunkCountX = 4;
  [SerializeField]
  private int chunkCountY = 2;
  [SerializeField]
  private int chunkCountZ = 4;
  [SerializeField]
  private int chunkWidth = 16;
  [SerializeField]
  private int chunkDepth = 16;
  [SerializeField]
  private int chunkHeight = 32;
  [SerializeField]
  private float threshold = 0.5f;
  [SerializeField]
  private RandomNoise_SO noiseScriptableObject;

  private bool _isGameRunning = false;

  public void Start()
  {
    _isGameRunning = true;
    GenerateChunks();
  }

  public void OnValidate()
  {
    if (_isGameRunning)
    {
      GenerateChunks();
    }
  }

  private void GenerateChunks()
  {
    ClearChunks();

    for (int x = 0; x < chunkCountX; x++)
    {
      for (int y = 0; y < chunkCountY; y++)
      {
        for (int z = 0; z < chunkCountZ; z++)
        {
          CreateChunk(x, y, z);
        }
      }
    }
  }

  private void CreateChunk(int x, int y, int z)
  {
    GameObject chunk = new GameObject($"Chunk_{x}_{y}_{z}");

    chunk.transform.parent = transform;
    chunk.transform.localPosition = new Vector3(x * chunkWidth, y * chunkHeight, z * chunkDepth);

    Vector2 offset = new Vector2(x * chunkWidth, z * chunkDepth);
    float[,] noiseMap2d = noiseScriptableObject.GenerateNoiseMap(
      chunkWidth + 1,
      chunkHeight + 1,
      offset);
    float[,,] noiseMap = NoiseUtils.Convert2DTo3D(noiseMap2d, chunkHeight);

    MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
    meshFilter.mesh = GenerateChunkMesh(noiseMap);

    MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
    meshRenderer.material = new Material(Shader.Find("Standard"));

    chunk.transform.parent = transform;
  }

  private Mesh GenerateChunkMesh(float[,,] noiseMap)
  {
    Mesh mesh = MarchingCubes.GenerateMesh(noiseMap, threshold);
    return mesh;
  }

  private void ClearChunks()
  {
    foreach (Transform child in transform)
    {
      Destroy(child.gameObject);
    }
  }
}
