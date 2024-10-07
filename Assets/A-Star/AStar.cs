using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities.Debugging.Visualization;
using UnityEngine.InputSystem;

namespace VidTools.Examples
{
	public class AStar : MonoBehaviour
	{
		[Header("Display Settings")]
		[Range(0.9f, 1)]
		public float tileSize = 1;
		public Color pathTileCol;
		public Color backgroundCol;

		void Update()
		{
			Draw.Quad(new Vector2(0f, 1f), Vector2.one * tileSize, pathTileCol);
			Draw.Quad(Vector2.one, Vector2.one * tileSize, pathTileCol);
			Camera.main.backgroundColor = backgroundCol;
		}
	}
}