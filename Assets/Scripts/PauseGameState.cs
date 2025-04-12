using UnityEngine;

public static class PauseGameState
{
    public static bool IsPaused { get; private set; } = false;
    public static float LocalTimeScale => IsPaused ? 0f : 1f;
    public static void Pause()
    {
        IsPaused = true;
    }
    public static void Resume()
    {
        IsPaused = false;
    }
}
