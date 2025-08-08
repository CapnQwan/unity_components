using UnityEngine;

// Lookup Tables for Marching Cubes
//
// These tables differ from the original paper (Marching Cubes: A High Resolution 3D Surface Construction Algorithm)
//
// The co-ordinate system has the more convenient properties:
//
//    i = cube index [0, 7]
//    x = (i & 1) >> 0
//    y = (i & 2) >> 1
//    z = (i & 4) >> 2
//
// Axes are:
//
//      y
//      |     z
//      |   /
//      | /
//      +----- x
//
// Vertex and edge layout:
//
//            5             6
//            +-------------+
//          / |           / |
//        /   |         /   |
//    4 +-----+-------+  7  |
//      |   1 +-------+-----+ 3
//      |   /         |   /
//      | /           | /
//    0 +-------------+ 2
//
// Triangulation cases are generated prioritising rotations over inversions, which can introduce non-manifold geometry.
//
public static class MarchingCubesLookupTable
{
  public static Vector3[] Verticies = new Vector3[]
  {
      new Vector3(0f, 0f, 0f), // 0
      new Vector3(0f, 0f, 1f), // 1
      new Vector3(1f, 0f, 0f), // 2
      new Vector3(1f, 0f, 1f), // 3
      new Vector3(0f, 1f, 0f), // 4
      new Vector3(0f, 1f, 1f), // 5
      new Vector3(1f, 1f, 0f), // 6
      new Vector3(1f, 1f, 1f), // 7
  };

  public static int[][] PolygonsIndices = new int[][]
  {
      new int[] { 0, 2, 3, 7 }, // Case 0
      new int[] { 0, 2, 6, 7 }, // Case 1
      new int[] { 0, 4, 6, 7 }, // Case 2
      new int[] { 0, 6, 1, 2 }, // Case 3
      new int[] { 0, 6, 1, 4 }, // Case 4
      new int[] { 5, 6, 1, 4 }, // Case 5
  };
}
