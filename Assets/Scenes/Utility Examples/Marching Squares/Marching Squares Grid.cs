using UnityEngine;

public class MarchingSquaresGrid : MonoBehaviour
{
  public GameObject prefab;
  public RandomNoise_SO noiseScriptableObject;
  public int width;
  public int height;
  public float threshold;
  private float[,] noiseMap;

  // Start is called before the first frame update
  void Start()
  {
    noiseMap = noiseScriptableObject.GenerateNoiseMap(width, height);
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        GameObject point = Instantiate(prefab);
        point.transform.parent = transform;
        point.transform.position = new Vector3(x, 0f, y);
      }
    }
  }
}
