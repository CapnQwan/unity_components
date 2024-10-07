using System;
using UnityEngine;

public class PathNode
{
	private PathChunk _pathChunk;
	public PathChunk PathChunk => _pathChunk;
	private Vector2 _nodePosition;
	public Vector2 NodePosition => _nodePosition;
	private PathNode[] _neighbours;
	private bool[] _isWalkable;

	public void Init(PathChunk chunk, Vector2 position)
	{
		_neighbours = new PathNode[8];
		_isWalkable = new bool[8];
		_pathChunk = chunk;
		_nodePosition = position;
	}

	public PathNodeNeighboursDirections GetDirectionOfNeighbour(PathNode node)
	{
		return (PathNodeNeighboursDirections)Array.IndexOf(_neighbours, node);
	}

	public bool IsWalkable(PathNodeNeighboursDirections neighbourIndex)
	{
		return _isWalkable[(int)neighbourIndex];
	}

	public bool IsWalkable(PathNode node)
	{
		return _isWalkable[(int)GetDirectionOfNeighbour(node)];
	}
}
