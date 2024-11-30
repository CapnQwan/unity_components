using System;
using UnityEngine;

/// <summary>
/// Renders a noise map as a texture on the attached object's material using a noise map generator.
/// </summary>
public class NoiseRenderer : MonoBehaviour
{
  /// <summary>
  /// A ScriptableObject containing the parameters for generating the noise map.
  /// </summary>
  [SerializeField]
  private RandomNoise_SO noiseScriptableObject;

  /// <summary>
  /// The Renderer component attached to this GameObject.
  /// </summary>
  private Renderer _meshRenderer;

  /// <summary>
  /// The generated noise map represented as a 2D array of floats.
  /// </summary>
  private float[,] _noiseMap;

  /// <summary>
  /// Generates and applies the noise texture at the start of the game.
  /// </summary>
  void Start()
  {
    this.GetRenderer();
    this.GenerateTexture();

    if (noiseScriptableObject != null)
    {
      noiseScriptableObject.OnValuesChanged += UpdateNoiseMesh;
    }
  }

  /// <summary>
  /// Regenerates the texture when a property of the scriptable object changes in the editor.
  /// </summary>
  void OnValidate()
  {
    //this.GenerateTexture();
  }

  private void GetRenderer()
  {
    this._meshRenderer = this.GetComponent<MeshRenderer>();
  }

  /// <summary>
  /// Generates a texture from the noise map and applies it to the material of the attached Renderer.
  /// </summary>
  private void GenerateTexture()
  {
    if (!this._meshRenderer)
    {
      throw new ArgumentException("No Mesh Renderer Attached");
    }

    // Generate the noise map using the scriptable object.
    this._noiseMap = this.noiseScriptableObject.GenerateNoiseMap();

    // Create a Texture2D from the noise map.
    Texture2D texture = TextureGenerator.TextureFromHeightMap(this._noiseMap);

    // Apply the texture to the material of the Renderer.
    this._meshRenderer.sharedMaterial.mainTexture = texture;
  }

  [ContextMenu("Update Noise Mesh")]
  public void UpdateNoiseMesh()
  {
    GenerateTexture();
  }
}
