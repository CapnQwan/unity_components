namespace Noise
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using UnityEngine;

  public static class BlueNoise
  {
    public static float[,] GenerateBlueNoiseMap(BlueNoise_SO nosieScriptableObject)
    {
      return GenerateBlueNoiseMap(
        nosieScriptableObject.Width,
        nosieScriptableObject.Height,
        nosieScriptableObject.MinDistance,
        nosieScriptableObject.MaxAttempts);
    }

    /// <summary>
    /// Generates a blue noise map using a dart-throwing algorithm.
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="minDistance">Minimum distance between points in the noise.</param>
    /// <param name="maxAttempts">Maximum number of attempts to place a point in an empty area.</param>
    /// <returns>A 2D float array representing the blue noise map.</returns>
    public static float[,] GenerateBlueNoiseMap(int width, int height, float minDistance, int maxAttempts = 30)
    {
      float[,] noiseMap = new float[width, height];
      ConcurrentBag<Vector2> points = new ConcurrentBag<Vector2>();
      ConcurrentQueue<Vector2> processingQueue = new ConcurrentQueue<Vector2>();

      // Start with a random point using Unity's Random
      Vector2 initialPoint = new Vector2(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
      points.Add(initialPoint);
      processingQueue.Enqueue(initialPoint);
      noiseMap[(int)initialPoint.x, (int)initialPoint.y] = 1f; // Mark as filled

      // Create a thread-safe random number generator for parallel tasks
      ParallelOptions parallelOptions = new ParallelOptions();
      _ = Parallel.ForEach(Partitioner.Create(0, maxAttempts), parallelOptions, (range, state) =>
      {
        System.Random threadRandom = new System.Random(Guid.NewGuid().GetHashCode());

        while (!processingQueue.IsEmpty)
        {
          if (processingQueue.TryDequeue(out Vector2 currentPoint))
          {
            for (int i = range.Item1; i < range.Item2; i++)
            {
              // Generate a random point around the current point
              float angle = (float)(threadRandom.NextDouble() * Math.PI * 2);
              float distance = (float)(threadRandom.NextDouble() * (2 * minDistance - minDistance) + minDistance);
              Vector2 newPoint = currentPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

              if (IsValidPoint(newPoint, points, width, height, minDistance))
              {
                points.Add(newPoint);
                processingQueue.Enqueue(newPoint);
                lock (noiseMap)
                {
                  noiseMap[(int)newPoint.x, (int)newPoint.y] = 1f; // Mark as filled
                }
              }
            }
          }
        }
      });

      // Normalize the map to have values between 0 and 1
      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          noiseMap[x, y] = noiseMap[x, y] > 0 ? 1f : 0f;
        }
      }

      return noiseMap;
    }

    /// <summary>
    /// Checks if a point is valid to be added to the noise map.
    /// </summary>
    private static bool IsValidPoint(Vector2 point, IEnumerable<Vector2> points, int width, int height, float minDistance)
    {
      if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
      {
        return false; // Out of bounds
      }

      foreach (Vector2 existingPoint in points)
      {
        if (Vector2.Distance(existingPoint, point) < minDistance)
        {
          return false; // Too close to another point
        }
      }

      return true;
    }
  }
}