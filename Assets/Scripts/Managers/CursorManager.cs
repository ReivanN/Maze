using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Texture2D defaultCursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public int cursorSize = 512;
    public GameObject pauseObject;

    void Update()
    {
        if (cursorTexture != null && SceneManager.GetActiveScene().name == "MazeScene" && !pauseObject.activeInHierarchy)
        {
            Texture2D resizedCursor = ResizeTexture(cursorTexture, cursorSize, cursorSize);
            Vector2 hotspot = new Vector2(resizedCursor.width / 2, resizedCursor.height / 2);
            Cursor.SetCursor(resizedCursor, hotspot, cursorMode);
        }
        else 
        {
            Vector2 hotspot = Vector2.zero;
            Cursor.SetCursor(defaultCursorTexture, hotspot, cursorMode);
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
