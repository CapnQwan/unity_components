using UnityEngine;

public class PathChunk
{
	private Vector2 _chunkPosition;
	public Vector2 ChunkPosition => _chunkPosition;
	private PathNode[,] _nodes;
	public PathNode[,] Nodes => _nodes;
	private PathChunk[] _neighbours;

	public void Init(Vector2 position)
	{
		_chunkPosition = position;
		_nodes = new PathNode[PathFindingMetrics.ChunkSize, PathFindingMetrics.ChunkSize];

		float chunkXOffset = _chunkPosition.x * PathFindingMetrics.ChunkSize;
		float chunkYOffset = _chunkPosition.y * PathFindingMetrics.ChunkSize;

		for (int x = 0; x < PathFindingMetrics.ChunkSize; x++)
		{
			for (int y = 0; y < PathFindingMetrics.ChunkSize; y++)
			{
				_nodes[x, y] = new PathNode();
				_nodes[x, y].Init(this, new Vector2(chunkXOffset + x, chunkYOffset + y));
			}
		}
		_neighbours = new PathChunk[8];
	}

	public PathNode GetNode(int index)
	{
		int x = index % PathFindingMetrics.ChunkSize;
		int y = index / PathFindingMetrics.ChunkSize;
		return _nodes[x, y];
	}

	public PathNode GetNode(int x, int y)
	{
		return _nodes[x, y];
	}
}
