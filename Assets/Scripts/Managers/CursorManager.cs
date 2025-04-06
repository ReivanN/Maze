using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Texture2D defaultCursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public int cursorSize = 512;
    public GameObject pauseObject;

    private Texture2D resizedCursor;
    private bool isCustomCursorActive = false;

    void Start()
    {
        if (cursorTexture != null)
        {
            resizedCursor = ResizeTexture(cursorTexture, cursorSize, cursorSize);
        }

        UpdateCursor();
    }

    void Update()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        bool shouldUseCustomCursor =
            cursorTexture != null &&
            SceneManager.GetActiveScene().name == "MazeScene" &&
            (pauseObject == null || !pauseObject.activeInHierarchy);

        if (shouldUseCustomCursor && !isCustomCursorActive)
        {
            Vector2 hotspot = new Vector2(resizedCursor.width / 2, resizedCursor.height / 2);
            Cursor.SetCursor(resizedCursor, hotspot, cursorMode);
            isCustomCursorActive = true;
        }
        else if (!shouldUseCustomCursor && isCustomCursorActive)
        {
            Cursor.SetCursor(defaultCursorTexture, Vector2.zero, cursorMode);
            isCustomCursorActive = false;
        }
    }

    private Texture2D ResizeTexture(Texture2D source, int width, int height)
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

    void OnDestroy()
    {
        if (resizedCursor != null)
        {
            Destroy(resizedCursor);
        }
    }
}
