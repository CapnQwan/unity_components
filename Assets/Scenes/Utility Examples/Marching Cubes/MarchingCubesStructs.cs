using UnityEngine;

public struct IsoGridCell
{
  public Vector3[] Vertices;
  public int[] IsoValues;

  public IsoGridCell(Vector3 position, int[] isoValues)
  {
    Vertices = new Vector3[]
    {
      position + MarchingCubesLookupTable.CellVertices[0],
      position + MarchingCubesLookupTable.CellVertices[1],
      position + MarchingCubesLookupTable.CellVertices[2],
      position + MarchingCubesLookupTable.CellVertices[3],
      position + MarchingCubesLookupTable.CellVertices[4],
      position + MarchingCubesLookupTable.CellVertices[5],
      position + MarchingCubesLookupTable.CellVertices[6],
      position + MarchingCubesLookupTable.CellVertices[7],
    };
    IsoValues = isoValues;
  }
}

public struct Triangle
{
  public Vector3 VertexA;
  public Vector3 VertexB;
  public Vector3 VertexC;

  public Triangle(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
  {
    VertexA = vertexA;
    VertexB = vertexB;
    VertexC = vertexC;
  }
}