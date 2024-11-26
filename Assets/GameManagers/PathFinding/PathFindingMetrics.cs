public static class PathFindingMetrics
{

	public static int ChunkSize = 10;
	public static int ChunkNodeCount = ChunkSize * ChunkSize;

	public static float PathNodeDistanceStraight = 1f;
	public static float PathNodeDistanceAngle = 1.414f;

	public static float GetPathNodeDistance(int neighbourIndex)
	{
		if (neighbourIndex % 2 == 0)
		{
			return PathNodeDistanceStraight;
		}

		return PathNodeDistanceAngle;
	}

	public static float GetPathNodeDistance(PathNodeNeighboursDirections neighbourIndex)
	{
		return GetPathNodeDistance((int)neighbourIndex);
	}

	public static float GetPathNodeDistance(PathNode nodeA, PathNode nodeB)
	{
		PathNodeNeighboursDirections direction = nodeA.GetDirectionOfNeighbour(nodeB);
		return GetPathNodeDistance(direction);
	}

	public static float GetPathNodeDistance(PathNode nodeA, PathNodeNeighboursDirections direction)
	{
		return GetPathNodeDistance(direction);
	}
}

public enum PathNodeNeighboursDirections
{
	top = 0,
	topRight = 1,
	right = 2,
	bottomRight = 3,
	bottom = 4,
	bottomLeft = 5,
	left = 6,
	topLeft = 7
}