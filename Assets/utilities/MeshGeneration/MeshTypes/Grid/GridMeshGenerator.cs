using System;
using UnityEngine;

public static class GridMeshGenerator
{
  public static Mesh GenerateGridMesh(GridMesh_SO meshScriptableObject)
  {
    float[,] heightMap = meshScriptableObject.HeightMap?.GenerateNoiseMap();
    return GenerateGridMesh(
      meshScriptableObject.Width,
      meshScriptableObject.Height,
      meshScriptableObject.SegmentsX,
      meshScriptableObject.SegmentsY,
      meshScriptableObject.HeightMapScale,
      heightMap);
  }

  public static Mesh GenerateGridMesh(
    float width,
    float height,
    int segmentsX,
    int segmentsY,
    float heightMapScale,
    float[,] noiseMap = null)
  {
    if (width <= 0 || height <= 0 || segmentsX <= 0 || segmentsY <= 0)
    {
      throw new ArgumentException("Width and height must be greater than 0.");
    }

    noiseMap ??= new float[segmentsX + 1, segmentsY + 1];

    Mesh mesh = new Mesh
    {
      name = $"Grid_{width}x{height}",
    };

    int xVerticies = segmentsX + 1;
    int yVerticies = segmentsY + 1;
    int vertexCount = xVerticies * yVerticies;
    int triPointsCount = segmentsX * segmentsY * 6;
    float vertexWidth = width / segmentsX;
    float vertexHeight = height / segmentsY;

    Vector3[] vertices = new Vector3[vertexCount];
    int[] triangles = new int[triPointsCount];
    Vector2[] uvs = new Vector2[vertexCount];
    Vector3[] normals = new Vector3[vertexCount];

    for (int x = 0, i = 0, t = 0; x < xVerticies; x++)
    {
      for (int y = 0; y < yVerticies; y++, i++)
      {
        vertices[i] = new Vector3(x * vertexWidth, noiseMap[x, y] * heightMapScale, y * vertexHeight);
        uvs[i] = new Vector2(x / xVerticies, y / yVerticies);
        normals[i] = Vector3.back;

        if (x != segmentsX && y != segmentsY)
        {
          triangles[t * 6] = i;
          triangles[t * 6 + 1] = i + 1;
          triangles[t * 6 + 2] = i + xVerticies;
          triangles[t * 6 + 3] = i + xVerticies;
          triangles[t * 6 + 4] = i + 1;
          triangles[t * 6 + 5] = i + xVerticies + 1;
          t++;
        }
      }
    }

    // Assign the generated data to the mesh.
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;
    mesh.normals = normals;

    return mesh;
  }
}
