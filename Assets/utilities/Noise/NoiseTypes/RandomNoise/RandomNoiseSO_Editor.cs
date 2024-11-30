using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomNoise_SO), true)]
public class RandomNoiseSO_Editor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    if (GUI.changed)
    {
      RandomNoise_SO noiseSO = (RandomNoise_SO)target;
      noiseSO.OnValuesChanged?.Invoke();
    }
  }
}
