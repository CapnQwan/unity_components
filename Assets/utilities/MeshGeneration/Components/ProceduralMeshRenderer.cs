using UnityEngine;

/// <summary>
/// Component for generating procedural meshes and rendering.
/// </summary>
public class ProceduralMeshRenderer : MonoBehaviour
{
  [SerializeField] private Mesh_SO meshScriptableObject;
  [SerializeField] private bool isRenderedInEditor;
  private MeshFilter _meshFilter;
  private MeshRenderer _meshRenderer;
  private Mesh _mesh;

  void Awake()
  {
    setupRendering();
    UpdateMesh();
  }

  void OnValidate()
  {
    if (isRenderedInEditor)
    {
      setupRendering();
      UpdateMesh();
    }
    if (!isRenderedInEditor)
    {
      DisableRendering();
    }
  }

  private void UpdateMesh()
  {
    if (_meshFilter)
    {
      _mesh = meshScriptableObject.GenerateMesh();
      _meshFilter.mesh = _mesh;
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

    if (meshScriptableObject != null)
    {
      meshScriptableObject.OnValuesChanged += UpdateMesh;
    }
  }

  private void DisableRendering()
  {
    if (_meshFilter != null)
    {
      Destroy(_meshFilter);
    }

    if (_meshRenderer != null)
    {
      Destroy(_meshRenderer);
    }
  }
}
