using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    [HideInInspector]public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
    public int cursorSize = 8;

    void Start()
    {
        if (cursorTexture != null && SceneManager.GetActiveScene().name == "MazeScene")
        {
            Texture2D resizedCursor = ResizeTexture(cursorTexture, cursorSize, cursorSize);
            Cursor.SetCursor(resizedCursor, hotspot, cursorMode);
        }
    }

    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 32);
        RenderTexture.active = rt;

        Graphics.Blit(source, rt);

        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        rt.Release();

        return result;
    }
}