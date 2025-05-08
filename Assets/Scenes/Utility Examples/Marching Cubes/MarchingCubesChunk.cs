using UnityEngine;

public class MarchingCubesChunk : MonoBehaviour
{
  public int chunkCountX = 4;
  public int chunkCountY = 2;
  public int chunkCountZ = 4;
  public int chunkWidth = 16;
  public int chunkDepth = 16;
  public int chunkHeight = 32;
  public float threshold = 0.5f;
  public PerlinNoise3D_SO noiseScriptableObject;

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

    Vector3 offset = new Vector3(
      x * chunkWidth,
      y * chunkHeight,
      z * chunkDepth);
    float[,,] noiseMap = noiseScriptableObject.GenerateNoiseMap(
      chunkWidth + 1,
      chunkHeight + 1,
      chunkDepth + 1,
      offset);

    MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
    meshFilter.mesh = GenerateChunkMesh(noiseMap);

    MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
    meshRenderer.material = new Material(Shader.Find("Standard"));

    chunk.transform.parent = transform;
  }

  private Mesh GenerateChunkMesh(float[,,] noiseMap)
  {
    Mesh mesh = MarchingCubes.GenerateMesh(chunkWidth, chunkHeight, chunkDepth, noiseMap, threshold);
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
