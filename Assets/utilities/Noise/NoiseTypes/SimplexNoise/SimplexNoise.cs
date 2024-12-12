using System;
using System.Linq;
using UnityEngine;

namespace Noise
{
  /// <summary>
  /// A static class for generating Simplex noise maps using customizable parameters.
  /// </summary>
  public static class SimplexNoise
  {

    private static readonly int[] Permutation = Enumerable
        .Repeat(new int[] { 151, 160, 137, 91, 90, 15 }, 512 / 6 + 1)
        .SelectMany(x => x)
        .Take(512)
        .ToArray();

    private static readonly int[] P;

    private static readonly Vector2[] Gradients = {
            new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1),
            new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
        };

    /// <summary>Default width of the generated noise map.</summary>
    public const int DEFAULT_WIDTH = 50;

    /// <summary>Default height of the generated noise map.</summary>
    public const int DEFAULT_HEIGHT = 50;

    /// <summary>Default seed for the random number generator.</summary>
    public const int DEFAULT_SEED = 0;

    /// <summary>Default scale factor for the noise map.</summary>
    public const float DEFAULT_SCALE = 0.5f;

    /// <summary>Default lacunarity value for controlling frequency.</summary>
    public const float DEFAULT_LACUNARITY = 2.0f;

    /// <summary>Default persistence value for controlling amplitude reduction.</summary>
    public const float DEFAULT_PERSISTANCE = 0.5f;

    /// <summary>Default number of octaves for generating multi-layer noise.</summary>
    public const int DEFAULT_OCTAVES = 4;

    /// <summary>Default normalization mode for noise values.</summary>
    public const NormalizeMode DEFAULT_NORMALIZE_MODE = NormalizeMode.Local;

    /// <summary>Default offset applied to the noise map coordinates.</summary>
    public static readonly Vector2 DEFAULT_OFFSET = new Vector2(0f, 0f);


    /// <summary>
    /// Generates a Simplex noise map using a <see cref="SimplexNoise_SO"/> scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    public static float[,] GenerateSimplexNoiseMap(int width, int height, SimplexNoise_SO noiseScriptableObject)
    {
      return GenerateSimplexNoiseMap(
        width,
        height,
        noiseScriptableObject.Seed,
        noiseScriptableObject.Octaves,
        noiseScriptableObject.Scale,
        noiseScriptableObject.Lacunarity,
        noiseScriptableObject.Persistance,
        noiseScriptableObject.NormalizeMode,
        noiseScriptableObject.Offset);
    }

    /// <summary>
    /// Generates a Simplex noise map using a <see cref="SimplexNoise_SO"/> scriptable object.
    /// </summary>
    /// <param name="noiseScriptableObject">The scriptable object containing noise parameters.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    public static float[,] GenerateSimplexNoiseMap(SimplexNoise_SO noiseScriptableObject)
    {
      return GenerateSimplexNoiseMap(
        noiseScriptableObject.Width,
        noiseScriptableObject.Height,
        noiseScriptableObject.Seed,
        noiseScriptableObject.Octaves,
        noiseScriptableObject.Scale,
        noiseScriptableObject.Lacunarity,
        noiseScriptableObject.Persistance,
        noiseScriptableObject.NormalizeMode,
        noiseScriptableObject.Offset);
    }

    /// <summary>
    /// Generates a Simplex noise map using <see cref="SimplexNoiseParameters"/>.
    /// </summary>
    /// <param name="simplexNoiseParameters">A struct containing all the parameters for the noise generation.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    public static float[,] GenerateSimplexNoiseMap(SimplexNoiseParameters simplexNoiseParameters)
    {
      return GenerateSimplexNoiseMap(
        simplexNoiseParameters.Width,
        simplexNoiseParameters.Height,
        simplexNoiseParameters.Seed,
        simplexNoiseParameters.Octaves,
        simplexNoiseParameters.Scale,
        simplexNoiseParameters.Lacunarity,
        simplexNoiseParameters.Persistance,
        simplexNoiseParameters.NormalizeMode,
        simplexNoiseParameters.Offset);
    }

    /// <summary>
    /// Generates a Simplex noise map with specified parameters.
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="seed">Seed for the random number generator.</param>
    /// <param name="octaves">Number of octaves to use for multi-layered noise.</param>
    /// <param name="scale">Scale factor for the noise map.</param>
    /// <param name="lacunarity">Controls the frequency of each octave.</param>
    /// <param name="persistence">Controls the amplitude reduction of each octave.</param>
    /// <param name="normalizeMode">Specifies how to normalize the noise values.</param>
    /// <param name="offset">Offset applied to the noise map coordinates.</param>
    /// <returns>A 2D array of float values representing the generated noise map.</returns>
    public static float[,] GenerateSimplexNoiseMap(
        int width = DEFAULT_WIDTH,
        int height = DEFAULT_HEIGHT,
        int seed = DEFAULT_SEED,
        int octaves = DEFAULT_OCTAVES,
        float scale = DEFAULT_SCALE,
        float lacunarity = DEFAULT_LACUNARITY,
        float persistence = DEFAULT_PERSISTANCE,
        NormalizeMode normalizeMode = DEFAULT_NORMALIZE_MODE,
        Vector2 offset = default)
    {
      if (width <= 0 || height <= 0) { throw new ArgumentException("Width and height must be greater than zero."); }
      if (scale <= 0)
      {
        scale = 0.0001f;
      }

      float[,] noiseMap = new float[width, height];
      System.Random prng = new System.Random(seed);

      // Generate octave offsets
      Vector2[] octaveOffsets = new Vector2[octaves];
      for (int i = 0; i < octaves; i++)
      {
        float offsetX = prng.Next(-100000, 100000) + offset.x;
        float offsetY = prng.Next(-100000, 100000) + offset.y;
        octaveOffsets[i] = new Vector2(offsetX, offsetY);
      }

      float maxPossibleHeight = 0;
      float amplitude = 1;

      for (int i = 0; i < octaves; i++)
      {
        maxPossibleHeight += amplitude;
        amplitude *= persistence;
      }

      float maxLocalNoiseHeight = float.MinValue;
      float minLocalNoiseHeight = float.MaxValue;

      float halfWidth = width * 0.5f;
      float halfHeight = height * 0.5f;

      // Thread-safe variables for min/max values
      object lockObject = new object();

      _ = System.Threading.Tasks.Parallel.For(0, height, y =>
     {
       float localMax = float.MinValue;
       float localMin = float.MaxValue;

       for (int x = 0; x < width; x++)
       {
         float amplitude = 1;  // Thread-local variable
         float frequency = 1;  // Thread-local variable
         float noiseHeight = 0;

         for (int i = 0; i < octaves; i++)
         {
           float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
           float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

           float simplexValue = Evaluate(sampleX, sampleY);
           noiseHeight += simplexValue * amplitude;

           amplitude *= persistence;
           frequency *= lacunarity;
         }

         // Thread-local min/max
         localMax = Mathf.Max(localMax, noiseHeight);
         localMin = Mathf.Min(localMin, noiseHeight);

         // Write directly to noiseMap
         noiseMap[x, y] = noiseHeight;
       }

       // Update global min/max safely
       lock (lockObject)
       {
         maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight, localMax);
         minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight, localMin);
       }
     });

      // Normalize the noise map
      _ = System.Threading.Tasks.Parallel.For(0, height, y =>
      {
        for (int x = 0; x < width; x++)
        {
          if (normalizeMode == NormalizeMode.Local)
          {
            noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
          }
          else
          {
            float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight);
            noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, 1);
          }
        }
      });

      return noiseMap;
    }

    static SimplexNoise()
    {
      // Double the permutation table to avoid overflow issues
      P = new int[512];
      for (int i = 0; i < 512; i++)
        P[i] = Permutation[i % 256];
    }

    /// <summary>
    /// Evaluate 2D Simplex Noise at given coordinates.
    /// </summary>
    private static float Evaluate(float x, float y)
    {
      float F2 = 0.5f * (Mathf.Sqrt(3f) - 1f); // Skew factor for 2D
      float G2 = (3f - Mathf.Sqrt(3f)) / 6f;  // Unskew factor for 2D

      // Skew input space to determine which simplex cell we're in
      float s = (x + y) * F2;
      int i = Mathf.FloorToInt(x + s);
      int j = Mathf.FloorToInt(y + s);

      // Unskew the cell origin back to (x, y) space
      float t = (i + j) * G2;
      float X0 = i - t;
      float Y0 = j - t;
      float x0 = x - X0;
      float y0 = y - Y0;

      // Determine simplex corner offsets
      int i1, j1;
      if (x0 > y0) { i1 = 1; j1 = 0; } // Lower triangle
      else { i1 = 0; j1 = 1; }        // Upper triangle

      float x1 = x0 - i1 + G2;
      float y1 = y0 - j1 + G2;
      float x2 = x0 - 1f + 2f * G2;
      float y2 = y0 - 1f + 2f * G2;

      // Hash the corners
      int ii = i & 255;
      int jj = j & 255;

      int g0 = P[ii + P[jj]] % 8;
      int g1 = P[ii + i1 + P[jj + j1]] % 8;
      int g2 = P[ii + 1 + P[jj + 1]] % 8;

      // Calculate the contributions from the three corners
      float t0 = 0.5f - x0 * x0 - y0 * y0;
      float n0 = (t0 < 0) ? 0.0f : (t0 * t0) * Vector2.Dot(Gradients[g0], new Vector2(x0, y0));

      float t1 = 0.5f - x1 * x1 - y1 * y1;
      float n1 = (t1 < 0) ? 0.0f : (t1 * t1) * Vector2.Dot(Gradients[g1], new Vector2(x1, y1));

      float t2 = 0.5f - x2 * x2 - y2 * y2;
      float n2 = (t2 < 0) ? 0.0f : (t2 * t2) * Vector2.Dot(Gradients[g2], new Vector2(x2, y2));

      // Add contributions and scale result
      return 70.0f * (n0 + n1 + n2);
    }
  }
}
