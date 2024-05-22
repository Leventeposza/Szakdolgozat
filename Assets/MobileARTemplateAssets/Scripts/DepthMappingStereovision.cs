using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthMapAR : MonoBehaviour
{
    public Camera leftCamera;
    public Camera rightCamera;

    private Texture2D leftTexture;
    private Texture2D rightTexture;
    private Texture2D depthMapTexture;

    private void Start()
    {
        leftTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        rightTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        depthMapTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        // Capture left camera image
        leftTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        leftTexture.Apply();

        // Capture right camera image
        rightTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        rightTexture.Apply();

        // Compute depth map
        ComputeDepthMap();
    }

    private void ComputeDepthMap()
    {
        // Ensure left and right textures have the same dimensions
        if (leftTexture.width != rightTexture.width || leftTexture.height != rightTexture.height)
        {
            Debug.LogError("Left and right camera images must have the same dimensions.");
            return;
        }

        int width = leftTexture.width;
        int height = leftTexture.height;

        // Create a new texture for the depth map
        Color[] depthMapPixels = new Color[width * height];

        // Loop through each pixel in the images
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Get the pixel intensity values from the left and right images
                Color leftColor = leftTexture.GetPixel(x, y);
                Color rightColor = rightTexture.GetPixel(x, y);

                // Compute the disparity (difference in x-coordinate) between corresponding pixels in the left and right images
                int disparity = (int)(Mathf.Abs(leftColor.grayscale - rightColor.grayscale) * 255f); // Assuming images are grayscale

                // Normalize depth value to fit within 8-bit range (0-255)
                int normalizedDepth = Mathf.Clamp(disparity, 0, 255);

                // Set the depth value in the depth map texture
                depthMapPixels[y * width + x] = new Color(normalizedDepth / 255f, normalizedDepth / 255f, normalizedDepth / 255f);
            }
        }

        // Update the depth map texture with the computed depth map
        depthMapTexture.SetPixels(depthMapPixels);
        depthMapTexture.Apply();

        // Display the depth map texture on a GameObject 
        GetComponent<Renderer>().material.mainTexture = depthMapTexture;
    }
}