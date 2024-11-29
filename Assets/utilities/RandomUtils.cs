using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtils
{
    private static int _seed = 0;
    private static System.Random _prng = new System.Random(_seed);

    private static void updateSeed(int seed) {
        if (_seed != seed) {
            _seed = seed;
            _prng = new System.Random(_seed);
        }
    }

    public static float GetRandomSeededFloat(int seed)
    {
        this.updateSeed(seed);
        return (float)prng.NextDouble();
    }

    public static float GetRandomSeededfloatInRange(int seed, float min, float max)
    {
        this.updateSeed(seed);
        return (float)prng.NextDouble() * (max - min) + min;
    }

    public static float GetRandomSmoothedFloatValue(float x, float y, int seed, Vector2 offset)
    {
        this.updateSeed(seed);

        float offsetX = (float)prng.NextDouble() * 200000 - 100000 + offset.x;
        float offsetY = (float)prng.NextDouble() * 200000 - 100000 + offset.y;

        float sampleX = x + offsetX;
        float sampleY = y + offsetY;
        
        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
        return perlinValue;
    }
}
