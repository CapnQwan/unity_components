using UnityEngine;

public class PathGrid
{
	private PathChunk[] _chunks;
	public PathChunk[] Chunks => _chunks;

	public void Init()
	{
		_chunks = new PathChunk[4];
		for (int i = 0; i < _chunks.Length; i++)
		{
			int x = i - 1;
			int y = i % 2 - 1;
			_chunks[i] = new PathChunk();
			_chunks[i].Init(new Vector2(x, y));
		}
	}
}
