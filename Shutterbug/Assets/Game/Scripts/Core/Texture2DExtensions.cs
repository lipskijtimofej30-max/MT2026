using UnityEngine;

namespace Game.Scripts.Core
{
    public static class Texture2DExtensions
    {
        public static Texture2D ResizeTexture(this Texture2D texture2D, int newWidth, int newHeight)
        {
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Bilinear;
            
            Graphics.Blit(texture2D, rt);
            
            Texture2D result = new Texture2D(newWidth, newHeight, texture2D.format, false);
            
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;
            result.ReadPixels(new Rect(0,0,newWidth,newHeight), 0, 0);
            result.Apply();
            
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            return result;
        }
    }
}