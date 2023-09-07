using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blur : MonoBehaviour
{
    public Material BlurMaterial;

    [Tooltip("µü´ú´ÎÊý")]
    public int iterations;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        for (int i = 0; i < iterations; i++)
        {
            RenderTexture buffer = RenderTexture.GetTemporary(source.width, source.height, 0);
            Graphics.Blit(source, buffer, BlurMaterial);
            RenderTexture.ReleaseTemporary(source);
            source = buffer;
        }

        Graphics.Blit(source, destination);
    }
}
