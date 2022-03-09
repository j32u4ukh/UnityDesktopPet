using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class Builder
{   
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string path)
    {
        Debug.Log($"[Builder] OnPostprocessBuild | target: {target}, path: {path}");

        if (DualScreenManager.use_dual_screen)
        {
            Debug.Log($"[Builder] OnPostprocessBuild | Dual screen");
            PlayerSettings.cursorHotspot = new Vector2(x: 1920.0f * 2.0f, y: 1080.0f);
        }
        else
        {
            Debug.Log($"[Builder] OnPostprocessBuild | Single screen");
            PlayerSettings.cursorHotspot = new Vector2(x: 1920.0f, y: 1080.0f);
        }
    }
}
