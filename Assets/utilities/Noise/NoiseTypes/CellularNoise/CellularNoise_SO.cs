using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise/CellularNoise", order = 3)]
public class CellularNoise_SO : Noise_SO
{
  [SerializeField]
  private int cellsCountX, cellsCountY;
  public int CellCountX => cellsCountX;
  public int CellCountY => cellsCountY;
}
