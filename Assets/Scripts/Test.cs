using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        private CommandBuffer cmd;
        public Mesh Mesh;
        private MaterialPropertyBlock materialProperties;
        public Material DrawMaterial;
        private int lastFrameWithDrawCommands;
        const string buffName = "Vis Draw Commands";

        void Awake()
        {
            materialProperties = new MaterialPropertyBlock();
            materialProperties.SetColor("_Color", tileCol);

            cmd = new CommandBuffer();
            cmd.name = "Test buffer";

            Camera.onPreRender -= PreRender;
            Camera.onPreRender += PreRender;

            if (Camera.main != null)
            {
                //Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, cmd);
            }
            else
            {
                Debug.LogWarning("No main camera found.");
            }
        }

        void OnDestroy()
        {
            // Clean up the command buffer when the object is destroyed
            if (Camera.main)
            {
                Camera.main.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, cmd);
            }
        }

        // Update is called once per frame
        void Update()
        {

            //Draw.Quad(tilePosition, Vector2.one * tileSize, tileCol);

            // Clear the command buffer so it doesn't keep rendering the previous frame's commands
            cmd.Clear();

            if (DrawMaterial == null)
            {
                Debug.LogWarning("DrawMaterial is not assigned.");
            }

            if (Mesh == null)
            {
                Debug.LogWarning("Mesh is not assigned.");
            }

            // Draw the mesh with the material and property block
            cmd.DrawMesh(Mesh, Matrix4x4.identity, DrawMaterial, 0, 0, materialProperties);
            Camera.main.backgroundColor = backgroundCol;
        }


        private void PreRender(Camera cam)
        {
            CameraEvent drawCameraEvent = CameraEvent.BeforeImageEffects;

            var allBuffers = cam.GetCommandBuffers(drawCameraEvent);

            // Remove buffer by name.
            // This is done because there are situations in which the buffer can be
            // null (but still attached to camera), and I don't want to think about it.
            foreach (var b in allBuffers)
            {
                if (string.Equals(b.name, buffName, System.StringComparison.Ordinal))
                {
                    cam.RemoveCommandBuffer(drawCameraEvent, b);
                }
            }


            cam.AddCommandBuffer(drawCameraEvent, cmd);

        }

    }
}
