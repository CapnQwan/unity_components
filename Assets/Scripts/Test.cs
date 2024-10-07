using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities.Debugging.Visualization;
using UnityEngine;

namespace VidTools.Examples
{
    public class Test : MonoBehaviour
    {
        [Header("Settings")]
        public Vector2Int tilePosition;

        [Header("Display Settings")]
        [Range(0.1f, 10f)]
        public float tileSize = 1;
        public Color tileCol;
        public Color backgroundCol;

        // Update is called once per frame
        void Update()
        {

            Draw.Quad(tilePosition, Vector2.one * tileSize, tileCol);
            Camera.main.backgroundColor = backgroundCol;
        }
    }
}