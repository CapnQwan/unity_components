using UnityEngine;

public class PathGrid
{
	private PathChunk[,] _chunks;
	public PathChunk[,] Chunks => _chunks;

	public void Init()
	{
		_chunks = new PathChunk[2, 2];
		for (int x = 0; x < _chunks.GetLength(0); x++)
		{
			for (int y = 0; y < _chunks.GetLength(1); y++)
			{
				_chunks[x, y] = new PathChunk();
				_chunks[x, y].Init(new Vector2(x - 1, y - 1));
			}
		}
	}
}
