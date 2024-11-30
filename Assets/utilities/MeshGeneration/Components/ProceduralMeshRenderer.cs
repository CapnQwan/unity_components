using UnityEngine;

/// <summary>
/// Component for generating procedural meshes and rendering.
/// </summary>
public class ProceduralMeshRenderer : MonoBehaviour
{
  [SerializeField] private Mesh_SO meshScriptableObject;
  private MeshFilter _meshFilter;
  private MeshRenderer _meshRenderer;
  private Mesh _mesh;

  void Awake()
  {
    this._meshFilter = this.gameObject.AddComponent<MeshFilter>();
    this._meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
    this._meshRenderer.material = new Material(Shader.Find("Standard"));
    this._mesh = this.meshScriptableObject.GenerateMesh();
    this._meshFilter.mesh = this._mesh;
  }
}
