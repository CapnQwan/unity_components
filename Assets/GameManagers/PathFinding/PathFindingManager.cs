using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities.Debugging.Visualization;
using UnityEngine.InputSystem;

public class PathFindingManager : MonoBehaviour
{
	public static PathFindingManager Instance;
	private PathGrid _pathGrid;
	private PathNode[] _nodes;
	public Color col;
	public Color backgroundCol;
	public Color pathTileCol;
	public float tileSize = 1;

	private void Awake()
	{
		// Ensure only one instance of the GameManager exists
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);  // Keeps the PathFindingManager between scenes
		}
		else
		{
			Destroy(gameObject);
		}

		_pathGrid = new PathGrid();
		_pathGrid.Init();
		int nodeCount = _pathGrid.Chunks[0, 0].Nodes.Length * _pathGrid.Chunks.Length;
		_nodes = new PathNode[nodeCount];
		int index = 0;
		foreach (PathChunk chunk in _pathGrid.Chunks)
		{
			foreach (PathNode node in chunk.Nodes)
			{
				_nodes[index] = node;
				index++;
			}
		}
	}

	private void Update()
	{
		//Draw.Quad(Vector2.zero, Vector2.one * tileSize, pathTileCol);
		//Draw.Quad(Vector2.one, Vector2.one * tileSize, col);

		foreach (PathNode node in _nodes)
		{
			Vector3 position = node.NodePosition;
			Draw.Quad(position, Vector2.one * tileSize, Color.gray);
		}

		Camera.main.backgroundColor = backgroundCol;
	}

	public PathNode[] GetNodes()
	{
		return _nodes;
	}
}
