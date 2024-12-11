using UnityEngine;

/// <summary>
/// A scriptable object for storing parameters and generating a grid.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mesh/grid", order = 6)]
public class GridMesh_SO : Mesh_SO
{
  /// <summary>
  /// The width of the quad.
  /// </summary>
  [SerializeField]
  private float width = 1f;

  /// <summary>
  /// The height of the quad.
  /// </summary>
  [SerializeField]
  private float height = 1f;

  /// <summary>
  /// The number of the segments or faces a cross the width of the grid.
  /// </summary>
  [SerializeField]
  private int segmentsX = 1;

  /// <summary>
  /// The number of the segments or faces a cross the height of the grid.
  /// </summary>
  [SerializeField]
  private int segmentsY = 1;

  /// <summary>
  /// How much the height map will increase / decrease the y value of the grid.
  /// </summary>
  [SerializeField]
  private float heightMapScale = 1f;

  /// <summary>
  /// The height map for offsetting the verticies of the grid.
  /// </summary>
  [SerializeField]
  private RandomNoise_SO heightMap;

  /// <summary>
  /// Gets the width of the quad.
  /// </summary>
  public float Width => this.width;

  /// <summary>
  /// Gets the height of the quad.
  /// </summary>
  public float Height => this.height;

  /// <summary>
  /// Gets the number of the segments or faces a cross the width of the grid.
  /// </summary>
  public int SegmentsX => this.segmentsX;

  /// <summary>
  /// Gets the number of the segments or faces a cross the height of the grid.
  /// </summary>
  public int SegmentsY => this.segmentsY;

  /// <summary>
  /// Gets the scale for increasing / decreasing the height of the grid.
  /// </summary>
  public float HeightMapScale => this.heightMapScale;

  /// <summary>
  /// Gets the height map for the mesh.
  /// </summary>
  public RandomNoise_SO HeightMap => this.heightMap;

  /// <summary>
  /// Generates a simple quad mesh.
  /// </summary>
  /// <returns>A simple quad mesh.</returns>
  public override Mesh GenerateMesh()
  {
    return GridMeshGenerator.GenerateGridMesh(this);
  }
}
