using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Mesh_SO), true)]
public class MeshSO_Editor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    if (GUI.changed)
    {
      Mesh_SO noiseSO = (Mesh_SO)target;
      noiseSO.OnValuesChanged?.Invoke();
    }
  }
}
