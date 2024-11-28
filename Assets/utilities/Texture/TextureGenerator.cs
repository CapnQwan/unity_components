using UnityEngine;

public static class TextureGenerator
{
    /// <summary>
    /// Creates a Texture2D from a color map.
    /// </summary>
    /// <param name="colourMap">An array of Color values representing the texture.</param>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <returns>A Texture2D object generated from the provided color map.</returns>
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        // Create a new Texture2D with the specified dimensions
        Texture2D texture = new Texture2D(width, height);

        // Set texture filter mode to Point for pixelated scaling
        texture.filterMode = FilterMode.Point;

        // Set texture wrap mode to Clamp to avoid tiling
        texture.wrapMode = TextureWrapMode.Clamp;

        // Apply the color map to the texture
        texture.SetPixels(colourMap);

        // Finalize the texture changes
        texture.Apply();

        // Return the generated texture
        return texture;
    }

    /// <summary>
    /// Creates a Texture2D from a height map.
    /// </summary>
    /// <param name="heightMap">A 2D array of float values representing height data.</param>
    /// <returns>A Texture2D object generated from the height map, with colors mapped from black (low) to white (high).</returns>
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        // Get the dimensions of the height map
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // Create an array to store the color values for the texture
        Color[] colourMap = new Color[width * height];

        // Iterate over each position in the height map
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Map height values to colors from black (0) to white (1)
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        // Generate a texture from the computed color map
        return TextureFromColourMap(colourMap, width, height);
    }
}
