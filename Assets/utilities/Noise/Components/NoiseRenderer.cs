using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseRenderer : MonoBehaviour
{
  [SerializeField]
  private Noise_SO noiseScriptableObject;

  private Renderer _renderer;
  private float[,] _noiseMap;

  void Start()
  {
    generateTexture();
  }

  void OnValidate()
  {
    generateTexture();
  }

  void generateTexture()
  {
    _noiseMap = NoiseGeneration.GenerateNoiseMap(noiseScriptableObject);
    Texture2D texture = TextureGenerator.TextureFromHeightMap(_noiseMap);

    _renderer = this.GetComponent<Renderer>();
    _renderer.material.mainTexture = texture;
  }
}
