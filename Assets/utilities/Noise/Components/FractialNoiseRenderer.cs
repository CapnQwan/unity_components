using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NoiseLayer
{
  [SerializeField]
  private Noise_SO noiseScriptableObject;
  public Noise_SO NoiseScriptableObject => noiseScriptableObject;
  [SerializeField]
  [Range(0, 1)]
  private float amplitude;
  public float Amplitude => amplitude;
}

public class FractialNoiseRenderer : MonoBehaviour
{
  [SerializeField]
  private NoiseLayer[] noiseLayers;

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
    _noiseMap = NoiseGeneration.GenerateFractalNoise(noiseLayers);
    Texture2D texture = TextureGenerator.TextureFromHeightMap(_noiseMap);

    _renderer = this.GetComponent<Renderer>();
    _renderer.material.mainTexture = texture;
  }
}
